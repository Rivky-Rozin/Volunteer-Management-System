namespace Dal;
using DalApi;
using System.Diagnostics;

sealed internal class DalXml : IDal
{
    public static IDal Instance { get; } = new DalXml();
    private DalXml() { }

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
