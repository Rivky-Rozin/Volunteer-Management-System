namespace Helpers;
using DalApi;
using DO;

internal static class CallManager
{

    internal static ObserverManager Observers = new(); //stage 5 
    private static IDal s_dal = Factory.Get; //stage 4
    internal static BO.Call ConvertToBO(DO.Call call)
    {
        // Correcting the type mismatch issue by ensuring the correct type is used for assignments

        List<BO.CallAssignInList> assignments;
        lock (AdminManager.BlMutex)
            assignments = s_dal.Assignment.ReadAll(a => a.CallId == call.Id)
                .Select(a => new BO.CallAssignInList
                {
                    VolunteerId = a.VolunteerId,
                    VolunteerName = s_dal.Volunteer.Read(a.VolunteerId)?.Name ?? "Unknown",
                    StartTreatmentTime = a.StartTreatment,
                    FinishTreatmentTime = a.EndTreatmentTime ?? null,
                    EndOfTreatmentType = a.TreatmentType.HasValue ? (BO.TreatmentEndTypeEnum)a.TreatmentType.Value : (BO.TreatmentEndTypeEnum?)null
                }).ToList();

        return new BO.Call
        {
            Id = call.Id,
            CallType = (BO.CallType)call.CallType,
            Address = call.FullAddress,
            Latitude = call.Latitude,
            Longitude = call.Longitude,
            CreationTime = call.OpenTime,
            Description = call.Description,
            MaxFinishTime = call.MaxCallTime,
            Status = GetCallStatus(call.Id),
            Assignments = assignments // Ensure the assignments are correctly set in the BO.Call object
        };
    }

    internal static DO.Call ConvertToDO(BO.Call call) => new DO.Call(
        call.Id,
        (DO.CallType)call.CallType,
        call.Address,
        call.Latitude,
        call.Longitude,
        call.CreationTime,
        call.Description,
        call.MaxFinishTime ?? AdminManager.Now.Add(AdminManager.TreatmentTime)
    );

    internal static BO.CallInList ConvertToCallInList(DO.Call call)
    {
        // שליפת כל ההקצאות של הקריאה הזו
        List<DO.Assignment> assignments;
        lock (AdminManager.BlMutex) //stage 7
            assignments = s_dal.Assignment.ReadAll(a => a.CallId == call.Id).ToList();

        // סטטוס הקריאה מחושב לפי ההגדרות במחלקת CallManager
        BO.CallStatus status = GetCallStatus(call.Id);

        // המתנדב האחרון (אם יש הקצאות) לפי זמן התחלה
        DO.Assignment? lastAssignment = assignments
            .Where(a => a.VolunteerId != 0) // לוודא שזה לא הקצאה ריקה
            .OrderByDescending(a => a.StartTreatment)
            .FirstOrDefault();

        string? lastVolunteerName = null;
        if (lastAssignment != null)
        {
            try
            {
                lock (AdminManager.BlMutex) //stage 7
                {

                    DO.Volunteer? volunteer = s_dal.Volunteer.Read(lastAssignment.VolunteerId);
                    lastVolunteerName = volunteer?.Name;
                }
            }
            catch
            {
                lastVolunteerName = null; // אם אין מתנדב כזה
            }
        }

        // חישוב זמן כולל של טיפול (לפי כל ההקצאות שכבר הסתיימו)
        TimeSpan totalTreatmentTime = assignments
            .Where(a => a.EndTreatment != null && a.StartTreatment != null)
            .Aggregate(TimeSpan.Zero, (sum, a) => sum + ((DateTime)a.EndTreatment! - a.StartTreatment));

        // זמן עד שהוקצתה הקריאה לראשונה (אם קיימת הקצאה)
        TimeSpan? timeUntilAssigning = null;
        DO.Assignment? firstAssign = assignments.OrderBy(a => a.StartTreatment).FirstOrDefault();
        if (firstAssign != null)
            timeUntilAssigning = firstAssign.StartTreatment - call.OpenTime;

        return new BO.CallInList
        {
            Id = call.Id,
            CallId = call.Id,
            CallType = (BO.CallType)call.CallType,
            OpenTime = call.OpenTime,
            TimeUntilAssigning = timeUntilAssigning,
            LastVolunteerName = lastVolunteerName,
            totalTreatmentTime = totalTreatmentTime,
            Status = status,
            NumberOfAssignments = assignments.Count
        };
    }

