using BO;

namespace BlApi;

public interface ICall: IObservable
{
    int[] GetCallStatusCounts();
    IEnumerable<BO.CallInList> GetCallList(CallInListField? filterField, object? filterValue, CallInListField? sortField);
    BO.Call GetCallDetails(int callId);
    void UpdateCall(BO.Call call);
    void DeleteCall(int callId);
    void AddCall(BO.Call call);
    IEnumerable<BO.ClosedCallInList> GetClosedCallsOfVolunteer(int volunteerId, BO.CallType? callTypeFilter, BO.ClosedCallInListEnum? sortField);
    IEnumerable<BO.OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, BO.CallType? callTypeFilter, BO.OpenCallInListEnum? sortField);
    void CompleteCallTreatment(int volunteerId, int assignmentId);
    void CancelCallTreatment(int requesterId, int assignmentId);
    void SelectCallForTreatment(int volunteerId, int callId);
}