namespace BlImplementation;
using System;
using System.Collections.Generic;
using BlApi;
using DalApi;
using Helpers;
//ללללל
internal class CallImplementation : BlApi.ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    //עובד
    public void AddCall(BO.Call call)
    {
        try
        {
            // בדיקה: זמן סיום חייב להיות אחרי זמן פתיחה (אם הוגדר)      
            if (call.MaxFinishTime != null && call.MaxFinishTime <= call.CreationTime)
                // todo : להוסיף שגיאה מתאימה    
                throw new BO.InvalidActionException("זמן הסיום חייב להיות אחרי זמן הפתיחה");

            // עדכון זמן פתיחה לזמן הנוכחי של המערכת      
            var openTime = ClockManager.Now;
            call.CreationTime = openTime;

            // המרה ל-DO      
            DO.Call doCall = CallManager.ConvertToDO(call);

            // הוספה ל-DAL      
            _dal.Call.Create(doCall); // FIX: Changed 'Add' to 'Create' to match the ICrud<T> interface  
        }
        catch (Exception ex)
        {
            // todo: להוסיף שגיאה מתאימה
            throw new BO.GeneralException("שגיאה בהוספת קריאה", ex);
        }
    }

    //שאלה למורה
    public void CompleteCallTreatment(int volunteerId, int assignmentId)
    {
        DO.Assignment assignment;
        try
        {
            assignment = _dal.Assignment.Read(assignmentId);
        }
        catch (KeyNotFoundException)
        {
            //todo
            throw new BO.Exceptions.EntityNotFoundException("ההקצאה לא נמצאה");
        }

        // בדיקת הרשאה
        if (assignment.VolunteerId != volunteerId)
            //todo
            throw new BO.Exceptions.AuthorizationException("אין הרשאה לסיים טיפול - המתנדב אינו רשום על ההקצאה");

        // בדיקה שההקצאה פתוחה (כלומר לא טופלה, לא בוטלה ולא פג תוקף)
        if (assignment.EndTreatment != null)
            //todo
            throw new BO.Exceptions.InvalidOperationException("לא ניתן לסיים טיפול - ההקצאה כבר טופלה או בוטלה");

        // עדכון פרטי סיום
        DO.Call call = _dal.Call.Read(assignment.VolunteerId);
        call.TreatmentType = DO.Enums.TreatmentType.Treated;
        _dal.Call.Update(call);
        //todo איך מסדרים את זה??
        assignment.EndTreatment = DateTime.Now;

        try
        {
            _dal.Assignment.Update(assignment);
        }
        catch (Exception ex)
        {
            throw new BO.Exceptions.EntityNotFoundException("ההקצאה לא נמצאה בעדכון");
        }
    }

    //todo
    public void CancelCallTreatment(int requesterId, int assignmentId)
    {
        DO.Assignment assignment;
        try
        {
            assignment = _dal.Assignment.Read(assignmentId);
        }
        catch (KeyNotFoundException)
        {
            //todo
            throw new BO.EntityNotFoundException("ההקצאה לא נמצאה");
        }

        // בדיקת הרשאה: מנהל או המתנדב הרשום
        bool isAdmin = _dal.User.IsAdmin(requesterId); // נניח שיש שיטה כזו
        if (!isAdmin && assignment.VolunteerId != requesterId)
            throw new BO.Exceptions.AuthorizationException("אין הרשאה לבטל טיפול");

        // בדיקה שהטיפול עדיין לא הסתיים
        if (assignment.EndTreatmentTime != null)
            throw new BO.Exceptions.InvalidOperationException("לא ניתן לבטל טיפול שכבר הסתיים");

        // עדכון סטטוס וזמן
        assignment.EndTreatmentTime = DateTime.Now;
        assignment.TreatmentStatus = assignment.VolunteerId == requesterId
            ? DO.Enums.TreatmentStatus.SelfCancelled
            : DO.Enums.TreatmentStatus.ManagerCancelled;

        try
        {
            _dal.Assignment.Update(assignment);
        }
        catch (KeyNotFoundException)
        {
            throw new BO.Exceptions.EntityNotFoundException("ההקצאה לא נמצאה בעדכון");
        }
    }

    //todo
    public void DeleteCall(int callId)
    {
        DO.Call call;
        try
        {
            call = dal.Call.GetById(callId);
        }
        catch (KeyNotFoundException)
        {
            throw new BO.Exceptions.EntityNotFoundException("הקריאה לא נמצאה");
        }

        // בדיקה אם מותר למחוק: הקריאה בסטטוס פתוח ולא הוקצתה
        if (call.Status != DO.Enums.CallStatus.Open || dal.Assignment.ExistsForCall(callId))
            throw new BO.Exceptions.InvalidOperationException("לא ניתן למחוק קריאה שטופלה או הוקצתה בעבר");

        try
        {
            dal.Call.Delete(callId);
        }
        catch (KeyNotFoundException)
        {
            throw new BO.Exceptions.EntityNotFoundException("הקריאה לא נמצאה בעת ניסיון המחיקה");
        }
    }

    //עובד
    public BO.Call GetCallDetails(int callId)
    {
        try
        {
            //todo
            //להוסיף שגיאה מתאימה
            DO.Call? doCall = _dal.Call.Read(callId) ?? throw new BO.EntityNotFoundException($"Call with ID {callId} not found.");

            BO.Call boCall = CallManager.ConvertToBO(doCall);

            return boCall;
        }
        catch (DO.EntityNotFoundException ex)
        {
            //todo להוסיף את זה לקובץ של השגיאות
            throw new BO.EntityNotFoundException("קריאה לא נמצאה", ex);
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
        catch (DO.EntityNotFoundException ex)
        {
            //todo להוסיף את זה לקובץ של השגיאות  
            throw new BO.EntityNotFoundException("שגיאה באחזור רשימת הקריאות", ex);
        }
    }

    //עובד
    public int[] GetCallStatusCounts()
    {
        try
        {
            var calls = GetCallList(null, null, null);

            var counts = calls
              .GroupBy(c => c.Status)        // קיבוץ לפי הסטטוס של הקריאה
              .OrderBy(g => g.Key)           // מיון קבוצות הסטטוס לפי הערך שלהן (כלומר: לפי סדר enum)
              .Select(g => g.Count())        // הפיכת כל קבוצה למספר: כמה קריאות יש בקבוצה הזו
              .ToArray();                    // המרה למערך רגיל (int[])

            return counts;
        }
        catch (Exception ex)
        {
            //todo להוסיף את זה לקובץ של השגיאות
            throw new BO.GeneralException("שגיאה בקבלת סטטיסטיקת קריאות", ex);
        }
    }

    //עובד
    public IEnumerable<BO.ClosedCallInList> GetClosedCallsOfVolunteer(int volunteerId, BO.CallType? callTypeFilter, BO.CallField? sortField)
    {
        try
        {
            var calls = _dal.Call.ReadAll();

            var closedCalls = from call in calls
                              let boCall = CallManager.ConvertToBO(call)
                              where boCall.Status == BO.CallStatus.Closed
                                    && boCall.Assignments != null
                                    && boCall.Assignments.Any(a => a.VolunteerId == volunteerId)
                              let closedCall = CallManager.ConvertToClosedCallInList(boCall)
                              where callTypeFilter == null || closedCall.CallType == callTypeFilter
                              select closedCall;

            // מיון – אם לא נבחר שדה, ממוינים לפי מספר קריאה  
            if (sortField == null)
            {
                closedCalls = closedCalls.OrderBy(c => c.Id);
            }
            else
            {
                closedCalls = sortField switch
                {
                    BO.CallField.FullAddress => closedCalls.OrderBy(c => c.FullAddress), // ממיינים לפי כתובת  
                    BO.CallField.TreatmentEndType => closedCalls.OrderBy(c => c.TreatmentEndType), // ממיינים לפי סוג סיום הטיפול  
                    BO.CallField.OpenTime => closedCalls.OrderBy(c => c.OpenTime), // ממיינים לפי זמן פתיחה  
                    BO.CallField.EntryToTreatmentTime => closedCalls.OrderBy(c => c.EntryToTreatmentTime), // ממיינים לפי זמן כניסה לטיפול  
                    BO.CallField.ActualTreatmentEndTime => closedCalls.OrderBy(c => c.ActualTreatmentEndTime), // ממיינים לפי זמן סיום טיפול בפועל  
                    BO.CallField.CallType => closedCalls.OrderBy(c => c.CallType), // ממיינים לפי סוג הקריאה  
                    BO.CallField.Id => closedCalls.OrderBy(c => c.Id), //לפי מספר קריאה 
                    _ => closedCalls // ברירת מחדל: לא ממיינים  
                };
            }

            return closedCalls;
        }
        catch (Exception ex)
        {
            //todo
            throw new BO.GeneralException("שגיאה בקבלת קריאות סגורות למתנדב", ex);
        }
    }

    //todo
    public void UpdateCall(BO.Call call)
    {
        // בדיקות פורמט
        if (call == null)
            throw new ArgumentNullException(nameof(call), "האובייקט שקיבלת לעדכון הוא null.");

        if (string.IsNullOrWhiteSpace(call.Address))
            throw new ArgumentException("כתובת הקריאה אינה יכולה להיות ריקה.");

        // בדיקות לוגיקה
        if (call.OpeningTime >= call.MaxEndTime)
            throw new InvalidOperationException("זמן הסיום חייב להיות אחרי זמן הפתיחה.");

        // קבלת קואורדינטות מהכתובת
        try
        {
            var (lat, lon) = CallManager.GetCoordinatesFromAddress(call.Address);
            call.Latitude = lat;
            call.Longitude = lon;
        }
        catch (Exception ex)
        {
            throw new FormatException("הכתובת שסופקה אינה תקינה או שלא ניתן לאתר אותה.", ex);
        }

        // יצירת אובייקט מסוג DO.Call
        DO.Call callEntity = new DO.Call
        {
            Id = call.Id,
            Address = call.Address,
            Latitude = call.Latitude,
            Longitude = call.Longitude,
            OpeningTime = call.OpeningTime,
            MaxEndTime = call.MaxEndTime,
            // הוסף כאן כל שדה נוסף שקיים במחלקת הנתונים
        };

        // ניסיון לעדכן את הקריאה במאגר
        try
        {
            dal.Call.Update(callEntity);
        }
        catch (DO.EntityNotFoundException ex)
        {
            throw new BO.CallNotFoundException($"לא נמצאה קריאה עם מזהה {call.Id}.", ex);
        }
    }

    //הבעיה שאין סטטוס בCall של DO
    public IEnumerable<BO.OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, BO.CallType? callTypeFilter, BO.OpenCallInListEnum? sortField)
    {
        //todo
        // שליפת נתוני המתנדב
        var volunteer = _dal.Volunteer.Read(volunteerId)
            ?? throw new BO.ArgumentException("Volunteer not found");

        // שליפת הקריאות בסטטוס פתוחה או פתוחה בסיכון
        var openStatuses = new[] { DO.CallStatus.Open, DO.CallStatus.OpenAtRisk };
        var openCalls = _dal.Call.ReadAll(c => openStatuses.Contains(c.Status)).ToList();

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

    //הבעיה שאין סטטוס בקריאה של DO
    public void SelectCallForTreatment(int volunteerId, int callId)
    {
        DO.Call call;
        try
        {
            call = _dal.Call.Read(callId);
        }
        catch (Exception ex)
        {
            throw new BO.CallDoesNotExist("The call does not exist", ex);
        }
        //todo כאן שוב צריך לבדוק את הסטטוס של הקריאה
        // בדיקה אם הקריאה כבר טופלה
        if (call.Status == DO.CallStatus.Closed)
            //todo
            throw new BO.InvalidOperationException("הקריאה כבר טופלה.");

        // בדיקה אם הקריאה פגה תוקף
        if (call.MaxCallTime <= Helpers.ClockManager.Now)
            //todo
            throw new BO.ExpiredCall("Call expired");

        // בדיקה אם יש כבר הקצאה פתוחה לקריאה זו
        var existingAssignments = _dal.Assignment.ReadAll(a => a.CallId == callId && a.Status == DO.CallStatus == Open);
        if (existingAssignments.Any())
            //todo
            throw new BO.CallAlreadyInTreatment("The call is already under treatment");

        // יצירת הקצאה חדשה
        var assignment = new DO.Assignment
        {
            CallId = callId,
            VolunteerId = volunteerId,
            StartTreatment = DateTime.Now,
            EndTreatment = null,
            TreatmentType = null
        };
        try
        {
            _dal.Assignment.Create(assignment);
        }
        catch (Exception ex)
        {
            //todo
            throw new BO.FailedToCreate("Failed to create assignment", ex);
        }
    }
}
