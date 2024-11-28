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
        try
        {
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
            return new Call(id, callType, address, latitude, longitude, openTime, description, maxCallTime);

        }
        catch (FormatException ex)
        {
            Console.WriteLine($"שגיאה בהזנת נתונים: {ex.Message}");
            return null; // מחזירים null במקרה של שגיאה
        }
        catch (Exception ex)
        {
            Console.WriteLine($"שגיאה כללית: {ex.Message}");
            return null; // מחזירים null במקרה של שגיאה
        }
    }

    static void ReadByCallId()
    {
        try
        {
            // בקשת מידע מהמשתמש עבור ה-ID
            Console.Write("הזן את מזהה הקריאה (ID): ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("מזהה לא חוקי");
                return;
            }


            // קריאה למתודת ה-Read עם ה-ID שהוזן
            var call = s_dalCall.Read(id);

            if (call != null)
            {
                // הצגת נתוני הקריאה
                Console.WriteLine("הקריאה שנמצאה:");
                Console.WriteLine($"ID: {call.Id}");
                Console.WriteLine($"סוג הקריאה: {call.CallType}");
                Console.WriteLine($"כתובת: {call.FullAddress}");
                Console.WriteLine($"קואורדינטות: ({call.Latitude}, {call.Longitude})");
                Console.WriteLine($"זמן פתיחה: {call.OpenTime}");
                Console.WriteLine($"תיאור: {call.Description}");
                Console.WriteLine($"זמן סיום מקסימלי: {(call.MaxCallTime.HasValue ? call.MaxCallTime.Value.ToString() : "לא קבוע")}");
            }
            else
            {
                Console.WriteLine("לא נמצאה קריאה עם מזהה זה.");
            }
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
