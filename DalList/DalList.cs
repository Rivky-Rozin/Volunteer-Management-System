namespace Dal;
using DalApi;

sealed internal class DalList : IDal

{
    public static IDal Instance { get; } = new DalList();
    private DalList() { }

    public ICall Call { get; } = new CallImplementation();
    public IAssignment Assignment { get; } = new AssignmentImplementation();
    public IVolunteer Volunteer { get; } = new VolunteerImplementation();
    public IConfig Config { get; } = new ConfigImplementation();

    public void ResetDB()
    {
        Call.DeleteAll();
        Assignment.DeleteAll();
        Volunteer.DeleteAll();
      	  
        Config.Reset();

    }
}
