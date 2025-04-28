using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlApi
{
    public interface ICall
    {
        int[] GetCallStatusCounts();
        IEnumerable<BO.CallInList> GetCallList(BO.CallInList? filterField, object? filterValue, BO.CallInList? sortField);
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
}
