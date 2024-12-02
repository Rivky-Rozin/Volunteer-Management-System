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
        string allCallsString = string.Join(Environment.NewLine, allCalls.Select(call =>  call.ToString()));
        Console.WriteLine(allCallsString);
    }
    //static void UpdateCall()
    //{ 
    //    int id = getInt("Enter ID");
    //    // Find the object by ID
    //    Call? callToUpdate = s_dalCall.Read(id);
    //    if (callToUpdate == null)
    //    {
    //        Console.WriteLine("No call found with the given ID.");
    //        return;
    //    }

    //    Console.WriteLine("Current call details:");
    //    Console.WriteLine(callToUpdate);

    //    // Update call type
    //    Console.Write("Enter new call type (or leave blank to keep current): ");
    //    string callTypeInput = Console.ReadLine();
    //    if (!string.IsNullOrWhiteSpace(callTypeInput))
    //    {
    //        Enum.TryParse(typeof(CallType), callTypeInput, true, out object? callType) ;
    //    }

    //    // Update address
    //    Console.Write("Enter new address (or leave blank to keep current): ");
    //    string addressInput = Console.ReadLine();
    //    if (!string.IsNullOrWhiteSpace(addressInput)) {
    //        addressInput=callToUpdate.FullAddress;
    //    }


    //    // Update latitude
    //    Console.Write("Enter new latitude (or leave blank to keep current): ");
    //    string latitudeInput = Console.ReadLine();
    //    if (string.IsNullOrWhiteSpace(latitudeInput) && double.TryParse(latitudeInput, out double latitude))
    //    {
    //        latitude = callToUpdate.Latitude;
    //    }


    //    // Update longitude
    //    Console.Write("Enter new longitude (or leave blank to keep current): ");
    //    string longitudeInput = Console.ReadLine();
    //    if (!string.IsNullOrWhiteSpace(longitudeInput) && double.TryParse(longitudeInput, out double longitude))
    //    {
    //        callToUpdate.Longitude = longitude;
    //    }
    //    else if (!string.IsNullOrWhiteSpace(longitudeInput))
    //    {
    //        Console.WriteLine("Invalid longitude. No changes made.");
    //    }

    //    // Update open time
    //    Console.Write("Enter new open time (yyyy-MM-dd HH:mm:ss or leave blank to keep current): ");
    //    string openTimeInput = Console.ReadLine();
    //    if (!string.IsNullOrWhiteSpace(openTimeInput) && DateTime.TryParse(openTimeInput, out DateTime openTime))
    //    {
    //        callToUpdate.OpenTime = openTime;
    //    }
    //    else if (!string.IsNullOrWhiteSpace(openTimeInput))
    //    {
    //        Console.WriteLine("Invalid open time. No changes made.");
    //    }

    //    // Update description
    //    Console.Write("Enter new description (or leave blank to keep current): ");
    //    string descriptionInput = Console.ReadLine();
    //    if (!string.IsNullOrWhiteSpace(descriptionInput))
    //    {
    //        callToUpdate.Description = descriptionInput;
    //    }

    //    // Update max call time
    //    Console.Write("Enter new maximum call time (yyyy-MM-dd HH:mm:ss or leave blank to keep current): ");
    //    string maxCallTimeInput = Console.ReadLine();
    //    if (!string.IsNullOrWhiteSpace(maxCallTimeInput) && DateTime.TryParse(maxCallTimeInput, out DateTime maxCallTime))
    //    {
    //        callToUpdate.MaxCallTime = maxCallTime;
    //    }
    //    else if (!string.IsNullOrWhiteSpace(maxCallTimeInput))
    //    {
    //        Console.WriteLine("Invalid maximum call time. No changes made.");
    //    }
    //}
    static void DeleteAllCalls()
    {
        s_dalCall.DeleteAll();
        Console.WriteLine("All the calls were deleted");
    }
    static void DeleteCall()
    {
        try
        {
            int id = getInt("enter the ID to delete");
            s_dalCall.Delete(id);
            Console.WriteLine("Call deleted successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    static Call? CreateNewCall()
    {
        try
        {
            Console.Write("Enter the call type (Technical, Food, etc.): ");
            string? callTypeInput = Console.ReadLine();
            if (!Enum.TryParse(typeof(CallType), callTypeInput, true, out var callType) || callType == null)
            {
                throw new Exception("Invalid call type.");
                
            }

            Console.Write("Enter the address: ");
            string? address = Console.ReadLine();
            if (string.IsNullOrEmpty(address))
            {
                throw new Exception("Address cannot be empty.");
              
            }

            Console.Write("Enter the latitude: ");
            if (!double.TryParse(Console.ReadLine(), out double latitude))
            {
                throw new Exception("Invalid latitude.");
                
            }

            Console.Write("Enter the longitude: ");
            if (!double.TryParse(Console.ReadLine(), out double longitude))
            {
                throw new Exception("Invalid longitude.");
            }

            Console.Write("Enter the open time (format: yyyy-MM-dd HH:mm:ss): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime openTime))
            {
                throw new Exception ("Invalid open time format.");
            }

            Console.Write("Enter a description (optional): ");
            string? description = Console.ReadLine();

            Console.Write("Enter the maximum end time (format: yyyy-MM-dd HH:mm:ss) or leave blank if none: ");
            string? maxCallTimeInput = Console.ReadLine();
            DateTime? maxCallTime = string.IsNullOrEmpty(maxCallTimeInput)
                ? (DateTime?)null
                : DateTime.TryParse(maxCallTimeInput, out DateTime parsedMaxCallTime)
                    ? parsedMaxCallTime
                    : null;

            if (!string.IsNullOrEmpty(maxCallTimeInput) && maxCallTime == null)
            {
                throw new Exception("Invalid maximum end time format.");
            }

            // Create a new Call object and return it
            return new Call(id, (CallType)callType, address, latitude, longitude, openTime, description, maxCallTime);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null;
        }
    }
    static Call? UpdateCall()
    {
        try
        {
            int id = getInt("Enter the ID of the Call");



            Console.Write("Enter the call type (Technical, Food, etc.): ");
            string? callTypeInput = Console.ReadLine();
            if (string.IsNullOrEmpty(callTypeInput) || !Enum.TryParse(typeof(CallType), callTypeInput, true, out var callType) || callType == null)
            {
                throw new Exception("Invalid call type.");
            }

            Console.Write("Enter the address: ");
            string? address = Console.ReadLine();
            address = string.IsNullOrEmpty(address) ? "0" : address;

            Console.Write("Enter the latitude: ");
            string? latitudeInput = Console.ReadLine();
            double latitude = string.IsNullOrEmpty(latitudeInput) ? 0 : double.TryParse(latitudeInput, out var lat) ? lat : 0;

            Console.Write("Enter the longitude: ");
            string? longitudeInput = Console.ReadLine();
            double longitude = string.IsNullOrEmpty(longitudeInput) ? 0 : double.TryParse(longitudeInput, out var lon) ? lon : 0;

            Console.Write("Enter the open time (format: yyyy-MM-dd HH:mm:ss): ");
            string? openTimeInput = Console.ReadLine();
            DateTime openTime = string.IsNullOrEmpty(openTimeInput) || !DateTime.TryParse(openTimeInput, out var parsedOpenTime)
                ? DateTime.MinValue
                : parsedOpenTime;

            Console.Write("Enter a description (optional): ");
            string? description = Console.ReadLine();
            description = string.IsNullOrEmpty(description) ? "0" : description;

            Console.Write("Enter the maximum end time (format: yyyy-MM-dd HH:mm:ss) or leave blank if none: ");
            string? maxCallTimeInput = Console.ReadLine();
            DateTime? maxCallTime = string.IsNullOrEmpty(maxCallTimeInput)
                ? null
                : DateTime.TryParse(maxCallTimeInput, out var parsedMaxCallTime)
                    ? parsedMaxCallTime
                    : null;

            // Create a new Call object and return it
            return new Call(id, (CallType)callType, address, latitude, longitude, openTime, description, maxCallTime);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null;
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
            int id = getInt("enter the ID to delete");
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
                throw new Exception("A volunteer with this ID does not exist");           
            }
            else
            {
                //יוצר וולנטיר חדש ומכניס אותו למערך. הבעיה שמדובר בעדכון וולנטיר, ואז הוא מתריע שהוולנטיר קיים כבר .
                Volunteer updatedVolunteer = createNewVolunteer(id); // מתודה לעדכון אובייקט קיים
                s_dalVolunteer.Update(updatedVolunteer);
                Console.WriteLine("Volunteer updated successfully!");
            }

            Console.WriteLine("Enter new ID");
            string idInput = Console.ReadLine();
            int Id;
            string.IsNullOrWhiteSpace(idInput) ? Id = doesExist.Id : Id = int.Parse(idInput);
        
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
            Console.WriteLine(volunteer);
            //Console.WriteLine($"ID: {volunteer.Id}, Name: {volunteer.Name}, Phone: {volunteer.Phone}, Email: {volunteer.Email}, Role: {volunteer.Role}, Active: {volunteer.IsActive}, Distance Kind: {volunteer.DistanceKind}, Address: {volunteer.Address ?? "N/A"}");
        }
    }
    //הצגת מתנדב לפי מספר מזהה
    private static void ReadVolunteerlById()
    {
        try
        {
            int id = getInt("Enter volunteer ID: ");
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
            id = getInt("Enter volunteer ID: ");
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

    #region  Assignment methods
    //Assignment התפריט של
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
                        ReadAssignmentById(); // מתודה להצגת אובייקט לפי מזהה
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
            int id = getInt("Enter the assignment ID to delete: ");
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
            int id = getInt("enter the ID to update");
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
            Console.WriteLine(assignment);
         //Console.WriteLine($"ID: {assignment.Id}, Volunteer ID: {assignment.VolunteerId}, Call ID: {assignment.CallId}");
        }

    }
    private static void ReadAssignmentById()
    {
        try
        {
            int id = getInt("Enter Assignment ID: ");
            Assignment? assignment = s_dalAssignment.Read(id);
            if (assignment == null)
            {
                Console.WriteLine("An assignment with this ID does not exist.");
                return;
            }
            //Console.WriteLine($"ID: {assignment.Id}, Volunteer ID: {assignment.VolunteerId}, Call ID: {assignment.CallId}");
            Console.WriteLine(assignment);
        }

        //else
        //{
        //    Console.WriteLine("Invalid ID. Please enter a valid integer.");
        //}
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
    private static Assignment? CreateNewAssignment(int id = 0)
    {
        try
        {
            int volunteerId = getInt("Enter volunteer ID: ");
            //Volunteer? isValidVolunteerId = s_dalVolunteer.Read(volunteerId);
            //if (isValidVolunteerId == null)
            //{
            //    throw new Exception("A volunteer with this ID does not exist.");
            //}
            int callID = getInt("Enter call ID: ");
            //Call? isValidCallId = s_dalCall.Read(callID);
            //if (isValidCallId == null)
            //{
            //    throw new Exception("A call with this ID does not exist.");
            //}

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
    public static void ShowConfigurationSubMenu()
    {
        bool exit = false;

        while (!exit)
        {
            Console.WriteLine("===== Configuration Menu =====");
            Console.WriteLine("1. Exit the submenu");
            Console.WriteLine("2. Advance system clock by 1 minute");
            Console.WriteLine("3. Advance system clock by 1 hour");
            Console.WriteLine("4. Show current system clock value");
            Console.WriteLine("5. Set a new value for a configuration variable");
            Console.WriteLine("6. Show the current value of a configuration variable");
            Console.WriteLine("7. Reset all configuration values");
            Console.Write("Choose an option: ");
            string? input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    exit = true;
                    break;
                case "2":
                    s_dalConfig.Clock = s_dalConfig.Clock.AddMinutes(1);
                    Console.WriteLine("System clock advanced by 1 minute.");
                    break;
                case "3":
                    s_dalConfig.Clock = s_dalConfig.Clock.AddHours(1);
                    Console.WriteLine("System clock advanced by 1 hour.");
                    break;
                case "4":
                    Console.WriteLine(s_dalConfig.Clock);
                    break;
                case "5":
                    SetNextCallId();
                    break;
                case "6":
                    ShowConfigValue();
                    break;
                case "7":
                    s_dalConfig.Reset();
                    Console.WriteLine("All configuration values have been reset.");
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }

            //Console.WriteLine("לחץ על מקש כלשהו להמשיך...");
            //Console.ReadKey();
        }
    }
    //לא ממומש
    private static void SetNextCallId()
    {
        Console.Write("Enter the value of the next ID: ");
        int? variable = Console.ReadLine();

        Console.Write("Enter a new value: ");
        int? newValue = int.Parse(Console.ReadLine());
        
        // Logic to set the new value for the requested variable
        Console.WriteLine($"The value of has been updated to '{newValue}'.");
    }
    //לא ממומש
    private static void ShowConfigValue()
    {
        Console.Write("Enter the name of the configuration variable to display: ");
        string? variable = Console.ReadLine();

        // Logic to display the current value of the requested variable
        Console.WriteLine($"The current value of '{variable}': <Not implemented>.");
    }
    

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