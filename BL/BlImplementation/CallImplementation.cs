namespace BlImplementation;
using System;
using System.Collections.Generic;
using Helpers;
internal class CallImplementation : BlApi.ICall
{
    #region Stage 5
    public void AddObserver(Action listObserver) =>
    CallManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
CallManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
CallManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
CallManager.Observers.RemoveObserver(id, observer); //stage 5
    #endregion Stage 5
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    //עובד
    public void AddCall(BO.Call call)
    {
        try
        {
            // בדיקה: זמן סיום חייב להיות אחרי זמן פתיחה (אם הוגדר)      
            if (call.MaxFinishTime != null && call.MaxFinishTime <= call.CreationTime)
                throw new BO.BlInvalidActionException("Finish time must be after creation time");

            // עדכון זמן פתיחה לזמן הנוכחי של המערכת      
            var openTime = AdminManager.Now;
            call.CreationTime = openTime;

            // המרה ל-DO      
            DO.Call doCall = CallManager.ConvertToDO(call);

            // הוספה ל-DAL      
            _dal.Call.Create(doCall); 
            CallManager.Observers.NotifyListUpdated(); //stage 5
        }
        catch (Exception ex)
        {
            throw new BO.BlErrorAddingObject("Error adding call", ex);
        }
    }

    //עובד
    public void CompleteCallTreatment(int volunteerId, int callId)
    {
        // חיפוש ההקצאה הפתוחה לפי callId
        DO.Assignment? assignment = _dal.Assignment.ReadAll()
            .FirstOrDefault(a => a.CallId == callId && a.VolunteerId == volunteerId);

        if (assignment == null)
            throw new BO.BlDoesNotExistException("ההקצאה עבור הקריאה לא נמצאה או שכבר טופלה");

        // יצירת אובייקט חדש עם עדכון סטטוס וסיום טיפול
        DO.Assignment updatedAssignment = new()
        {
            Id = assignment.Id,
            CallId = assignment.CallId,
            VolunteerId = assignment.VolunteerId,
            StartTreatment = assignment.StartTreatment,
            TreatmentType = DO.TreatmentType.Treated,
            EndTreatment = AdminManager.Now
        };

        try
        {
            _dal.Assignment.Update(updatedAssignment);
            CallManager.Observers.NotifyListUpdated(); // stage 5
            CallManager.Observers.NotifyItemUpdated(updatedAssignment.Id); // stage 5
        }
        catch
        {
            throw new BO.BlDoesNotExistException("ההקצאה לא נמצאה בעדכון");
        }
    }


    //עובד
    public void CancelCallTreatment(int requesterId, int callId)
    {
        // שליפת ההקצאה הפעילה של הקריאה הזו
        DO.Assignment? assignment;
        try
        {
            assignment = _dal.Assignment
                .ReadAll()
                .FirstOrDefault(a => a.CallId == callId && a.VolunteerId == requesterId);
        }
        catch (Exception ex)
        {
            throw new BO.BlDoesNotExistException("שגיאה בגישה להקצאות", ex);
        }

        if (assignment == null)
            throw new BO.BlDoesNotExistException("לא נמצאה הקצאה פעילה לקריאה הזו");
        
        // שליפת פרטי המתנדב
        DO.Volunteer? doVolunteer = _dal.Volunteer.Read(requesterId);
        if (doVolunteer == null)
            throw new BO.BlDoesNotExistException("המתנדב לא קיים");

        // בדיקת הרשאה: מנהל או המתנדב עצמו
        bool isAdmin = doVolunteer.Role == DO.VolunteerRole.Manager;
        if (!isAdmin && assignment.VolunteerId != requesterId)
            throw new BO.BlAuthorizationException("אין הרשאה לבטל את ההקצאה");

        // בדיקה שהטיפול עדיין לא הסתיים (ליתר ביטחון – גם אחרי הסינון למעלה)
        if (assignment.EndTreatment != null)
            throw new BO.BlInvalidOperationException("אי אפשר לבטל טיפול שכבר הסתיים");

        // קביעת סוג ביטול
        DO.TreatmentType treatmentType = isAdmin
            ? DO.TreatmentType.ManagerCancelled
            : DO.TreatmentType.UserCancelled;

        // יצירת אובייקט חדש עם זמן סיום וסוג טיפול מעודכן
        DO.Assignment updatedAssignment = new DO.Assignment
        {
            Id = assignment.Id,
            CallId = assignment.CallId,
            VolunteerId = assignment.VolunteerId,
            StartTreatment = assignment.StartTreatment,
            EndTreatment = AdminManager.Now,
            TreatmentType = treatmentType
        };

        // עדכון ב־DAL
        try
        {
            _dal.Assignment.Update(updatedAssignment);
            CallManager.Observers.NotifyListUpdated();         // stage 5
            CallManager.Observers.NotifyItemUpdated(updatedAssignment.Id); // stage 5
        }
        catch (Exception ex)
        {
            throw new BO.BlDoesNotExistException("שגיאה בעדכון ההקצאה", ex);
        }
    }


