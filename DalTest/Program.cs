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
            ShowMainMenu();

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    public static void ShowMainMenu()
    {
        bool exit = false;

        while (!exit)
        {
            Console.Clear();
            Console.WriteLine("=== תפריט ראשי ===");
            Console.WriteLine("1. הצגת תת-תפריט עבור ישות 1");
            Console.WriteLine("2. הצגת תת-תפריט עבור ישות 2");
            Console.WriteLine("3. הצגת תת-תפריט עבור ישות 3");
            Console.WriteLine("4. אתחול הנתונים (Initialization.Do)");
            Console.WriteLine("5. הצגת כל הנתונים בבסיס הנתונים");
            Console.WriteLine("6. הצגת תת-תפריט עבור ישות תצורה");
            Console.WriteLine("7. איפוס בסיס נתונים ואיפוס נתוני התצורה");
            Console.WriteLine("0. יציאה");
            Console.Write("בחר אופציה: ");

            string choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        ShowEntityCall(); // מתודה להצגת תת-תפריט עבור ישות Call
                        break;

                    case "2":
                        ShowEntityAssignment(); // מתודה להצגת תת-תפריט עבור ישות Assignment
                        break;

                    case "3":
                        ShowEntityVolunteer(); // מתודה להצגת תת-תפריט עבור ישות Volunteer
                        break;

                    case "4":
                        InitializeAll();
                        break;

                    case "5":
                        DisplayAllData(); // מתודה שמציגה את כל הנתונים בבסיס הנתונים
                        break;

                    case "6":
                        ShowConfigurationSubMenu(); // מתודה להצגת תפריט ישות תצורה
                        break;

                    case "7":
                        ResetDatabase(); // מתודה שמאפסת את בסיס הנתונים ואת נתוני התצורה
                        break;

                    case "0":
                        exit = true;
                        Console.WriteLine("יציאה מתפריט ראשי...");
                        break;

                    default:
                        Console.WriteLine("אופציה לא חוקית, נסה שנית.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"שגיאה: {ex.Message}");
            }

            if (!exit)
            {
                Console.WriteLine("\nלחץ על מקש כלשהו כדי להמשיך...");
                Console.ReadKey();
            }
        }
    }

    public static void ShowEntityCall() { }
    public static void ShowEntityAssignment() { }
    public static void ShowEntityVolunteer() { }
    public static void InitializeAll()
    {
        Initialization.Do(s_dalCall, s_dalVolunteer, s_dalAssignment, s_dalConfig); // קריאה לאתחול הנתונים
        Console.WriteLine("data initialized succesfully");
    }
    public static void DisplayAllData() {
        Console.WriteLine(s_dalCall.ReadAll());
        Console.WriteLine(s_dalAssignment.ReadAll());
        Console.WriteLine(s_dalVolunteer.ReadAll());
    }
    private static void ShowConfigurationSubMenu()
    {
    }
    private static void ResetDatabase()
    {
        s_dalCall.DeleteAll();
        s_dalAssignment.DeleteAll();
        s_dalVolunteer.DeleteAll();
        Console.WriteLine("Database resetted succesfully");
    }
}
