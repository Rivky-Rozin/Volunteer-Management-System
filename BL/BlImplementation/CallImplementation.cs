
namespace BlImplementation;

using System.Collections.Generic;
using BlApi;
using BO;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public void AddCall(Call call)
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

    public Call GetCallDetails(int callId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<CallInList> GetCallList(CallInList? filterField, object? filterValue, CallInList? sortField)
    {
        throw new NotImplementedException();
    }

    public int[] GetCallStatusCounts()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<ClosedCallInList> GetClosedCallsOfVolunteer(int volunteerId, CallType? callTypeFilter, ClosedCallInList? sortField)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, CallType? callTypeFilter, OpenCallInList? sortField)
    {
        throw new NotImplementedException();
    }

    public void SelectCallForTreatment(int volunteerId, int callId)
    {
        throw new NotImplementedException();
    }

    public void UpdateCall(Call call)
    {
        throw new NotImplementedException();
    }
}
