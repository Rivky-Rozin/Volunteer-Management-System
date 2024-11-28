using Dal;
using DalApi;
using DO;
using System.Data;



namespace DalTest;
internal class Program
{

    //creating lists for the database
    private static IVolunteer? s_dalVolunteer = new VolunteerImplementation(); //stage 1
    private static ICall? s_dalCall = new CallImplementation(); //stage 1
    private static IAssignment? s_dalAssignment = new AssignmentImplementation(); //stage 1
    private static IConfig? s_dalConfig = new ConfigImplementation(); //stage 1

    static void Main(string[] args)
    {
        try
        {
            Initialization.Do(s_dalCall, s_dalVolunteer, s_dalAssignment, s_dalConfig);
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
            //Console.Clear();
            Console.WriteLine("=== Main Menu ===");
            Console.WriteLine("1. Display submenu for Entity call");
            Console.WriteLine("2. Display submenu for Entity Assignment");
            Console.WriteLine("3. Display submenu for Entity volunteer");
            Console.WriteLine("4. Initialize data (Initialization.Do)");
            Console.WriteLine("5. Display all data in the database");
            Console.WriteLine("6. Display submenu for Configuration Entity");
            Console.WriteLine("7. Reset database and configuration data");
            Console.WriteLine("0. Exit");
            Console.Write("Select an option: ");

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
                        Console.WriteLine("Exiting main menu");
                        break;

                    default:
                        Console.WriteLine("Invalid option, please try again");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (!exit)
            {
                Console.WriteLine("press any key to continue");
                Console.ReadKey();
            }
        }
    }
    public static void ShowEntityCall()
    {
        Initialization.Do(s_dalCall, s_dalVolunteer, s_dalAssignment, s_dalConfig); // קריאה לאתחול הנתונים
        Console.WriteLine("data initialized succesfully");
    }

    //להוציא מהסלשה בסוף!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    public static void DisplayAllData()
    {
        ReadAllVolunteers();
        //עדיין אין מימוש
        //ReadAllCalls();
        //ReadAllAssignments();
    }

    //configure התפריט של
    private static void ShowConfigurationSubMenu()
    {
    }

    //להוציא מהסלשה בסוף!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    private static void ResetDatabase()
    {
        DeleteAllVolunteers();
        //DeleteAllCalls();
        //DeleteAllAssignments();
        Console.WriteLine("Database resetted succesfully");
    }

    //Assignment התפריט של
    public static void ShowEntityAssignment() { }

    //מתודה לקבלת פרטים מהמשתמש ליצור קריאה חדשה
    public static void createNewCall()
    {
        try
        {
            int id = 0;
            Console.Write("הזן את סוג הקריאה (Technical, Food וכו'): ");
            Enum callType = (Enum)Enum.Parse(typeof(Enum), Console.ReadLine(), true); // צריך להמיר את הטקסט שנכנס לסוג המתאים ב-Enum

            Console.Write("הזן את הכתובת: ");
            string address = Console.ReadLine();

            Console.Write("הזן את קואורדינטת הרוחב (Latitude): ");
            double latitude = double.Parse(Console.ReadLine());

            Console.Write("הזן את קואורדינטת האורך (Longitude): ");
            double longitude = double.Parse(Console.ReadLine());

            Console.Write("הזן את זמן פתיחת הקריאה (בפורמט yyyy-MM-dd HH:mm:ss): ");
            DateTime openTime = DateTime.Parse(Console.ReadLine());

            Console.Write("הזן תיאור (אופציונלי): ");
            string? description = Console.ReadLine();

            Console.Write("הזן את זמן הסיום המקסימלי (בפורמט yyyy-MM-dd HH:mm:ss) או השאר ריק אם אין: ");
            string maxCallTimeInput = Console.ReadLine();
            DateTime? maxCallTime = string.IsNullOrEmpty(maxCallTimeInput) ? (DateTime?)null : DateTime.Parse(maxCallTimeInput);

            // יצירת אובייקט חדש מסוג Call והוספה לרשימה
            s_dalCall!.Create(new Call(id, callType, address, latitude, longitude, openTime, description, maxCallTime));

            Console.WriteLine("הקריאה נוצרה בהצלחה!");
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"שגיאה בהזנת נתונים: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"שגיאה כללית: {ex.Message}");
        }
    }


