namespace Helpers;

using BO;
using DalApi;



    internal static class CallManager
    {
    private static IDal s_dal = Factory.Get; //stage 4
    internal static BO.Call ConvertToBO(DO.Call call)
    {
        return new BO.Call
        {
            Id = call.Id,
            CallType = (BO.CallType)call.CallType,
            Address = call.FullAddress,
            Latitude = call.Latitude,
            Longitude = call.Longitude,
            CreationTime = call.OpenTime,
            Description = call.Description,
            MaxFinishTime = call.MaxCallTime
        };
    }

    internal static DO.Call ConvertToDO(BO.Call call) => new DO.Call(
        call.Id,
        (DO.CallType)call.CallType,
        call.Address,
        call.Latitude,
        call.Longitude,
        call.CreationTime,
        call.Description,
        call.MaxFinishTime
    );

    internal static BO.CallInList ConvertToCallInList(DO.Call call)
    {
        return new BO.CallInList
        {
            Id = call.Id,
            RequesterName = call.RequesterName,
            StartTime = call.StartTime,
            Status = (BO.CallStatus)call.Status
        };
    }
    // מתודת עזר לבדיקת התאמה לסינון
    internal static bool MatchesFilter(BO.CallInList call, CallField field, object value) { return field switch { CallFilterField.RequesterName => call.RequesterName == value.ToString(), CallFilterField.Status => Enum.TryParse(typeof(BO.CallStatus), value.ToString(), out var statusObj) && call.Status == (BO.CallStatus)statusObj, CallFilterField.StartTime => DateTime.TryParse(value.ToString(), out var time) && call.StartTime.Date == time.Date, _ => true }; }



}
    public static BO.ClosedCallInList ConvertToClosedCallInList(DO.Call call)
    {
        return new BO.ClosedCallInList
        {
            Id = call.Id,
            RequesterName = call.RequesterName,
            CallType = (BO.CallType)call.CallType,
            StartTime = call.OpenTime,
            CloseTime = call.CloseTime ?? DateTime.MinValue, 
            // אם זה nullable Status = (BO.CallStatus)call.Status, FullAddress = call.FullAddress
            };
    }

    public static OpenCallInList ConvertToOpenCallInList(Call call)
    {
        // Fix for CS0117: Add the missing method definition  
        return new OpenCallInList
        {
            Id = call.Id,
            CallId = call.CallId,
            CallType = call.CallType,
            OpenTime = call.OpenTime,
            TimeUntilAssigning = call.TimeUntilAssigning,
            LastVolunteerName = call.LastVolunteerName,
            totalTreatmentTime = call.totalTreatmentTime,
            Status = call.Status,
            NumberOfAssignments = call.NumberOfAssignments
        };
    }

}