    //עובד
    public void DeleteCall(int callId)
    {
        try
        {
            // שלב 1: שליפת הקריאה משכבת הנתונים
            DO.Call call = _dal.Call.Read(callId)
                       ?? throw new BO.BlDoesNotExistException("ההקצאה לא נמצאה בעדכון");

            // שלב 2: בדיקה האם הקריאה בסטטוס פתוח
            if (CallManager.GetCallStatus(call.Id) != BO.CallStatus.Open)
                throw new BO.BlCannotDeleteException($"Cannot delete call #{callId} because it is not in 'Open' status.");

            // שלב 3: בדיקה אם הקריאה הוקצתה למתנדב כלשהו בעבר
            var assignments = _dal.Assignment.ReadAll(a => a.CallId == callId);

            if (assignments.Any())
                throw new BO.BlCannotDeleteException($"Cannot delete call #{callId} because it has already been assigned to a volunteer.");

            // שלב 4: אם עברה את כל הבדיקות - ביצוע מחיקה
            _dal.Call.Delete(callId);
            CallManager.Observers.NotifyListUpdated(); //stage 5
        }
        catch (Exception ex)
        {
           // שלב 5: אם הקריאה לא קיימת בשכבת הנתונים – זרוק חריגה מתאימה לשכבת התצוגה
            throw new BO.BlDoesNotExistException("Call", ex);
        }
    }

    //עובד
    public BO.Call GetCallDetails(int callId)
    {
        try
        {
            //להוסיף שגיאה מתאימה
            DO.Call? doCall = _dal.Call.Read(callId) ?? throw new BO.BlDoesNotExistException($"Call with ID {callId} not found.");

            BO.Call boCall = CallManager.ConvertToBO(doCall);

            return boCall;
        }
        catch (Exception ex)
        {
            throw new BO.BlDoesNotExistException("קריאה לא נמצאה", ex);
        }
    }

    //עובד
    public IEnumerable<BO.CallInList> GetCallList(BO.CallInListField? filterField, object? filterValue, BO.CallInListField? sortField)
    {
        try
        {
            var calls = _dal.Call.ReadAll();

            // Conversion from DO to BO using LINQ  
            var query = from call in calls
                        let boCall = CallManager.ConvertToCallInList(call)
                        where filterField == null || filterValue == null || CallManager.MatchesFilter(boCall, filterField.Value, filterValue)
                        select boCall;

            // Sorting based on the selected field  
            if (sortField != null)
            {
                query = sortField switch
                {
                    BO.CallInListField.Id => query.OrderBy(c => c.Id),
                    BO.CallInListField.CallId => query.OrderBy(c => c.CallId),
                    BO.CallInListField.CallType => query.OrderBy(c => c.CallType),
                    BO.CallInListField.OpenTime => query.OrderBy(c => c.OpenTime),
                    BO.CallInListField.TimeUntilAssigning => query.OrderBy(c => c.TimeUntilAssigning),
                    BO.CallInListField.LastVolunteerName => query.OrderBy(c => c.LastVolunteerName),
                    BO.CallInListField.totalTreatmentTime => query.OrderBy(c => c.totalTreatmentTime),
                    BO.CallInListField.Status => query.OrderBy(c => c.Status),
                    BO.CallInListField.NumberOfAssignments => query.OrderBy(c => c.NumberOfAssignments),
                    _ => query
                };
            }

            return query;
        }
        catch (Exception ex)
        {
            // להוסיף את זה לקובץ של השגיאות  
            throw new BO.BlDoesNotExistException("שגיאה באחזור רשימת הקריאות", ex);
        }
    }

    //עובד
    public int[] GetCallStatusCounts()
    {
        try
        {
            var calls = GetCallList(null, null, null);

            var grouped = calls
                .GroupBy(c => c.Status)
                .ToDictionary(g => g.Key, g => g.Count());

            var allStatuses = Enum.GetValues(typeof(BO.CallStatus)).Cast<BO.CallStatus>();

            return allStatuses
                .Select(status => grouped.TryGetValue(status, out int count) ? count : 0)
                .ToArray();
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralException("שגיאה בקבלת סטטיסטיקת קריאות", ex);
        }
    }