    //התפריט של volunteer
    public static void ShowEntityVolunteer()
    {
        bool exit = false;

        while (!exit)
        {
            //Console.Clear();
            Console.WriteLine("=== Menu for volunteer Entity ===");
            Console.WriteLine("1. Add a new volunteer (Create)");
            Console.WriteLine("2. Display volunteer by ID (Read)");
            Console.WriteLine("3. Display all volunteers (ReadAll)");
            Console.WriteLine("4. Update an existing volunteer (Update)");
            Console.WriteLine("5. Delete a volunteer from the list (Delete)");
            Console.WriteLine("6. Delete all volunteers from the list (DeleteAll)");
            Console.WriteLine("0. Exit");
            Console.Write("Choose an option: ");


            string choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        Call item = CreateNewCall(); // מתודה להוספת אובייקט חדש
                        s_dalCall.Create(item);
                        break;

                    case "2":

                        ReadByCallId(); // מתודה להצגת אובייקט לפי מזהה
                        break;

                    case "3":
                        ReadAllCalls();
                        break;

                    case "4":
                        UpdateCall();
                        break;

                    case "5":
                        DeleteCall(); // מתודה למחיקת אובייקט לפי מזהה
                        break;

                    case "6":
                        s_dalCall.DeleteAll(); // מתודה למחיקת כל האובייקטים
                        break;

                    case "0":
                        exit = true;
                        Console.WriteLine("Exiting menu");
                        break;

                    default:
                        Console.WriteLine("Invalid option, try again");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (!exit)
            {
                Console.WriteLine("press any key to continue");
                Console.ReadKey();
            }
        }

        static void UpdateCall()
        {
            Console.WriteLine("enter the ID to update");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("מזהה לא חוקי");
                return;
            }
            Call Item = CreateNewCall(id); // מתודה לעדכון אובייקט קיים
            s_dalCall.Update(Item);
        }

        static void ReadAllCalls()
        {
            var allCalls = s_dalCall.ReadAll();  // קורא לרשימת כל הקריאות
            string allCallsString = string.Join(Environment.NewLine, allCalls.Select(call => call.ToString()));
            Console.WriteLine(allCallsString);
        }

        static void DeleteCall()
        {
            Console.WriteLine("enter the ID to delete");
            if (!int.TryParse(Console.ReadLine(), out int idd))
            {
                Console.WriteLine("מזהה לא חוקי");
                return;
            }
            s_dalCall.Delete(idd);
        }
    }
    static void InitializeAll()
    {
        Initialization.Do(s_dalCall, s_dalVolunteer, s_dalAssignment, s_dalConfig); // קריאה לאתחול הנתונים
        Console.WriteLine("data initialized succesfully");
    }
    static void DisplayAllData()
    {
        Console.WriteLine(s_dalCall.ReadAll());
        Console.WriteLine(s_dalAssignment.ReadAll());
        Console.WriteLine(s_dalVolunteer.ReadAll());
    }
    static void ShowConfigurationSubMenu()
    {
    }
    static void ResetDatabase()
    {
        s_dalCall.DeleteAll();
        s_dalAssignment.DeleteAll();
        s_dalVolunteer.DeleteAll();
        Console.WriteLine("Database resetted succesfully");
    }
    static void ShowEntityAssignment() { }
    static void ShowEntityVolunteer() { }

    //מתודה לקבלת פרטים מהמשתמש ליצור או עדכון קריאה 
    static Call CreateNewCall(int id = 0)
    {
        List<Volunteer> volunteers = s_dalVolunteer.ReadAll();

        if (volunteers.Count == 0) // בדיקה אם הרשימה ריקה
        {
            Console.WriteLine("No volunteers found.");
            return;
        }
            Console.Write("הזן את סוג הקריאה (Technical, Food וכו'): ");
            Enum callType = (Enum)Enum.Parse(typeof(CallType), Console.ReadLine(), true); // צריך להמיר את הטקסט שנכנס לסוג המתאים ב-Enum


            Console.Write("הזן את הכתובת: ");
            string address = Console.ReadLine();

            Console.Write("הזן את קואורדינטת הרוחב (Latitude): ");
            double latitude = double.Parse(Console.ReadLine());

            Console.Write("הזן את קואורדינטת האורך (Longitude): ");
            double longitude = double.Parse(Console.ReadLine());

            Console.Write("הזן את זמן פתיחת הקריאה (בפורמט yyyy-MM-dd HH:mm:ss): ");
            DateTime openTime = DateTime.Parse(Console.ReadLine());

            Console.Write("הזן תיאור (אופציונלי): ");
            string? description = Console.ReadLine();

            Console.Write("הזן את זמן הסיום המקסימלי (בפורמט yyyy-MM-dd HH:mm:ss) או השאר ריק אם אין: ");
            string maxCallTimeInput = Console.ReadLine();
            DateTime? maxCallTime = string.IsNullOrEmpty(maxCallTimeInput) ? (DateTime?)null : DateTime.Parse(maxCallTimeInput);

            // יצירת אובייקט חדש מסוג Call והוספה לרשימה
            s_dalCall!.Create(new Call(id, callType, address, latitude, longitude, openTime, description, maxCallTime));

            Console.WriteLine("הקריאה נוצרה בהצלחה!");
        }
        catch (FormatException ex)
        {
            Console.WriteLine("Invalid distance kind. Defaulting to 'Ground'.");
            distanceKind = DistanceKind.Ground;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"שגיאה כללית: {ex.Message}");
        }
    }

}