    // מתודת עזר לבדיקת התאמה לסינון
    internal static bool MatchesFilter(BO.CallInList call, BO.CallInListField field, object value)
    {
        return field switch
        {
            BO.CallInListField.Id => int.TryParse(value.ToString(), out var id) && call.Id == id,

            BO.CallInListField.CallId => int.TryParse(value.ToString(), out var callId) && call.CallId == callId,

            BO.CallInListField.CallType => Enum.TryParse(typeof(BO.CallType), value.ToString(), out var typeObj)
                                        && call.CallType == (BO.CallType)typeObj,

            BO.CallInListField.OpenTime => DateTime.TryParse(value.ToString(), out var openTime)
                                        && call.OpenTime.Date == openTime.Date,

            BO.CallInListField.TimeUntilAssigning => TimeSpan.TryParse(value.ToString(), out var assignTime)
                                                  && call.TimeUntilAssigning?.TotalMinutes == assignTime.TotalMinutes,

            BO.CallInListField.LastVolunteerName => call.LastVolunteerName == value.ToString(),

            BO.CallInListField.totalTreatmentTime => TimeSpan.TryParse(value.ToString(), out var totalTime)
                                                  && call.totalTreatmentTime?.TotalMinutes == totalTime.TotalMinutes,

            BO.CallInListField.Status => Enum.TryParse(typeof(BO.CallStatus), value.ToString(), out var statusObj)
                                      && call.Status == (BO.CallStatus)statusObj,

            BO.CallInListField.NumberOfAssignments => int.TryParse(value.ToString(), out var count)
                                                   && call.NumberOfAssignments == count,

            _ => false
        };
    }

    public static BO.ClosedCallInList ConvertToClosedCallInList(DO.Call call)
    {
        // שליפת הקריאה כקריאה לוגית שכוללת את ההקצאות
        BO.Call bocall = ConvertToBO(call);

        // רשימת ההקצאות
        List<BO.CallAssignInList> assignments = bocall.Assignments;

        if (assignments.Count == 0)
            throw new BO.BlFailedToCreate("Cannot create ClosedCallInList: no assignments found for call.");

        // מיון ההקצאות לפי זמן התחלה
        var sortedAssignments = assignments.OrderBy(a => a.StartTreatmentTime).ToList();

        // תחילת טיפול - מההקצאה הראשונה
        DateTime entryToTreatment = sortedAssignments.First().StartTreatmentTime;

        // סיום טיפול - מההקצאה האחרונה עם זמן סיום לא null
        var lastFinishedAssignment = sortedAssignments
            .Where(a => a.FinishTreatmentTime != null)
            .OrderByDescending(a => a.FinishTreatmentTime)
            .FirstOrDefault();

        DateTime? actualEndTime = lastFinishedAssignment?.FinishTreatmentTime;
        BO.TreatmentEndTypeEnum? endType = lastFinishedAssignment?.EndOfTreatmentType;

        return new BO.ClosedCallInList
        {
            Id = call.Id,
            CallType = (BO.CallType)call.CallType,
            FullAddress = call.FullAddress,
            OpenTime = call.OpenTime,
            EntryToTreatmentTime = entryToTreatment,
            ActualTreatmentEndTime = actualEndTime,
            TreatmentEndType = (BO.TreatmentEndTypeEnum?)endType
        };
    }