    //עובד
    public IEnumerable<BO.ClosedCallInList> GetClosedCallsOfVolunteer(int volunteerId, BO.CallType? callTypeFilter, BO.ClosedCallInListEnum? sortField)
    {
        try
        {
            var calls = _dal.Call.ReadAll();

            // Conversion from DO to BO using LINQ  
            var query = from call in calls
                        let boCall = CallManager.ConvertToBO(call)
                        select boCall;
            var closedCalls = from call in query
                              where call.Status == BO.CallStatus.Closed // Updated namespace from DO to BO  
                                                                        //????????
                              && call.Assignments.Any(a => a.VolunteerId == volunteerId)

                              let boCall = CallManager.ConvertToClosedCallInList(CallManager.ConvertToDO(call)) // Fix: Convert BO.CallInList to DO.Call before passing to ConvertToClosedCallInList  
                              where callTypeFilter == null || boCall.CallType == callTypeFilter
                              select boCall;

            if (sortField != null)
            {
                closedCalls = sortField switch
                {
                    BO.ClosedCallInListEnum.Id => closedCalls.OrderBy(c => c.Id),
                    BO.ClosedCallInListEnum.CallType => closedCalls.OrderBy(c => c.CallType),
                    BO.ClosedCallInListEnum.FullAddress => closedCalls.OrderBy(c => c.FullAddress),
                    BO.ClosedCallInListEnum.OpenTime => closedCalls.OrderBy(c => c.OpenTime),
                    BO.ClosedCallInListEnum.EntryToTreatmentTime => closedCalls.OrderBy(c => c.EntryToTreatmentTime),
                    BO.ClosedCallInListEnum.ActualTreatmentEndTime => closedCalls.OrderBy(c => c.ActualTreatmentEndTime),
                    BO.ClosedCallInListEnum.TreatmentEndType => closedCalls.OrderBy(c => c.TreatmentEndType),
                    _ => closedCalls
                };
            }

            return closedCalls;
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralException("שגיאה בקבלת קריאות סגורות למתנדב", ex);
        }
    }
    //עובד
    public void UpdateCall(BO.Call call)
    {
        if (call == null)
            throw new BO.BlObjectCanNotBeNullException("אובייקט הקריאה שהתקבל הוא null.");

        // בדיקת מזהה
        if (call.Id < 0)
            throw new BO.BlArgumentException("מזהה הקריאה חייב להיות מספר חיובי.");

        // בדיקת סוג הקריאה
        if (!Enum.IsDefined(typeof(BO.CallType), call.CallType))
            throw new BO.BlArgumentException("סוג הקריאה אינו חוקי.");

        // תיאור - רשות, אך אם קיים, נבדוק אם לא ריק מדי
        if (call.Description != null && call.Description.Trim().Length < 2)
            throw new BO.BlArgumentException("אם סופק תיאור, עליו להכיל לפחות 2 תווים.");

        // כתובת
        if (string.IsNullOrWhiteSpace(call.Address))
            throw new BO.BlArgumentException("כתובת אינה יכולה להיות ריקה.");

        // זמן יצירת הקריאה (לא נבדוק אם בעבר, כי זה עדכון)
        // זמן סיום
        if (call.MaxFinishTime != null && call.MaxFinishTime <= call.CreationTime)
            throw new BO.BlArgumentException("זמן הסיום המקסימלי חייב להיות אחרי זמן היצירה.");

        // סטטוס
        if (!Enum.IsDefined(typeof(BO.CallStatus), call.Status))
            throw new BO.BlArgumentException("סטטוס הקריאה אינו חוקי.");

        // קבלת קואורדינטות מהכתובת – מעדכן ישירות ל־call
        try
        {
            var (lat, lon) = Tools.GetCoordinatesFromAddress(call.Address);
            call.Latitude = lat;
            call.Longitude = lon;
        }
        catch
        {
            throw new BO.BlGeneralException("כתובת שגויה או לא קיימת – לא ניתן לאתר קואורדינטות.");
        }
        
        // המרה ל-DO.Call
        DO.Call callEntity = new DO.Call
        {
            Id = call.Id,
            CallType = (DO.CallType)call.CallType,
            Description = call.Description,
            FullAddress = call.Address,
            Latitude = call.Latitude,
            Longitude = call.Longitude,
            OpenTime = call.CreationTime,
            MaxCallTime = call.MaxFinishTime
        };

        // ניסיון לעדכן במאגר הנתונים
        try
        {
            _dal.Call.Update(callEntity);
            CallManager.Observers.NotifyListUpdated(); //stage 5
            CallManager.Observers.NotifyItemUpdated(callEntity.Id); //stage 5
        }
        catch (Exception ex)
        {
            throw new BO.BlDoesNotExistException($"קריאה עם מזהה {call.Id} לא נמצאה במערכת.", ex);
        }
    }

