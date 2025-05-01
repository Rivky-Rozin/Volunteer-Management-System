namespace BlImplementation;
using System;
using System.Collections.Generic;
using BlApi;
using BO;
using DalApi;
using Helpers;
//ללללל
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
            throw new DO.EntityNotFoundException("קריאה לא נמצאה", ex);
        }
    }


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



    // Fix for CS0234: Ensure the correct namespace is used for CallStatus.  
    // Based on the context, it seems CallStatus is part of BO, not DO.  
    // Update the namespace reference accordingly.  

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

    public IEnumerable<BO.OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, BO.CallType? callTypeFilter, BO.CallField? sortField)
    {
        try
        {
            var openCalls = from call in GetCallList(null, null, null) where call.Status == BO.CallStatus.Open let boCall = CallManager.ConvertToOpenCallInList(call) where callTypeFilter == null || boCall.CallType == callTypeFilter select boCall;

            if (sortField != null)
            {
                openCalls = sortField switch
                {
                    BO.CallField.RequesterName => openCalls.OrderBy(c => c.RequesterName),
                    BO.CallField.Status => openCalls.OrderBy(c => c.Status),
                    BO.CallField.StartTime => openCalls.OrderBy(c => c.OpenTime),
                    _ => openCalls
                };
            }

            return openCalls;
        }
        catch (Exception ex)
        {
            //todo: להוסיף את זה לקובץ של השגיאות
            throw new BO.GeneralException("שגיאה בקבלת קריאות פתוחות למתנדב", ex);
        }
    }

    public IEnumerable<OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, CallType? callTypeFilter, OpenCallInList? sortField)
    {
        throw new NotImplementedException();
    }

    public void SelectCallForTreatment(int volunteerId, int callId)
    {
        try
        {
            var call = _dal.Call.Read(callId) ?? throw new BO.EntityNotFoundException("הקריאה לא נמצאה");
            if (call.Status != DO.CallStatus.Open)
                throw new BO.InvalidActionException("לא ניתן לבחור קריאה שכבר אינה פתוחה.");

            call.Status = DO.CallStatus.InProgress;
            call.VolunteerId = volunteerId;
            call.SelectTime = ClockManager.Now;

            _dal.Call.Update(call);
        }
        catch (DO.EntityNotFoundException ex)
        {
            throw new BO.EntityNotFoundException("הקריאה לא נמצאה", ex);
        }
        catch (Exception ex)
        {
            throw new BO.GeneralException("שגיאה בבחירת קריאה לטיפול", ex);
        }
    }





public void UpdateCall(BO.Call call)
    {
        try
        {
            var existing = _dal.Call.Read(call.Id) ?? throw new BO.EntityNotFoundException("הקריאה לא נמצאה");


            var updatedCall = CallManager.ConvertToDO(call);
            _dal.Call.Update(updatedCall);
        }
        catch (DO.EntityNotFoundException ex)
        {//todo: להוסיף את זה לקובץ של השגיאות
            throw new BO.EntityNotFoundException("לא ניתן לעדכן קריאה שלא קיימת", ex);
        }
        catch (Exception ex)
        {//todo: להוסיף את זה לקובץ של השגיאות
            throw new BO.GeneralException("שגיאה בעדכון הקריאה", ex);
        }
    }
}
