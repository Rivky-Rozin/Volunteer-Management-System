namespace BlImplementation;
using System;
using System.Collections.Generic;
using BlApi;
using BO;
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


    public void CancelCallTreatment(int requesterId, int assignmentId)
    {
        throw new NotImplementedException();
    }

    public void CompleteCallTreatment(int volunteerId, int assignmentId)
    {
        throw new NotImplementedException();
    }

    public void DeleteCall(int callId)
    {
        throw new NotImplementedException();
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
