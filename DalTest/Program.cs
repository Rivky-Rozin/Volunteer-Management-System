using Dal;
using DalApi;
using DO;
using System.Data;

namespace DalTest;
internal class Program
{
    //מופעים של המחלקה לצורך הפעלת המתודות של המחלקות
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

    #region general methods 
    // שאלתי את GPT איך אני יכולה לסדר את הקוד והוא הביא את הרעיון הזה
    //הצגת התפריט הראשי
    private static void ShowMainMenu()
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

            //if (!exit)
            //{
            //    Console.WriteLine("press any key to continue");
            //    Console.ReadKey();
            //}
        }
    }
    //אתחול הכל
    private static void InitializeAll()
    {
        Initialization.Do(s_dalCall, s_dalVolunteer, s_dalAssignment, s_dalConfig); // קריאה לאתחול הנתונים
        Console.WriteLine("data initialized succesfully");
    }
    //הצגת כל המידע
    private static void DisplayAllData()
    {
        ReadAllVolunteers();
        ReadAllCalls();
        ReadAllAssignments();
    }
    //מחיקת כל המידע
    private static void ResetDatabase()
    {
        DeleteAllVolunteers();
        DeleteAllCalls();
        DeleteAllAssignments();
        Console.WriteLine("Database resetted succesfully");
    }
    #endregion

    #region Call Methods

    public static void ShowEntityCall()
    {
        bool exit = false;

        while (!exit)
        {
            Console.WriteLine("=== Menu for Call Entity ===");
            Console.WriteLine("1. Add a new object (Create)");
            Console.WriteLine("2. Display object by ID (Read)");
            Console.WriteLine("3. Display all objects (ReadAll)");
            Console.WriteLine("4. Update an existing object (Update)");
            Console.WriteLine("5. Delete an object from the list (Delete)");
            Console.WriteLine("6. Delete all objects from the list (DeleteAll)");
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
    //המתודות של call
    static void ReadAllCalls()
    {
        var allCalls = s_dalCall.ReadAll();  // קורא לרשימת כל הקריאות
        string allCallsString = string.Join(Environment.NewLine, allCalls.Select(call => call.ToString()));
        Console.WriteLine(allCallsString);
    }
    static void UpdateCall()
    {
        int id = getInt("enter the ID to update")
        Call Item = CreateNewCall(id); // מתודה לעדכון אובייקט קיים
        s_dalCall.Update(Item);
    }
    static void DeleteAllCalls()
    {
        s_dalCall.DeleteAll();
        Console.WriteLine("All the calls were deleted");
    }
    static void DeleteCall()
    {
        try
        {
            int id = getInt("enter the ID to delete")
            s_dalCall.Delete(id);
            Console.WriteLine("Call deleted successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    static Call? CreateNewCall(int id = 0)
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
            int id = getInt("Enter the call ID: ");
            var call = s_dalCall.Read(id);

            if (call != null)
            {
                // Display call data
                Console.WriteLine("The call found:");
                Console.WriteLine($"ID: {call.Id}");
                Console.WriteLine($"Call type: {call.CallType}");
                Console.WriteLine($"Address: {call.FullAddress}");
                Console.WriteLine($"Coordinates: ({call.Latitude}, {call.Longitude})");
                Console.WriteLine($"Open time: {call.OpenTime}");
                Console.WriteLine($"Description: {call.Description}");
                Console.WriteLine($"Max end time: {(call.MaxCallTime.HasValue ? call.MaxCallTime.Value.ToString() : "Not set")}");
            }
            else
            {
                Console.WriteLine("No call found with this ID.");
            }
        }

        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    #endregion

    #region Volunteer Methods
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
                        createNewVolunteer(); // מתודה להוספת אובייקט חדש
                        break;
                    case "2":
                        ReadVolunteerlById(); // מתודה להצגת אובייקט לפי מזהה
                        break;
                    case "3":
                        ReadAllVolunteers(); // מתודה להצגת כל האובייקטים
                        break;
                    case "4":
                        UpdateVolunteer(); // מתודה לעדכון אובייקט קיים
                        break;
                    case "5":
                        DeleteVolunteerById(); // מתודה למחיקת אובייקט לפי מזהה
                        break;
                    case "6":
                        DeleteAllVolunteers(); // מתודה למחיקת כל האובייקטים
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

            //if (!exit)
            //{
            //    Console.WriteLine("press any key to continue");
            //    Console.ReadKey();
            //}
        }
    }
    //volunteer המתודות של
    //מחיקת כל המתנדבים
    private static void DeleteAllVolunteers()
    {
        s_dalVolunteer.DeleteAll();
        Console.WriteLine("All the volunteers were deleted");
    }
    //ID מחיקת מתנדב לפי
    private static void DeleteVolunteerById()
    {
        try
        {
            int id = getInt("enter the ID to delete")
            s_dalVolunteer.Delete(id);
            Console.WriteLine("Volunteer deleted successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    //עדכון פרטי מתנדב
    private static void UpdateVolunteer()
    {
        try
        {
            int id = getInt("enter the ID to update");
            Volunteer? doesExist = s_dalVolunteer.Read(id);
            if (doesExist == null)
            {
                Console.WriteLine("A volunteer with this ID does not exist");
            }
            else
            {
                //יוצר וולנטיר חדש ומכניס אותו למערך. הבעיה שמדובר בעדכון וולנטיר, ואז הוא מתריע שהוולנטיר קיים כבר .
                Volunteer updatedVolunteer = createNewVolunteer(id); // מתודה לעדכון אובייקט קיים
                s_dalVolunteer.Update(updatedVolunteer);
                Console.WriteLine("Volunteer updated successfully!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    //הצגת כל המתנדבים
    private static void ReadAllVolunteers()
    {
        List<Volunteer> volunteers = s_dalVolunteer.ReadAll();

        if (volunteers.Count == 0) // בדיקה אם הרשימה ריקה
        {
            Console.WriteLine("No volunteers found.");
            return;
        }

        Console.WriteLine("List of all volunteers:");
        foreach (var volunteer in volunteers)
        {
            Console.WriteLine($"ID: {volunteer.Id}, Name: {volunteer.Name}, Phone: {volunteer.Phone}, Email: {volunteer.Email}, Role: {volunteer.Role}, Active: {volunteer.IsActive}, Distance Kind: {volunteer.DistanceKind}, Address: {volunteer.Address ?? "N/A"}");
        }
    }
    //הצגת מתנדב לפי מספר מזהה
    private static void ReadVolunteerlById()
    {
        try
        {
            int id = getInt("Enter volunteer ID: ")
            Volunteer? volunteer = s_dalVolunteer.Read(id);
            if (volunteer == null)
            {
                Console.WriteLine("A volunteer with this ID does not exist");
                return;
            }
            //Console.WriteLine($"ID: {volunteer.Id}, Name: {volunteer.Name}, Phone: {volunteer.Phone}, Email: {volunteer.Email}, Role: {volunteer.Role}, Active: {volunteer.IsActive}, Distance Kind: {volunteer.DistanceKind}, Address: {volunteer.Address ?? "N/A"}");
            Console.WriteLine(volunteer);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    //יצירת מתנדב חדש
    private static Volunteer? createNewVolunteer(int id = 0)
    {
        bool isUpdate = true;
        if (id == 0)
        {
            isUpdate = false;
            id = getInt("Enter volunteer ID: ")
        }

        Console.Write("Enter volunteer name: ");
        string name = Console.ReadLine();

        Console.Write("Enter phone number: ");
        string phone = Console.ReadLine();

        Console.Write("Enter email address: ");
        string email = Console.ReadLine();

        //למה כשמכניסים 0 הוא שם מנהל?
        Console.WriteLine("Choose volunteer role (0 for Regular, 1 for Manager): ");
        if (!Enum.TryParse<VolunteerRole>(Console.ReadLine(), out VolunteerRole role))
        {
            Console.WriteLine("Invalid role. Defaulting to 'Regular'.");
            role = VolunteerRole.Regular;
        }

        Console.WriteLine("Is the volunteer active? (yes/no): ");
        bool isActive = Console.ReadLine()?.ToLower() == "yes";

        Console.WriteLine("Choose distance kind (0 for Aerial, 1 for Ground): ");
        if (!Enum.TryParse<DistanceKind>(Console.ReadLine(), out DistanceKind distanceKind))
        {
            Console.WriteLine("Invalid distance kind. Defaulting to 'Ground'.");
            distanceKind = DistanceKind.Ground;
        }

        Console.Write("Enter address (optional, press Enter to skip): ");
        string? address = Console.ReadLine();
        address = string.IsNullOrWhiteSpace(address) ? null : address;
        Volunteer newVolunteer = new(id, name, phone, email, role, isActive, distanceKind, address);
        if (isUpdate == true)
        {
            return newVolunteer;
        }
        s_dalVolunteer.Create(newVolunteer);
        return null;
    }

    #endregion

    #region Call Assignment
    //    //Assignment התפריט של
    public static void ShowEntityAssignment()
    {
        bool exit = false;

        while (!exit)
        {
            //Console.Clear();
            Console.WriteLine("=== Menu for Assignment Entity ===");
            Console.WriteLine("1. Add a new assignment (Create)");
            Console.WriteLine("2. Display assignment by ID (Read)");
            Console.WriteLine("3. Display all assignments (ReadAll)");
            Console.WriteLine("4. Update an existing assignment (Update)");
            Console.WriteLine("5. Delete a assignment from the list (Delete)");
            Console.WriteLine("6. Delete all assignments from the list (DeleteAll)");
            Console.WriteLine("0. Exit");
            Console.Write("Choose an option: ");


            string choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        CreateNewAssignment(); // מתודה להוספת אובייקט חדש
                        break;
                    case "2":
                        ReadAssignmentlById(); // מתודה להצגת אובייקט לפי מזהה
                        break;
                    case "3":
                        ReadAllAssignments(); // מתודה להצגת כל האובייקטים
                        break;
                    case "4":
                        UpdateAssignment(); // מתודה לעדכון אובייקט קיים
                        break;
                    case "5":
                        DeleteAssignmentById(); // מתודה למחיקת אובייקט לפי מזהה
                        break;
                    case "6":
                        DeleteAllAssignments(); // מתודה למחיקת כל האובייקטים
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

            //if (!exit)
            //{
            //    Console.WriteLine("press any key to continue");
            //    Console.ReadKey();
            //}
        }
    }
    //Assignment המתודות של
    private static void DeleteAllAssignments()
    {
        s_dalAssignment.DeleteAll();
        Console.WriteLine("All the assignment were deleted");
    }
    private static void DeleteAssignmentById()
    {
        try
        {
            int id = getInt("Enter the assignment ID to delete: ")
           s_dalAssignment.Delete(id);
            Console.WriteLine("Assignment deleted successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
    private static void UpdateAssignment()
    {
        try
        {
            int id = getInt("enter the ID to update")
            Assignment? doesExist = s_dalAssignment.Read(id);
            if (doesExist == null)
            {
                Console.WriteLine("An assignment with this ID does not exist");
            }
            else
            {
                Assignment? updatedAssignment = CreateNewAssignment(id); // מתודה לעדכון אובייקט קיים
                s_dalAssignment.Update(updatedAssignment);
                Console.WriteLine("Assignment updated successfully!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    private static void ReadAllAssignments()
    {
        List<Assignment> assignments = s_dalAssignment.ReadAll();

        if (assignments.Count == 0) // בדיקה אם הרשימה ריקה
        {
            Console.WriteLine("No assignments found.");
            return;
        }

        Console.WriteLine("List of all assignments:");
        foreach (var assignment in assignments)
        {
            Console.WriteLine($"ID: {assignment.Id}, Volunteer ID: {assignment.VolunteerId}, Call ID: {assignment.CallId}");
        }

    }
    private static void ReadAssignmentById()
    {
        try
        {
            int id = getInt("Enter Assignment ID: ")
            Assignment? assignment = s_dalAssignment.Read(id);
            if (assignment == null)
            {
                Console.WriteLine("An assignment with this ID does not exist.");
                return;
            }
            //Console.WriteLine($"ID: {assignment.Id}, Volunteer ID: {assignment.VolunteerId}, Call ID: {assignment.CallId}");
            Console.WriteLine(assignment);
        }
            else
        {
            Console.WriteLine("Invalid ID. Please enter a valid integer.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
    private static Assignment? CreateNewAssignment(int id = 0)
    {
        try
        {
            int id = getInt("Enter volunteer ID: ")
            Volunteer? isValidVolunteerId = s_dalVolunteer.Read(volunteerId);
            if (isValidVolunteerId == null)
            {
                throw new Exception("A volunteer with this ID does not exist.");
            }
            int id = getInt("Enter call ID: ")
            Call? isValidCallId = s_dalCall.Read(callID);
            if (isValidCallId == null)
            {
                throw new Exception("A call with this ID does not exist.");
            }

            Assignment newAssignment = new()
            {
                Id = id,
                VolunteerId = volunteerId,
                CallId = callID
            };

            s_dalAssignment.Create(newAssignment);

            return id != 0 ? newAssignment : null;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }
    #endregion

    //------------------------------------------configure-----------------------------------------
    //configure התפריט של
    private static void ShowConfigurationSubMenu() { }

    //מתודת עזר לתוספת של TryParse
    static int getInt(string message)
    {
        int myInt;
        bool tryInt;

        do
        {
            Console.WriteLine(message);
            string input = Console.ReadLine();
            tryInt = int.TryParse(input, out myInt);
            if (!tryInt)
            {
                Console.WriteLine("Please enter only numbers.");
            }
        } while (!tryInt);

        return myInt;
    } }