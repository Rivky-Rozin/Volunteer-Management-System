namespace BlImplementation;
using System;
using System.Collections.Generic;
using BlApi;
using DalApi;
using Helpers;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
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
        catch (DO.EntityAlreadyExistsException ex)
        {
            //  todo: להוסיף שגיאה מתאימה
            throw new BO.EntityAlreadyExistsException("קריאה עם מזהה זה כבר קיימת", ex);
        }
        catch (Exception ex)
        {
            // todo: להוסיף שגיאה מתאימה
            throw new BO.GeneralException("שגיאה בהוספת קריאה", ex);
        }
    }

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
        call.
        _dal.Call.Update(call);
        call.TreatmentType = DO.Enums.TreatmentType.Treated;
        assignment.EndTreatmentTime = DateTime.Now;

        try
        {
            dal.Assignment.Update(assignment);
        }
        catch (KeyNotFoundException)
        {
            throw new BO.Exceptions.EntityNotFoundException("ההקצאה לא נמצאה בעדכון");
        }
    }

    public void CancelCallTreatment(int requesterId, int assignmentId)
    {
        DO.Assignment assignment;
        try
        {
            assignment = _dal.Assignment.GetById(assignmentId);
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
            dal.Assignment.Update(assignment);
        }
        catch (KeyNotFoundException)
        {
            throw new BO.Exceptions.EntityNotFoundException("ההקצאה לא נמצאה בעדכון");
        }
    }

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

    public BO.Call GetCallDetails(int callId)
    {
        try
        {
            //todo
            DO.Call? doCall = _dal.Call.Read(callId) ?? throw new DO.EntityNotFoundException($"Call with ID {callId} not found.");

            BO.Call boCall = CallManager.ConvertToBO(doCall);

            return boCall;
        }
        catch (DO.EntityNotFoundException ex)
        {
            //todo להוסיף את זה לקובץ של השגיאות
            throw new BO.EntityNotFoundException("קריאה לא נמצאה", ex);
        }
    }


    public IEnumerable<BO.CallInList> GetCallList(CallInListField? filterField, object? filterValue, CallInListField? sortField)
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
                    CallInListField.Id => query.OrderBy(c => c.Id),
                    CallInListField.CallId => query.OrderBy(c => c.CallId),
                    CallInListField.CallType => query.OrderBy(c => c.CallType),
                    CallInListField.OpenTime => query.OrderBy(c => c.OpenTime),
                    CallInListField.TimeUntilAssigning => query.OrderBy(c => c.TimeUntilAssigning),
                    CallInListField.LastVolunteerName => query.OrderBy(c => c.LastVolunteerName),
                    CallInListField.totalTreatmentTime => query.OrderBy(c => c.totalTreatmentTime),
                    CallInListField.Status => query.OrderBy(c => c.Status),
                    CallInListField.NumberOfAssignments => query.OrderBy(c => c.NumberOfAssignments),
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

    public IEnumerable<ClosedCallInList> GetClosedCallsOfVolunteer(int volunteerId, CallType? callTypeFilter, CallField? sortField)
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
}
