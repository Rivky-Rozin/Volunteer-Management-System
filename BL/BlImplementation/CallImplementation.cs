namespace BlImplementation;
using System;
using System.Collections.Generic;
using BlApi;
using Helpers;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void AddCall(BO.Call call)
    {
        throw new NotImplementedException();
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
            // Correcting the method call to use the appropriate method from ICrud<T> interface  
            DO.Call? doCall = _dal.Call.Read(callId);

            if (doCall == null)
            {
                throw new DO.EntityNotFoundException($"Call with ID {callId} not found.");
            }

            // Assuming CallConverter.ConvertToBO is a valid helper method for conversion
            // להכין את הפונקציות של ההמרה
            BO.Call boCall = CallManager.ConvertToBO(doCall);

            return boCall;
        }
        catch (DO.EntityNotFoundException ex)
        {
            throw new DO.EntityNotFoundException("קריאה לא נמצאה", ex);
        }
    }


    public IEnumerable<BO.CallInList> GetCallList(Enum? filterField, object? filterValue, Enum? sortField)
    {
        throw new NotImplementedException();
    }

    public int[] GetCallStatusCounts()
    {
        try
        {
            // Ensure the DO.Call type has a Status property or equivalent  
            var calls = _dal.Call.ReadAll();

            // Group by the Status property of DO.Call  
            var counts = calls
                .GroupBy(c => c.Status)
                .OrderBy(g => g.Key) // Sort by Status  
                .Select(g => g.Count())
                .ToArray();

            return counts;
        }
        catch (Exception ex)
        {
            throw new BO.GeneralException("שגיאה בקבלת סטטיסטיקת קריאות", ex);
        }
    }


    public IEnumerable<BO.ClosedCallInList> GetClosedCallsOfVolunteer(int volunteerId, BO.CallType? callTypeFilter, BO.ClosedCallInList? sortField)
    {
        throw new NotImplementedException();
    }

    public IEnumerable< BO.OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, BO.CallType? callTypeFilter, BO.OpenCallInList? sortField)
    {
        throw new NotImplementedException();
    }

    public void SelectCallForTreatment(int volunteerId, int callId)
    {
        throw new NotImplementedException();
    }

    public void UpdateCall(BO.Call call)
    {
        throw new NotImplementedException();
    }
}
