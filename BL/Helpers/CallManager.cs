namespace Helpers;

using System;
using BO;
using DalApi;
using DO;

internal static class CallManager
    {
        private static IDal s_dal = Factory.Get; //stage 4
    public static BO.Call ConvertToBO(DO.Call call) => new BO.Call(
       call.Id,
       (BO.CallType)call.CallType,
       call.FullAddress,
       call.Latitude,
       call.Longitude,
       call.OpenTime,
       call.Description,
       call.MaxCallTime
   );

    public static DO.Call ConvertToDO(BO.Call call) => new DO.Call(
        call.Id,
        (DO.CallType)call.CallType,
        call.FullAddress,
        call.Latitude,
        call.Longitude,
        call.OpenTime,
        call.Description,
        call.MaxCallTime
    );

    public static BO.CallInList ConvertToCallInList(DO.Call call)
    {
        return new BO.CallInList
        {
            Id = call.Id,
            CallType = (BO.CallType)call.CallType,
            FullAddress = call.FullAddress,
            OpenTime = call.OpenTime,
            MaxCallTime = call.MaxCallTime
            // אפשר להוסיף תכונות נוספות אם CallInList כולל יותר מידע
        };
    }

    internal static bool MatchesFilter(BO.CallInList call, CallInListField field, object value)
    {
        return field switch
        {
            CallField.Id => int.TryParse(value.ToString(), out var id)
                            && call.Id.HasValue && call.Id.Value == id,

            CallField.CallType => Enum.TryParse(typeof(BO.CallType), value.ToString(), out var callTypeObj)
                                  && call.CallType == (BO.CallType)callTypeObj,

            CallField.FullAddress => call.LastVolunteerName != null
                                     && call.LastVolunteerName.Contains(value.ToString() ?? "", StringComparison.OrdinalIgnoreCase),

            CallField.OpenTime => DateTime.TryParse(value.ToString(), out var openTime)
                                  && call.OpenTime.Date == openTime.Date,

            CallField.EntryToTreatmentTime => call.TimeUntilAssigning.HasValue
                                              && TimeSpan.TryParse(value.ToString(), out var entryTime)
                                              && call.TimeUntilAssigning.Value == entryTime,

            CallField.ActualTreatmentEndTime => call.totalTreatmentTime.HasValue
                                                && TimeSpan.TryParse(value.ToString(), out var totalTime)
                                                && call.totalTreatmentTime.Value == totalTime,

            CallField.TreatmentEndType => Enum.TryParse(typeof(BO.CallStatus), value.ToString(), out var statusObj)
                                          && call.Status == (BO.CallStatus)statusObj,

            _ => true
        };
    }
    //todo
    internal static BO.ClosedCallInList ConvertToClosedCallInList(BO.Call boCall)
    {
        throw new NotImplementedException();
    }
}