    //עובד
    public IEnumerable<BO.OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, BO.CallType? callTypeFilter, BO.OpenCallInListEnum? sortField)
    {
        //
        // שליפת נתוני המתנדב
        var volunteer = _dal.Volunteer.Read(volunteerId)
            ?? throw new BO.BlDoesNotExistException("Volunteer not found");

        // שליפת הקריאות בסטטוס פתוחה או פתוחה בסיכון
        var openStatuses = new[] { BO.CallStatus.Open, BO.CallStatus.OpenAtRisk };
        var openCalls = _dal.Call.ReadAll(c => openStatuses.Contains(CallManager.GetCallStatus(c.Id))).ToList();
        var allCalls = _dal.Call.ReadAll( ).ToList().Select(s=>s.MaxCallTime);

        // סינון לפי סוג הקריאה אם צריך
        if (callTypeFilter != null)
            openCalls = openCalls.Where(c => (BO.CallType)c.CallType == callTypeFilter).ToList();

        // המרה לרשימת BO.OpenCallInList עם חישוב מרחק
        var result = openCalls.Select(call =>
        {
            double distance = Tools.GetDistance(volunteer, call);

            //?
            //var assignments = _dal.Assignment.ReadAll(a => a.CallId == call.Id).ToList();

            return new BO.OpenCallInList
            {
                Id = call.Id,
                CallType = (BO.CallType)call.CallType,
                Description = call.Description,
                FullAddress = call.FullAddress,
                OpenTime = call.OpenTime,
                MaxEndTime = call.MaxCallTime,
                DistanceFromVolunteer = distance
            };
        });

        // מיון לפי השדה שנבחר או לפי CallId כברירת מחדל
        result = sortField switch
        {
            BO.OpenCallInListEnum.Id => result.OrderBy(r => r.Id),
            BO.OpenCallInListEnum.CallType => result.OrderBy(r => r.CallType),
            BO.OpenCallInListEnum.Description => result.OrderBy(r => r.Description),
            BO.OpenCallInListEnum.FullAddress => result.OrderBy(r => r.FullAddress),
            BO.OpenCallInListEnum.OpenTime => result.OrderBy(r => r.OpenTime),
            BO.OpenCallInListEnum.MaxEndTime => result.OrderBy(r => r.MaxEndTime),
            BO.OpenCallInListEnum.DistanceFromVolunteer => result.OrderBy(r => r.DistanceFromVolunteer),
            _ => result.OrderBy(r => r.Id) // ברירת מחדל
        };

        return result;
    }

    //עובד
    public void SelectCallForTreatment(int volunteerId, int callId)
    {
        DO.Call call;
        try
        {
            call = _dal.Call.Read(callId);
        }
        catch (Exception ex)
        {

            throw new BO.BlDoesNotExistException("The call does not exist", ex);
        }
        // בדיקה אם הקריאה כבר טופלה
        if (CallManager.GetCallStatus(call.Id) == BO.CallStatus.Closed)
            throw new BO.BlInvalidOperationException("הקריאה כבר טופלה.");

        // בדיקה אם הקריאה פגה תוקף
        if (call.MaxCallTime <= AdminManager.Now)
            
            throw new BO.BlExpired("Call expired");

        // בדיקה אם יש כבר הקצאה פתוחה לקריאה זו
        var existingAssignments = _dal.Assignment.ReadAll(a => a.CallId == callId && CallManager.GetCallStatus(a.Id) == BO.CallStatus.Open);
        if (existingAssignments.Any())
          
            throw new BO.BlAlreadyInTreatment("The call is already under treatment");

        // יצירת הקצאה חדשה
        var assignment = new DO.Assignment
        {
            CallId = callId,
            VolunteerId = volunteerId,
            StartTreatment = AdminManager.Now,
            EndTreatment = null,
            TreatmentType = null
        };
        try
        {
            _dal.Assignment.Create(assignment);
            CallManager.Observers.NotifyListUpdated(); //stage 5
        }
        catch (Exception ex)
        {
           
            throw new BO.BlFailedToCreate("Failed to create assignment", ex);
        }
    }
}