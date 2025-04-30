namespace BlApi;

public interface ICall
{
    int[] GetCallStatusCounts();
    IEnumerable<BO.CallInList> GetCallList(Enum? filterField, object? filterValue, Enum? sortField);
    BO.Call GetCallDetails(int callId);
    void UpdateCall(BO.Call call);
    void DeleteCall(int callId);
    void AddCall(BO.Call call);
    IEnumerable<BO.ClosedCallInList> GetClosedCallsOfVolunteer(int volunteerId, BO.CallType? callTypeFilter, BO.ClosedCallInList? sortField);
    IEnumerable<BO.OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, BO.CallType? callTypeFilter, BO.OpenCallInList? sortField);
    void CompleteCallTreatment(int volunteerId, int assignmentId);
    void CancelCallTreatment(int requesterId, int assignmentId);
    void SelectCallForTreatment(int volunteerId, int callId);
}
