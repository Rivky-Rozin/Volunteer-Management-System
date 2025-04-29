namespace Helpers;
using DalApi;



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
}

