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

            string? choice = Console.ReadLine();

            try
            {
                if (Enum.TryParse(choice, out MainMenuOption selectedOption))
                {
                    switch (selectedOption)
                    {
                        case MainMenuOption.DisplaySubmenuForEntityCall:
                            ShowEntityCall(); // מתודה להצגת תת-תפריט עבור ישות Call
                            break;
                        case MainMenuOption.DisplaySubmenuForEntityAssignment:
                            ShowEntityAssignment(); // מתודה להצגת תת-תפריט עבור ישות Assignment
                            break;
                        case MainMenuOption.DisplaySubmenuForEntityVolunteer:
                            ShowEntityVolunteer(); // מתודה להצגת תת-תפריט עבור ישות Volunteer
                            break;
                        case MainMenuOption.InitializeData:
                            InitializeAll();
                            break;
                        case MainMenuOption.DisplayAllData:
                            DisplayAllData(); // מתודה שמציגה את כל הנתונים בבסיס הנתונים
                            break;
                        case MainMenuOption.DisplayConfigurationSubMenu:
                            ShowConfigurationSubMenu(); // מתודה להצגת תפריט ישות תצורה
                            break;
                        case MainMenuOption.ResetDatabase:
                            ResetDatabase(); // מתודה שמאפסת את בסיס הנתונים ואת נתוני התצורה
                            break;
                        case MainMenuOption.Exit:
                            exit = true;
                            Console.WriteLine("Exiting main menu");
                            break;
                        default:
                            Console.WriteLine("Invalid option, please try again");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input, please enter a valid option.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


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

            string? choice = Console.ReadLine();

            try
            {
                if (Enum.TryParse(choice, out CallMenuOption selectedOption))
                {
                    switch (selectedOption)
                    {
                        case CallMenuOption.Create:
                            Call item = CreateNewCall(); // מתודה להוספת אובייקט חדש
                            s_dalCall.Create(item);
                            Console.WriteLine("Call object created successfully.");
                            break;
                        case CallMenuOption.ReadById:
                            ReadByCallId(); // מתודה להצגת אובייקט לפי מזהה
                            break;
                        case CallMenuOption.ReadAll:
                            ReadAllCalls();
                            break;
                        case CallMenuOption.Update:
                            UpdateCall();
                            break;
                        case CallMenuOption.Delete:
                            DeleteCall(); // מתודה למחיקת אובייקט לפי מזהה
                            break;
                        case CallMenuOption.DeleteAll:
                            s_dalCall.DeleteAll(); // מתודה למחיקת כל האובייקטים
                            Console.WriteLine("All call objects deleted successfully.");
                            break;
                        case CallMenuOption.Exit:
                            exit = true;
                            Console.WriteLine("Exiting submenu...");
                            break;
                        default:
                            Console.WriteLine("Invalid option. Try again.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid option.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
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
                throw new Exception("Invalid open time format.");
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
            return new Call(0, (CallType)callType, address, latitude, longitude, openTime, description, maxCallTime);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null;
        }
    }
    static void UpdateCall()
    {
        try
        {
            int id = getInt("Enter the ID of the Call");
            Call? callToUpdate = s_dalCall.Read(id);
            if (callToUpdate == null)
            {
                throw new Exception("A call with this ID does not exist");
            }
            Console.WriteLine("Current values for this call: " + callToUpdate);

            Console.Write("Enter the call type (Technical, Food, etc.): ");
            string? callTypeInput = Console.ReadLine();
            if (string.IsNullOrEmpty(callTypeInput) || !Enum.TryParse(typeof(CallType), callTypeInput, true, out var callType) || callType == null)
            {
                callType = callToUpdate.GetType();
            }

            Console.Write("Enter the address: ");
            string? address = Console.ReadLine();
            address = string.IsNullOrEmpty(address) ? callToUpdate.FullAddress : address;

            Console.Write("Enter the latitude: ");
            string? latitudeInput = Console.ReadLine();
            double latitude = string.IsNullOrEmpty(latitudeInput) ? callToUpdate.Latitude : double.TryParse(latitudeInput, out var lat) ? lat : callToUpdate.Latitude;

            Console.Write("Enter the longitude: ");
            string? longitudeInput = Console.ReadLine();
            double longitude = string.IsNullOrEmpty(longitudeInput) ? callToUpdate.Longitude : double.TryParse(longitudeInput, out var lon) ? lon : callToUpdate.Longitude;

            Console.Write("Enter the open time (format: yyyy-MM-dd HH:mm:ss): ");
            string? openTimeInput = Console.ReadLine();
            DateTime openTime = string.IsNullOrEmpty(openTimeInput) || !DateTime.TryParse(openTimeInput, out var parsedOpenTime)
                ? callToUpdate.OpenTime
                : parsedOpenTime;

            Console.Write("Enter a description (optional): ");
            string? description = Console.ReadLine();
            description = string.IsNullOrEmpty(description) ? callToUpdate.Description : description;

            Console.Write("Enter the maximum end time (format: yyyy-MM-dd HH:mm:ss) or leave blank if none: ");
            string? maxCallTimeInput = Console.ReadLine();
            DateTime? maxCallTime = string.IsNullOrEmpty(maxCallTimeInput)
                ? callToUpdate.MaxCallTime
                : DateTime.TryParse(maxCallTimeInput, out var parsedMaxCallTime)
                    ? parsedMaxCallTime
                    : callToUpdate.MaxCallTime;

            // Create a new Call object and return it
            Call updatedCall = new Call(id, (CallType)callType, address, latitude, longitude, openTime, description, maxCallTime);
            s_dalCall.Update(updatedCall);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
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
            Console.WriteLine("=== Menu for Volunteer Entity ===");
            Console.WriteLine("1. Add a new volunteer (Create)");
            Console.WriteLine("2. Display volunteer by ID (Read)");
            Console.WriteLine("3. Display all volunteers (ReadAll)");
            Console.WriteLine("4. Update an existing volunteer (Update)");
            Console.WriteLine("5. Delete a volunteer from the list (Delete)");
            Console.WriteLine("6. Delete all volunteers from the list (DeleteAll)");
            Console.WriteLine("0. Exit");
            Console.Write("Choose an option: ");

            string? choice = Console.ReadLine();

            try
            {
                if (Enum.TryParse(choice, out VolunteerMenuOption selectedOption))
                {
                    switch (selectedOption)
                    {
                        case VolunteerMenuOption.Create:
                            createNewVolunteer(); // מתודה להוספת אובייקט חדש
                            break;
                        case VolunteerMenuOption.ReadById:
                            ReadVolunteerlById(); // מתודה להצגת אובייקט לפי מזהה
                            break;
                        case VolunteerMenuOption.ReadAll:
                            ReadAllVolunteers(); // מתודה להצגת כל האובייקטים
                            break;
                        case VolunteerMenuOption.Update:
                            UpdateVolunteer(); // מתודה לעדכון אובייקט קיים
                            break;
                        case VolunteerMenuOption.Delete:
                            DeleteVolunteerById(); // מתודה למחיקת אובייקט לפי מזהה
                            break;
                        case VolunteerMenuOption.DeleteAll:
                            DeleteAllVolunteers(); // מתודה למחיקת כל האובייקטים
                            break;
                        case VolunteerMenuOption.Exit:
                            exit = true;
                            Console.WriteLine("Exiting menu...");
                            break;
                        default:
                            Console.WriteLine("Invalid option, try again.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid option.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

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
            int id = getInt("Enter the ID of the volunteer to update: ");
            Volunteer? volunteerToUpdate = s_dalVolunteer.Read(id);
            if (volunteerToUpdate == null)
            {
                throw new Exception("A volunteer with this ID does not exist.");
            }
            Console.WriteLine(volunteerToUpdate);

            Console.Write("Enter volunteer name: ");
            string? name = Console.ReadLine();
            name = string.IsNullOrWhiteSpace(name) ? volunteerToUpdate.Name : name;

            Console.Write("Enter phone number: ");
            string? phone = Console.ReadLine();
            phone = string.IsNullOrWhiteSpace(phone) ? volunteerToUpdate.Phone : phone;

            Console.Write("Enter email address: ");
            string? email = Console.ReadLine();
            email = string.IsNullOrWhiteSpace(email) ? volunteerToUpdate.Email : email;

            Console.WriteLine("Choose volunteer role [0 for Regular, 1 for Manager]: ");
            VolunteerRole role = volunteerToUpdate.Role;
            if (Enum.TryParse<VolunteerRole>(Console.ReadLine(), out var parsedRole))
            {
                role = parsedRole;
            }

            Console.WriteLine("Is the volunteer active? [yes/no]: ");
            string? isActiveInput = Console.ReadLine();
            bool isActive = isActiveInput == null
                ? volunteerToUpdate.IsActive // אם לא הוזן כלום, שמור את הערך הנוכחי
                : isActiveInput.ToLower() == "yes"
                    ? true // עדכן ל-"פעיל" אם הוזן "yes"
                    : isActiveInput.ToLower() == "no"
                        ? false // עדכן ל-"לא פעיל" אם הוזן "no"
                        : volunteerToUpdate.IsActive; // שמור את הערך הנוכחי אם הקלט לא חוקי

            Console.WriteLine("Choose distance kind [0 for Aerial, 1 for Ground]: ");
            DistanceKind distanceKind = (DistanceKind)volunteerToUpdate.DistanceKind;
            if (Enum.TryParse<DistanceKind>(Console.ReadLine(), out var parsedDistanceKind))
            {
                distanceKind = parsedDistanceKind;
            }

            Console.Write("Enter address: ");
            string? address = Console.ReadLine();
            address = string.IsNullOrWhiteSpace(address) ? volunteerToUpdate.Address : address;

            Volunteer updatedVolunteer = new Volunteer(
                id,
                name,
                phone,
                email,
                role,
                isActive,
                distanceKind,
                address
            );

            s_dalVolunteer.Update(updatedVolunteer);
            Console.WriteLine("Volunteer updated successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
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
            Console.WriteLine("5. Delete an assignment from the list (Delete)");
            Console.WriteLine("6. Delete all assignments from the list (DeleteAll)");
            Console.WriteLine("0. Exit");
            Console.Write("Choose an option: ");

            string? choice = Console.ReadLine();

            try
            {
                if (Enum.TryParse(choice, out AssignmentMenuOption selectedOption))
                {
                    switch (selectedOption)
                    {
                        case AssignmentMenuOption.Create:
                            CreateNewAssignment(); // מתודה להוספת אובייקט חדש
                            break;
                        case AssignmentMenuOption.ReadById:
                            ReadAssignmentById(); // מתודה להצגת אובייקט לפי מזהה
                            break;
                        case AssignmentMenuOption.ReadAll:
                            ReadAllAssignments(); // מתודה להצגת כל האובייקטים
                            break;
                        case AssignmentMenuOption.Update:
                            UpdateAssignment(); // מתודה לעדכון אובייקט קיים
                            break;
                        case AssignmentMenuOption.Delete:
                            DeleteAssignmentById(); // מתודה למחיקת אובייקט לפי מזהה
                            break;
                        case AssignmentMenuOption.DeleteAll:
                            DeleteAllAssignments(); // מתודה למחיקת כל האובייקטים
                            break;
                        case AssignmentMenuOption.Exit:
                            exit = true;
                            Console.WriteLine("Exiting menu...");
                            break;
                        default:
                            Console.WriteLine("Invalid option, try again.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid option.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
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
            // בקש מזהה של המשימה לעדכון
            int id = getInt("Enter the ID of the Assignment to update: ");
            Assignment? assignmentToUpdate = s_dalAssignment.Read(id);

            // בדיקה אם המשימה קיימת
            if (assignmentToUpdate == null)
            {
                throw new Exception("An assignment with this ID does not exist.");
            }

            Console.WriteLine(assignmentToUpdate);

            //  Id של מתנדב
            Console.WriteLine("Enter volunteer ID: ");
            string? volunteerIdInput = Console.ReadLine();
            int volunteerId = string.IsNullOrEmpty(volunteerIdInput)
                ? assignmentToUpdate.Id
                : int.TryParse(volunteerIdInput, out var parsedVolunteerId)
                    ? parsedVolunteerId
                    : assignmentToUpdate.Id;

            //  ID של קריאה
            Console.WriteLine("Enter call ID: ");
            string? callIdInput = Console.ReadLine();
            int callId = string.IsNullOrEmpty(callIdInput)
                ? assignmentToUpdate.CallId
                : int.TryParse(callIdInput, out var parsedCallId)
                    ? parsedCallId
                    : assignmentToUpdate.CallId;

            // בקש זמן תחילת טיפול
            Console.WriteLine("Enter start treatment time: ");
            string? startTreatmentInput = Console.ReadLine();
            DateTime startTreatment = string.IsNullOrEmpty(startTreatmentInput) || !DateTime.TryParse(startTreatmentInput, out var parsedStartTreatment)
                ? assignmentToUpdate.StartTreatment
                : parsedStartTreatment;

            // בקש זמן סיום טיפול
            Console.WriteLine("Enter end treatment time: ");
            string? endTreatmentInput = Console.ReadLine();
            DateTime? endTreatment = string.IsNullOrEmpty(endTreatmentInput)
                ? assignmentToUpdate.EndTreatment
                : DateTime.TryParse(endTreatmentInput, out var parsedEndTreatment)
                    ? parsedEndTreatment
                    : assignmentToUpdate.EndTreatment;

            // בקש סוג טיפול
            Console.WriteLine("Enter treatment type: ");
            string? treatmentTypeInput = Console.ReadLine();
            TreatmentType? treatmentType = string.IsNullOrEmpty(treatmentTypeInput) || !Enum.TryParse<TreatmentType>(treatmentTypeInput, true, out var parsedTreatmentType)
                ? assignmentToUpdate.TreatmentType
                : parsedTreatmentType;


            // יצירת מופע מעודכן
            Assignment updatedAssignment = new(
                id,
                volunteerId,
                callId,
                startTreatment,
                endTreatment,
                treatmentType
            );

            // עדכון בבסיס הנתונים
            s_dalAssignment.Update(updatedAssignment);

            Console.WriteLine("Assignment updated successfully.");
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
    private static void CreateNewAssignment()
    {
        try
        {
            // בקש מזהה מתנדב
            int volunteerId = getInt("Enter volunteer ID: ");

            // בקש מזהה קריאה
            int callId = getInt("Enter call ID: ");

            // בקש זמן תחילת טיפול
            Console.WriteLine("Enter start treatment time (format: yyyy-MM-dd HH:mm:ss): ");
            string? startTreatmentInput = Console.ReadLine();
            if (string.IsNullOrEmpty(startTreatmentInput) || !DateTime.TryParse(startTreatmentInput, out var startTreatment))
            {
                throw new Exception("Start treatment time is mandatory and must be in a valid format.");
            }

            // בקש זמן סיום טיפול
            Console.WriteLine("Enter end treatment time (format: yyyy-MM-dd HH:mm:ss) or leave blank for none: ");
            string? endTreatmentInput = Console.ReadLine();
            DateTime? endTreatment = string.IsNullOrEmpty(endTreatmentInput)
                ? null
                : DateTime.TryParse(endTreatmentInput, out var parsedEndTreatment)
                    ? parsedEndTreatment
                    : throw new Exception("Invalid end treatment time format.");

            // בקש סוג טיפול
            Console.WriteLine("Enter treatment type (Technical, Food, etc.) or leave blank for none: ");
            string? treatmentTypeInput = Console.ReadLine();
            TreatmentType? treatmentType = string.IsNullOrEmpty(treatmentTypeInput) || !Enum.TryParse<TreatmentType>(treatmentTypeInput, true, out var parsedTreatmentType)
                ? null
                : parsedTreatmentType;

            // יצירת המופע
            Assignment newAssignment = new(
                0,
                volunteerId,
                callId,
                startTreatment,
                endTreatment,
                treatmentType
            );
            s_dalAssignment.Create(newAssignment);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
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
            Console.WriteLine("4. Advance system clock by 1 day");
            Console.WriteLine("5. Advance system clock by 1 year");
            Console.WriteLine("6. Show current system clock value");
            Console.WriteLine("7. Set a new value for a configuration variable");
            Console.WriteLine("8. Show the current value of a configuration variable");
            Console.WriteLine("9. Reset all configuration values");
            Console.Write("Choose an option: ");
            string? input = Console.ReadLine();

            if (Enum.TryParse(input, out ConfigurationOption option))
            {
                switch (option)
                {
                    case ConfigurationOption.ExitSubmenu:
                        exit = true;
                        break;
                    case ConfigurationOption.AdvanceClockByMinute:
                        s_dalConfig.Clock = s_dalConfig.Clock.AddMinutes(1);
                        Console.WriteLine("System clock advanced by 1 minute.");
                        break;
                    case ConfigurationOption.AdvanceClockByHour:
                        s_dalConfig.Clock = s_dalConfig.Clock.AddHours(1);
                        Console.WriteLine("System clock advanced by 1 hour.");
                        break;
                    case ConfigurationOption.AdvanceClockByDay:
                        s_dalConfig.Clock = s_dalConfig.Clock.AddDays(1);
                        Console.WriteLine("System clock advanced by 1 day.");
                        break;
                    case ConfigurationOption.AdvanceClockByYear:
                        s_dalConfig.Clock = s_dalConfig.Clock.AddYears(1);
                        Console.WriteLine("System clock advanced by 1 year.");
                        break;
                    case ConfigurationOption.ShowCurrentClock:
                        Console.WriteLine(s_dalConfig.Clock);
                        break;
                    case ConfigurationOption.SetConfigurationVariable:
                        SetNextCallId();
                        break;
                    case ConfigurationOption.ShowConfigurationValue:
                        SetNextAssignmentId();
                        break;
                    case ConfigurationOption.ResetAllConfigurations:
                        s_dalConfig.Reset();
                        Console.WriteLine("All configuration values have been reset.");
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }
    }
    private static void SetNextCallId()
    {
        int value = getInt("Enter the value for the id of the next call")!;
        s_dalConfig.SetNextCallId(value);
    }
    private static void SetNextAssignmentId()
    {
        int value = getInt("Enter the value for the id of the next assignment")!;
        s_dalConfig.SetNextAssignmentId(value);
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
    }
}