namespace Dal;
using DalApi;

sealed internal class DalList : IDal

{
    //זה עושה LAZY INITIALIZATION כי האובייקט נוצר - קוראים לניו רק כשניגשים אליו באמת, ולא לפני
    //private static readonly Lazy<IDal> _instance = new Lazy<IDal>(() => new DalList());

    //public static IDal Instance => _instance.Value;
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