    internal static BO.CallStatus GetCallStatus(int callId)
    {
        lock (AdminManager.BlMutex) //stage 7
        {

            TimeSpan riskThreshold = s_dal.Config.RiskTimeSpan;
            var call = s_dal.Call.Read(callId)
                       ?? throw new BO.BlDoesNotExistException("Call");

            var assignments = s_dal.Assignment.ReadAll(a => a.CallId == callId).ToList();

            DateTime now = AdminManager.Now;


            bool hasActiveAssignment = assignments.Any(a => a.EndTreatment == null);
            bool hasCompletedAssignment = assignments.Any(a => a.EndTreatment != null);

            bool isExpired = call.MaxCallTime != null && now > call.MaxCallTime;
            bool isRisk = call.MaxCallTime != null &&
                          (call.MaxCallTime.Value - now) <= riskThreshold &&
                          (call.MaxCallTime.Value - now) > TimeSpan.Zero;

            // סגורה – אם לפחות מתנדב אחד סיים טיפול
            if (hasCompletedAssignment)
                return BO.CallStatus.Closed;

            // בטיפול – יש מתנדב שמטפל כרגע
            if (hasActiveAssignment)
            {
                if (isExpired)
                    return BO.CallStatus.Expired;
                if (isRisk)
                    return BO.CallStatus.InProgressAtRisk;
                return BO.CallStatus.InProgress;
            }

            // פתוחה – לא קיימת הקצאה פעילה
            if (isExpired)
                return BO.CallStatus.Expired;
            if (isRisk)
                return BO.CallStatus.OpenAtRisk;
            return BO.CallStatus.Open;
        }
    }
    internal static void UpdateExpiredOpenCalls()
    {
        DateTime now = AdminManager.Now;

        // שליפת כל הקריאות הפתוחות – ToList מיידי!
        List<Call> allOpenCalls;
        lock (AdminManager.BlMutex)
            allOpenCalls = s_dal.Call
                .ReadAll(call => !(GetCallStatus(call.Id) == BO.CallStatus.Closed) && call.MaxCallTime <= now)
                .ToList();

        // רשימה של קריאות שצריך לשלוח עליהן Notification
        List<int> callsToNotify = new();

        foreach (var call in allOpenCalls)
        {
            DO.Assignment? assignment;
            lock (AdminManager.BlMutex)
                assignment = s_dal.Assignment.Read(a => a.CallId == call.Id);

            if (assignment == null)
            {
                lock (AdminManager.BlMutex)
                {
                    s_dal.Assignment.Create(new DO.Assignment
                    {
                        CallId = call.Id,
                        VolunteerId = 0,
                        StartTreatment = call.OpenTime,
                        EndTreatment = now,
                        TreatmentType = DO.TreatmentType.ExpiredCancel
                    });
                }

                // נאסוף את הקריאה להתראה, אבל לא נשלח עדיין
                callsToNotify.Add(call.Id);
            }
            else if (assignment.EndTreatment == null)
            {
                DO.Assignment newAssignment = new
                (
                   assignment.Id,
                   assignment.VolunteerId,
                   call.Id,
                   assignment.StartTreatment,
                   now,
                   DO.TreatmentType.ExpiredCancel
                );

                lock (AdminManager.BlMutex)
                    s_dal.Assignment.Update(newAssignment);

                callsToNotify.Add(call.Id);
            }
        }

        // שליחת התראות אחרי סיום כל ה-lockים
        if (callsToNotify.Count > 0)
            Observers.NotifyListUpdated(); // כללי, או לפי מזהים – תלוי במימוש שלך
    }

    public static async Task UpdateCallCoordinatesAsync(DO.Call doCall)
    {
        if (!string.IsNullOrWhiteSpace(doCall.FullAddress))
        {
            try
            {
                var (lat, lon) = await Tools.GetCoordinatesFromAddress(doCall.FullAddress);

                doCall = doCall with { Latitude = lat, Longitude = lon };

                lock (AdminManager.BlMutex)
                    s_dal.Call.Update(doCall); // עדכון הקואורדינטות

                CallManager.Observers.NotifyItemUpdated(doCall.Id);
                CallManager.Observers.NotifyListUpdated();
            }
            catch(Exception ex)
            {
                throw new BO.BlFailedToCreateException( "Failed to update call coordinates. Please check the address.", ex);
            }
        }
    }

}

