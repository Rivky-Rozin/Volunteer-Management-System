using Dal;
using DalApi;
using DO;
using Microsoft.VisualBasic;
using System.Net;



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
                        Initialization.Do(); // קריאה לאתחול הנתונים
                        Console.WriteLine("נתונים אותחלו בהצלחה!");
                        break;

                    case "5":
                        DisplayAllData(); // מתודה שמציגה את כל הנתונים בבסיס הנתונים
                        break;

                    case "6":
                        ShowConfigurationSubMenu(); // מתודה להצגת תפריט ישות תצורה
                        break;

                    case "7":
                        ResetDatabase(); // מתודה שמאפסת את בסיס הנתונים ואת נתוני התצורה
                        Console.WriteLine("בסיס הנתונים ונתוני התצורה אופסו בהצלחה!");
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
    public static void ShowEntityCall()
    {
        bool exit = false;

        while (!exit)
        {
            Console.Clear();
            Console.WriteLine("=== תפריט עבור ישות Call ===");
            Console.WriteLine("1. הוספת אובייקט חדש (Create)");
            Console.WriteLine("2. תצוגת אובייקט לפי מזהה (Read)");
            Console.WriteLine("3. תצוגת רשימת כל האובייקטים (ReadAll)");
            Console.WriteLine("4. עדכון אובייקט קיים (Update)");
            Console.WriteLine("5. מחיקת אובייקט מהרשימה (Delete)");
            Console.WriteLine("6. מחיקת כל האובייקטים מהרשימה (DeleteAll)");
            Console.WriteLine("0. יציאה");
            Console.Write("בחר אופציה: ");

            string choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        createNewCall(); // מתודה להוספת אובייקט חדש
                        break;

                    case "2":
                        ReadCallById(); // מתודה להצגת אובייקט לפי מזהה
                        break;

                    case "3":
                        ReadAllCalls(); // מתודה להצגת כל האובייקטים
                        break;

                    case "4":
                        UpdateCall(); // מתודה לעדכון אובייקט קיים
                        break;

                    case "5":
                        DeleteCallById(); // מתודה למחיקת אובייקט לפי מזהה
                        break;

                    case "6":
                        DeleteAllCalls(); // מתודה למחיקת כל האובייקטים
                        break;

                    case "0":
                        exit = true;
                        Console.WriteLine("יציאה מתת-תפריט...");
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

    public static void ShowEntityAssignment() { }
    public static void ShowEntityVolunteer() { }

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

}
