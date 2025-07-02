using BO;

namespace BlApi;

public interface ICall: IObservable
{
    int[] GetCallStatusCounts();
    IEnumerable<BO.CallInList> GetCallList(CallInListField? filterField=null, object? filterValue=null, CallInListField? sortField=null);
    BO.Call GetCallDetails(int callId);
    void UpdateCall(BO.Call call);
    void DeleteCall(int callId);
    void AddCall(BO.Call call);
    IEnumerable<BO.ClosedCallInList> GetClosedCallsOfVolunteer(int volunteerId, BO.CallType? callTypeFilter, BO.ClosedCallInListEnum? sortField);
    IEnumerable<BO.OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, BO.CallType? callTypeFilter, BO.OpenCallInListEnum? sortField);
    void CompleteCallTreatment(int volunteerId, int callId);
    void CancelCallTreatment(int requesterId, int callId);
    void SelectCallForTreatment(int volunteerId, int callId);
}