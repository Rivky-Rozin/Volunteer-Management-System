
namespace DalTest;
using Dal;
using DalApi;
using DO;
internal class Program
{

    //creating lists for the database
    private static IVolunteer? dalVolunteer = new VolunteerImplementation(); //stage 1
    private static ICall? s_dalCall = new CallImplementation(); //stage 1
    private static IAssignment? s_dalAssignment = new AssignmentImplementation(); //stage 1
    private static IConfig? s_dalConfig = new ConfigImplementation(); //stage 1
    
    //methods
    private void DisplayMainMenu()
    {
    }
    static void Main(string[] args)
    { 
        try
        {

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
