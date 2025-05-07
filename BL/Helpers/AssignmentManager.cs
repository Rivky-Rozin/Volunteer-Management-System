using DalApi;

namespace Helpers;

internal static class AssignmentManager
{
    internal static ObserverManager Observers = new(); //stage 5 
    private static IDal s_dal = Factory.Get; //stage 4
}
