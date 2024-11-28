using Dal;
using DalApi;
using DO;



namespace DalTest;
internal class Program
{

    //creating lists for the database
    private static IVolunteer? s_dalVolunteer = new VolunteerImplementation(); //stage 1
    private static ICall? s_dalCall = new CallImplementation(); //stage 1
    private static IAssignment? s_dalAssignment = new AssignmentImplementation(); //stage 1
    private static IConfig? s_dalConfig = new ConfigImplementation(); //stage 1

    //methods
    //private void DisplayMainMenu()
    //{
    //}
    static void Main(string[] args)
    {
        try
        {
            Initialization.Do(s_dalCall,s_dalVolunteer,s_dalAssignment,s_dalConfig);


        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
