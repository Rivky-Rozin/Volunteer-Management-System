namespace BlImplementation;
using System;
using System.Collections.Generic;
using BlApi;
using DalApi;
using DO;
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

    //עובד
    public void CancelCallTreatment(int requesterId, int assignmentId)
    {
        DO.Assignment assignment;
        try
        {
            assignment = _dal.Assignment.Read(assignmentId);
        }
        catch (Exception ex)
        {
            //todo
            throw new BO.EntityNotFoundException("ההקצאה לא נמצאה", ex);
        }
        DO.Volunteer doVolunteer = _dal.Volunteer.Read(requesterId); 

        // בדיקת הרשאה: מנהל או המתנדב הרשום
        bool isAdmin = doVolunteer.Role == DO.VolunteerRole.Manager; // נניח שיש שיטה כזו
        //todo
        if (!isAdmin && assignment.VolunteerId != requesterId)
            throw new BO.Exceptions.AuthorizationException("אין הרשאה לבטל טיפול");

        // בדיקה שהטיפול עדיין לא הסתיים
        if (assignment.EndTreatment != null)
            //todo
            throw new BO.Exceptions.InvalidOperationException("לא ניתן לבטל טיפול שכבר הסתיים");
        DO.TreatmentType TreatmentType;
        if (isAdmin)
        {
            TreatmentType = DO.TreatmentType.ManagerCancelled;
        }
        else
        {
            TreatmentType = DO.TreatmentType.UserCancelled;
        }
        // עדכון סטטוס וזמן
        DO.Assignment newAssignment = new DO.Assignment
        {
            Id = assignment.Id,
            CallId = assignment.CallId,
            VolunteerId = assignment.VolunteerId,
            StartTreatment = assignment.StartTreatment,
            EndTreatment = ClockManager.Now,
            TreatmentType = TreatmentType,
        };
        

        try
        {
            _dal.Assignment.Update(newAssignment);
        }
        catch (Exception ex)
        {
            //todo
            throw new BO.Exceptions.EntityNotFoundException("ההקצאה לא נמצאה בעדכון", ex);
        }
    }

    //todo
    public void DeleteCall(int callId)
    {
        try
        {
            // שלב 1: שליפת הקריאה משכבת הנתונים
            var call = dal.Call.Read(callId)
                       ?? throw new BO.BlEntityNotFoundException("Call", callId);

            // שלב 2: בדיקה האם הקריאה בסטטוס פתוח
            if (call.Status != BO.CallStatus.Open)
                throw new BO.BlCannotDeleteException($"Cannot delete call #{callId} because it is not in 'Open' status.");

            // שלב 3: בדיקה אם הקריאה הוקצתה למתנדב כלשהו בעבר
            var assignments = dal.Assignment.ReadAll()
                                 .Where(a => a.CallId == callId);

            if (assignments.Any())
                throw new BO.BlCannotDeleteException($"Cannot delete call #{callId} because it has already been assigned to a volunteer.");

            // שלב 4: אם עברה את כל הבדיקות - ביצוע מחיקה
            dal.Call.Delete(callId);
        }
        catch (DO.EntityNotFoundException ex)
        {
            // שלב 5: אם הקריאה לא קיימת בשכבת הנתונים – זרוק חריגה מתאימה לשכבת התצוגה
            throw new BO.BlEntityNotFoundException("Call", callId, ex);
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
            throw new DO.EntityNotFoundException("קריאה לא נמצאה", ex);
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

            // Conversion from DO to BO using LINQ  
            var query = from call in calls
                        let boCall = CallManager.ConvertToCall(call)
                        select boCall;
            var closedCalls = from call in query
                              where call.Status == BO.CallStatus.Closed // Updated namespace from DO to BO  
                              //????????
                              && call.AssAssignments == volunteerId //todo : check if this is correct ואם הID של הקראיה באמת כמו המתנדב  
                              let boCall = CallManager.ConvertToClosedCallInList(call) // Fix: Convert BO.CallInList to DO.Call before passing to ConvertToClosedCallInList  
                              where callTypeFilter == null || boCall.CallType == callTypeFilter
                              select boCall;

            if (sortField != null)
            {
                closedCalls = sortField switch
                {
                    BO.CallField.RequesterName => closedCalls.OrderBy(c => c.RequesterName),
                    BO.CallField.Status => closedCalls.OrderBy(c => c.Status),
                    BO.CallField.StartTime => closedCalls.OrderBy(c => c.StartTime),
                    _ => closedCalls
                };
            }

            return closedCalls;
        }
        catch (Exception ex)
        {
            //todo: להוסיף את זה לקובץ של השגיאות
            throw new BO.GeneralException("שגיאה בקבלת קריאות סגורות למתנדב", ex);
        }
    }

    //עובד
    public void UpdateCall(BO.Call call)
    {
        //todo
        if (call == null)
            throw new BO.ArgumentNullException(nameof(call), "אובייקט הקריאה שהתקבל הוא null.");

        // בדיקת מזהה
        if ( call.Id<100000000|| call.Id>999999999)
            throw new ArgumentException("מזהה הקריאה חייב להיות מספר חיובי.");

        // בדיקת סוג הקריאה
        if (!Enum.IsDefined(typeof(BO.CallType), call.CallType))
            throw new ArgumentException("סוג הקריאה אינו חוקי.");

        // תיאור - רשות, אך אם קיים, נבדוק אם לא ריק מדי
        if (call.Description != null && call.Description.Trim().Length < 2)
            throw new ArgumentException("אם סופק תיאור, עליו להכיל לפחות 2 תווים.");

        // כתובת
        if (string.IsNullOrWhiteSpace(call.Address))
            throw new ArgumentException("כתובת אינה יכולה להיות ריקה.");

        // זמן יצירת הקריאה (לא נבדוק אם בעבר, כי זה עדכון)
        // זמן סיום
        if (call.MaxFinishTime != null && call.MaxFinishTime <= call.CreationTime)
            throw new ArgumentException("זמן הסיום המקסימלי חייב להיות אחרי זמן היצירה.");

        // סטטוס
        if (!Enum.IsDefined(typeof(BO.CallStatus), call.Status))
            throw new ArgumentException("סטטוס הקריאה אינו חוקי.");

        // קבלת קואורדינטות מהכתובת – מעדכן ישירות ל־call
        try
        {
            var (lat, lon) = Tools.GetCoordinatesFromAddress(call.Address);
            call.Latitude = lat;
            call.Longitude = lon;
        }
        catch (Exception ex)
        {
            throw new FormatException("כתובת שגויה או לא קיימת – לא ניתן לאתר קואורדינטות.", ex);
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
        }
        catch (DO.EntityNotFoundException ex)
        {
            throw new BO.CallNotFoundException($"קריאה עם מזהה {call.Id} לא נמצאה במערכת.", ex);
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
