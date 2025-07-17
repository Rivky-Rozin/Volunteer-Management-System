namespace BlTest;

class Program
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    static void Main()
    {
        try
        {
            while (true)
            {
                Console.WriteLine("\n--- BL Test System ---");
                Console.WriteLine("1. Administration");
                Console.WriteLine("2. Volunteers");
                Console.WriteLine("3. Calls");
                Console.WriteLine("0. Exit");
                Console.Write("Choose an option: ");

                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    switch (choice)
                    {
                        case 1:
                            AdminMenu();
                            break;
                        case 2:
                            VolunteerMenu();
                            break;
                        case 3:
                            CallMenu();
                            break;
                        case 0:
                            return;
                        default:
                            Console.WriteLine("Invalid choice. Try again.");
                            break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            HandleException(ex);
       }
    }


    static void AdminMenu()
    {
        while (true)
        {
            Console.WriteLine("\n--- Administration ---");
            Console.WriteLine("1. Reset Database");
            Console.WriteLine("2. Initialize Database");
            Console.WriteLine("3. Advance Clock");
            Console.WriteLine("4. Show Clock");
            Console.WriteLine("5. Get Risk Time Range");
            Console.WriteLine("6. Set Risk Time Range");
            Console.WriteLine("7. Get Treatment Time Range");
            Console.WriteLine("8. Set Treatment Time Range");
            Console.WriteLine("0. Back");
            Console.Write("Choose an option: ");

            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                Console.WriteLine("Invalid input. Please enter a number.");
                continue;
            }

            try
            {
                switch (choice)
                {
                    case 1:
                        s_bl.Admin.ResetDatabase();
                        Console.WriteLine("Database reset successfully");
                        break;
                    case 2:
                        s_bl.Admin.InitializeDatabase();
                        Console.WriteLine("Database initialized successfully");
                        break;
                    case 3:
                        Console.Write("Enter time unit (Minute, Hour, Day, Month, Year): ");
                        if (Enum.TryParse(Console.ReadLine(), true, out BO.TimeUnit timeUnit))
                        {
                            s_bl.Admin.AdvanceTime(timeUnit);
                            Console.WriteLine("System clock advanced.");
                        }
                        else
                        {
                            throw new FormatException("Invalid time unit. Please enter: Minute, Hour, Day, Month, Year.");
                        }
                        break;
                    case 4:
                        Console.WriteLine($"Current System Clock: {s_bl.Admin.GetCurrentTime()}");
                        break;
                    case 5:
                        Console.WriteLine($"Current Risk Time Range: {s_bl.Admin.GetRiskTimeSpan()}");
                        break;
                    case 6:
                        Console.Write("Enter new risk time range (hh:mm:ss): ");
                        if (TimeSpan.TryParse(Console.ReadLine(), out TimeSpan timeRange))
                        {
                            s_bl.Admin.SetRiskTimeSpan(timeRange);
                            Console.WriteLine("Risk time range updated.");
                        }
                        else
                        {
                            throw new FormatException("Invalid time format. Please use hh:mm:ss.");
                        }
                        break;
                    case 7:
                        Console.WriteLine($"Current treatment time  Range: {s_bl.Admin.GetTreatmentTime()}");
                        break;
                    case 8:
                        Console.Write("Enter new risk time range (hh:mm:ss): ");
                        if (TimeSpan.TryParse(Console.ReadLine(), out TimeSpan treatmentTime))
                        {
                            s_bl.Admin.SetTreatmentTime(treatmentTime);
                            Console.WriteLine("Treatment time range updated.");
                        }
                        else
                        {
                            throw new FormatException("Invalid time format. Please use hh:mm:ss.");
                        }
                        break;
                    case 0:
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        break;
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }
    }

    static void VolunteerMenu()
    {
        while (true)
        {
            Console.WriteLine("\n--- Volunteer Management ---");
            Console.WriteLine("1. Login");
            Console.WriteLine("2. List Volunteers");
            Console.WriteLine("3. Get Filter/Sort volunteer");
            Console.WriteLine("4. Read Volunteer by ID");
            Console.WriteLine("5. Add Volunteer");
            Console.WriteLine("6. Remove Volunteer");
            Console.WriteLine("7. UpDate Volunteer");
            Console.WriteLine("0. Back");
            Console.Write("Choose an option: ");

            if (!int.TryParse(Console.ReadLine(), out int choice))
                Console.WriteLine("Invalid input. Please enter a number.");
            //throw new FormatException("The volunteer menu choice is not valid.");
            switch (choice)
            {
                case 1:
                    try
                    {
                        Console.WriteLine("Please log in.");
                        Console.Write("Username: ");
                        string username = Console.ReadLine()!;

                        Console.Write("Enter Password (must be at least 8 characters, contain upper and lower case letters, a digit, and a special character): ");
                        string password = Console.ReadLine()!;

                        BO.VolunteerRole userRole = s_bl.Volunteer.Login(username, password);
                        Console.WriteLine($"Login successful! Your role is: {userRole}");
                    }
                    catch (Exception ex)
                    {
                        HandleException(ex);
                    }
                    break;
                case 2:
                    try
                    {
                        foreach (var volunteer in s_bl.Volunteer.GetVolunteersList())
                            Console.WriteLine(volunteer);
                    }
                    catch (Exception ex)
                    {
                        HandleException(ex);
                    }
                    break;
                case 3:
                    try
                    {
                        bool? isActive;
                        BO.VolunteerInListEnum? sortBy;
                        GetVolunteerFilterAndSortCriteria(out isActive, out sortBy);
                        var volunteersList = s_bl.Volunteer.GetVolunteersList(isActive, sortBy);
                        if (volunteersList != null)
                            foreach (var volunteer in volunteersList)
                                Console.WriteLine(volunteer);
                        else
                            Console.WriteLine("No volunteers found matching the criteria.");
                    }
                    catch (Exception ex)
                    {
                        HandleException(ex);
                    }
                    break;
                case 4:
                    try
                    {
                        Console.Write("Enter Volunteer ID: ");
                        string volunteerId = Console.ReadLine();
                        var volunteer = s_bl.Volunteer.GetVolunteerDetails(volunteerId);
                        Console.WriteLine(volunteer);

                    }
                    catch (Exception ex)
                    {
                        HandleException(ex);
                    }
                    break;
                case 5:
                    try
                    {
                        BO.Volunteer volunteer;
                        Console.Write("הזן ת.ז.: ");
                        if (int.TryParse(Console.ReadLine(), out int id))
                        {
                            volunteer = new BO.Volunteer() { Id = id };
                        }

                        else
                        {
                            Console.WriteLine("❗ ת.ז. לא חוקי");
                            break;
                        }

                        Console.Write("הזן שם: ");
                        volunteer.Name = Console.ReadLine() ?? string.Empty;

                        Console.Write("הזן טלפון: ");
                        volunteer.Phone = Console.ReadLine() ?? string.Empty;

                        Console.Write("הזן דוא\"ל: ");
                        volunteer.Email = Console.ReadLine() ?? string.Empty;

                        Console.Write("הזן סיסמה: ");
                        volunteer.Password = Console.ReadLine();

                        Console.Write("הזן כתובת: ");
                        volunteer.Address = Console.ReadLine();

                        Console.Write("הזן קו רוחב (Latitude) או הקש Enter לדלג: ");
                        string? latInput = Console.ReadLine();
                        if (!string.IsNullOrEmpty(latInput) && double.TryParse(latInput, out double lat))
                            volunteer.Latitude = lat;

                        Console.Write("הזן קו אורך (Longitude) או הקש Enter לדלג: ");
                        string? lonInput = Console.ReadLine();
                        if (!string.IsNullOrEmpty(lonInput) && double.TryParse(lonInput, out double lon))
                            volunteer.Longitude = lon;

                        Console.Write("בחר תפקיד (0-מתנדב, 1-מנהל): ");
                        if (int.TryParse(Console.ReadLine(), out int role) && Enum.IsDefined(typeof(BO.VolunteerRole), role))
                            volunteer.Role = (BO.VolunteerRole)role;
                        else
                        {
                            volunteer.Role = BO.VolunteerRole.Regular;

                        }
                        //todo ברירת מחדל היא false
                        Console.Write("IsActive? (true/false):(other input is false) ");
                        if (bool.TryParse(Console.ReadLine(), out bool isActive))
                            volunteer.IsActive = isActive;
                        else
                        {
                            volunteer.IsActive = false;
                        }

                        Console.Write("הזן מרחק מקסימלי או הקש Enter לדלג: ");
                        string? maxDistInput = Console.ReadLine();
                        if (!string.IsNullOrEmpty(maxDistInput) && double.TryParse(maxDistInput, out double maxDist))
                            volunteer.MaxDistance = maxDist;
                        // todo לעשות ברירת מחדל שיהיה מרחק כלשהו
                        Console.Write("בחר סוג מרחק (0-קווי, 1-נסיעה) או הקש Enter לדלג: ");
                        string? distKindInput = Console.ReadLine();
                        if (!string.IsNullOrEmpty(distKindInput) && int.TryParse(distKindInput, out int distKind) &&
                            Enum.IsDefined(typeof(BO.DistanceKind), distKind))
                            volunteer.DistanceKind = (BO.DistanceKind)distKind;
                        else
                        {
                            volunteer.DistanceKind = BO.DistanceKind.Aerial;
                        }
                            s_bl.Volunteer.AddVolunteer(volunteer);
                        Console.WriteLine("Volunteer added successfully");
                    }
                    catch (Exception ex)
                    {
                        HandleException(ex);
                    }
                    break;
                case 6:
                    try
                    {
                        Console.Write("Enter Volunteer ID: ");
                        string? idToDelete = Console.ReadLine();
                        s_bl.Volunteer.DeleteVolunteer(idToDelete);
                        Console.WriteLine("Volunteer deleted successfully");
                    }
                    catch (Exception ex)
                    {
                        HandleException(ex);
                    }
                    break;
                case 7:
                    UpDateVolunteer();
                    break;
                case 0:
                    return;
                default:
                    Console.WriteLine("Invalid choice. Try again.");
                    break;
            }
        }
    }
    public static void GetVolunteerFilterAndSortCriteria(out bool? isActive, out BO.VolunteerInListEnum? sortBy)
    {
        isActive = null;
        sortBy = null;

        try
        {

            Console.WriteLine("Is the volunteer active? (yes/no or leave blank for null): ");
            string activeInput = Console.ReadLine();

            if (!string.IsNullOrEmpty(activeInput))
            {
                if (activeInput.Equals("yes", StringComparison.OrdinalIgnoreCase))
                    isActive = true;
                else if (activeInput.Equals("no", StringComparison.OrdinalIgnoreCase))
                    isActive = false;
                else
                    Console.WriteLine("Invalid input for active status. Defaulting to null.");
            }

            Console.WriteLine("Choose how to sort the volunteers by: ");
            Console.WriteLine("1. By FullName");
            Console.WriteLine("2. By HandledCallsCount");
            Console.WriteLine("3. By IsActive");
            Console.WriteLine("4. By CallInProgressId");
            Console.WriteLine("Select sorting option by number: ");
            string sortInput = Console.ReadLine();

            if (int.TryParse(sortInput, out int sortOption))
            {
                switch (sortOption)
                {
                    case 1:
                        sortBy = BO.VolunteerInListEnum.Name;
                        break;
                    case 2:
                        sortBy = BO.VolunteerInListEnum.HandledCallsCount;
                        break;
                    case 3:
                        sortBy = BO.VolunteerInListEnum.IsActive;
                        break;
                    case 4:
                        sortBy = BO.VolunteerInListEnum.CallInProgressId;
                        break;
                    default:
                        Console.WriteLine("Invalid selection. Defaulting to sorting by ID.");
                        break;
                }
            }
            else
            {
                //todo לבדוק אם מותר לזרוק שגיאה
                throw new FormatException("Invalid input for sorting option. Defaulting to sorting by ID.");
            }
        }
        catch (BO.BlGeneralException ex)
        {
            HandleException(ex);
        
        }
    }

   
    static void UpDateVolunteer()
    {
        try
        {
            Console.Write("Enter requester ID: ");
            string requesterIdStr = Console.ReadLine();
            if (!int.TryParse(requesterIdStr, out int requesterId))
            {
                Console.WriteLine("⚠️ Invalid input. Update aborted.");
                return;
            }

            // Get existing volunteer
            BO.Volunteer volunteer = s_bl.Volunteer.GetVolunteerDetails(requesterIdStr);
            Console.WriteLine("Press Enter to keep current value.");

            Console.Write($"Name ({volunteer.Name}): ");
            string nameInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nameInput))
                volunteer.Name = nameInput;

            Console.Write($"Phone Number ({volunteer.Phone}): ");
            string phoneInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(phoneInput))
                volunteer.Phone = phoneInput;

            Console.Write($"Email ({volunteer.Email}): ");
            string emailInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(emailInput))
                volunteer.Email = emailInput;

            Console.Write($"IsActive? ({volunteer.IsActive}) (true/false): ");
            string isActiveInput = Console.ReadLine();
            if (bool.TryParse(isActiveInput, out bool isActiveParsed))
                volunteer.IsActive = isActiveParsed;

            Console.Write($"Role ({volunteer.Role}): ");
            string roleInput = Console.ReadLine();
            if (Enum.TryParse(roleInput, true, out BO.VolunteerRole roleParsed))
                volunteer.Role = roleParsed;

            Console.Write($"Password ({volunteer.Password}): ");
            string passwordInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(passwordInput))
                volunteer.Password = passwordInput;

            Console.Write($"Address ({volunteer.Address}): ");
            string addressInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(addressInput))
                volunteer.Address = addressInput;

            Console.Write($"Latitude ({volunteer.Latitude}): ");
            string latInput = Console.ReadLine();
            if (double.TryParse(latInput, out double latParsed))
                volunteer.Latitude = latParsed;

            Console.Write($"Longitude ({volunteer.Longitude}): ");
            string lonInput = Console.ReadLine();
            if (double.TryParse(lonInput, out double lonParsed))
                volunteer.Longitude = lonParsed;

            Console.Write($"Max Distance ({volunteer.MaxDistance}): ");
            string maxDistInput = Console.ReadLine();
            if (double.TryParse(maxDistInput, out double distParsed))
                volunteer.MaxDistance = distParsed;

            Console.Write($"Distance Type ({volunteer.DistanceKind}) (Air/Drive/Walk): ");
            string distKindInput = Console.ReadLine();
            if (Enum.TryParse(distKindInput, true, out BO.DistanceKind distKindParsed))
                volunteer.DistanceKind = distKindParsed;

            s_bl.Volunteer.UpdateVolunteer(requesterIdStr, volunteer);
            Console.WriteLine("✅ Volunteer updated successfully.");
        }
        catch (Exception ex)
        {
            HandleException(ex);
        }
    }


    static void CallMenu()
    {
        try
        {

            while (true)
            {
                Console.WriteLine("\n--- Call Management ---");
                Console.WriteLine("1. Get call quantities by status");
                Console.WriteLine("2. Get Closed Calls Handled By Volunteer");
                Console.WriteLine("3. Show All Calls");
                Console.WriteLine("4. Read Call by ID");
                Console.WriteLine("5. Add Call");
                Console.WriteLine("6. Remove Call");
                Console.WriteLine("7. Update Call");
                Console.WriteLine("8. Get Open Calls For Volunteer");
                Console.WriteLine("9. Mark Call As Canceled");
                Console.WriteLine("10. Mark Call As Completed");
                Console.WriteLine("11. Select Call For Treatment");
                Console.WriteLine("0. Back");
                Console.Write("Choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                    //todo לבדוק מה אפשר לעשות פה במקום לזרוק שגיאה כי אסור לזרוק שגיאות בתוכנית הראשית
                    // אולי כן מותר... לבדוק את זה
                    throw new FormatException("The call menu choice is not valid.");

                switch (choice)
                {
                    case 1:
                        try
                        {
                            IEnumerable<int> callQuantities = s_bl.Call.GetCallStatusCounts();

                            Console.WriteLine("Call quantities by status:");

                            int index = 0;
                            foreach (BO.CallStatus status in Enum.GetValues(typeof(BO.CallStatus)))
                            {
                                Console.WriteLine($"{status}: {callQuantities.ElementAt(index)}");
                                index++;
                            }
                        }
                        catch (Exception ex)
                        {
                            HandleException(ex);
                        }
                        break;
                    case 2:
                        try
                        {
                            Console.Write("Enter Volunteer ID: ");
                            if (int.TryParse(Console.ReadLine(), out int volunteerId))
                            {
                                Console.WriteLine("Enter Call Type filter (Emergency,Equipment,Doctor,Training) or press Enter to skip:");
                                string? callTypeInput = Console.ReadLine();
                                BO.CallType? callTypeFilter = Enum.TryParse(callTypeInput, out BO.CallType parsedCallType) ? parsedCallType : null;

                                Console.WriteLine("Enter Sort Field (Completed, SelfCancelled, ManagerCancelled, Expired) or press Enter to skip:");
                                string? sortFieldInput = Console.ReadLine();
                                BO.ClosedCallInListEnum? sortField = Enum.TryParse(sortFieldInput, out BO.ClosedCallInListEnum parsedSortField) ? (BO.ClosedCallInListEnum?)parsedSortField : null;

                                var closedCalls = s_bl.Call.GetClosedCallsOfVolunteer(volunteerId, callTypeFilter, sortField);

                                Console.WriteLine("\nClosed Calls Handled By Volunteer:");
                                foreach (var call in closedCalls)
                                {
                                    Console.WriteLine(call);
                                }
                            }
                            else
                            {
                                throw new BO.BlFormatException("Invalid input. Volunteer ID must be a number.");
                            }
                        }
                        catch (Exception ex)
                        {
                            HandleException(ex);
                        }
                        break;
                    case 3:
                        try
                        {
                            //GetCallsList(BO.CallType ? filterField, object ? filterValue, string ? sortField)
                            Console.WriteLine("Enter Call Type filter (Emergency,Equipment,Doctor,Training) or press Enter to skip:");
                            string? filterFieldInput = Console.ReadLine();
                            BO.CallType? filterValue = Enum.TryParse(filterFieldInput, out BO.CallType parsedFilterField) ? parsedFilterField : null;

                            Console.WriteLine("Enter sort field ( Status,  Type,  CallerName ) or press Enter to skip:");
                            string? sortFieldInput = Console.ReadLine();
                            BO.CallInListField? sortField = Enum.TryParse(sortFieldInput, out BO.CallInListField parsedSortField) ? (BO.CallInListField?)parsedSortField : null;

                            var callList = s_bl.Call.GetCallList(BO.CallInListField.Status, filterValue, sortField);

                            foreach (var call in callList)
                                Console.WriteLine(call);
                        }
                        catch (Exception ex)
                        {
                            HandleException(ex);
                        }
                        break;
                    case 4:
                        try
                        {
                            Console.Write("Enter Call ID: ");
                            if (int.TryParse(Console.ReadLine(), out int callId))
                            {
                                var call = s_bl.Call.GetCallDetails(callId);
                                Console.WriteLine(call);
                            }
                            else
                            {
                                throw new FormatException("Invalid input. Volunteer ID must be a number.");
                            }
                        }
                        catch (Exception ex)
                        {
                            HandleException(ex);
                        }
                        break;
                    case 5:
                        try
                        {
                            BO.Call call = new BO.Call();

                            Console.Write("הזן סוג קריאה (0-חירום, 1-רפואי, 2-אספקה): ");
                            if (int.TryParse(Console.ReadLine(), out int callType) && Enum.IsDefined(typeof(BO.CallType), callType))
                                call.CallType = (BO.CallType)callType;
                            else
                            {
                                Console.WriteLine("❗ סוג קריאה לא חוקי");
                                break;
                            }

                            Console.Write("הזן תיאור: ");
                            call.Description = Console.ReadLine();

                            Console.Write("הזן כתובת: ");
                            call.Address = Console.ReadLine() ?? string.Empty;

                            Console.Write("הזן קו רוחב (Latitude): ");
                            double.TryParse(Console.ReadLine(), out double lat);
                            call.Latitude = lat;


                            Console.Write("הזן קו אורך (Longitude): ");
                            double.TryParse(Console.ReadLine(), out double lon);
                            call.Longitude = lon;

                            call.CreationTime = s_bl.Admin.GetCurrentTime();
                            call.Status = BO.CallStatus.Open;

                            s_bl.Call.AddCall(call);
                            Console.WriteLine("✅ קריאה נוספה בהצלחה");
                        }
                        catch (Exception ex)
                        {
                            HandleException(ex);
                        }
                        ;
                        break;
                    case 6:
                        try
                        {
                            Console.Write("Enter Call ID: ");
                            if (int.TryParse(Console.ReadLine(), out int cId))
                            {
                                s_bl.Call.DeleteCall(cId);
                                Console.WriteLine("Call removed.");
                            }
                            else
                            {
                                throw new FormatException("Invalid input. Volunteer ID must be a number.");
                            }
                        }
                        catch (Exception ex)
                        {
                            HandleException(ex);
                        }
                        break;
                    case 7:
                        UpDateCall();
                        break;
                    case 8:
                        try
                        {
                            Console.Write("Enter Volunteer ID: ");
                            if (int.TryParse(Console.ReadLine(), out int volunteerId))
                            {
                                Console.WriteLine("Enter Call Type filter (or press Enter to skip):");
                                string? callTypeInput = Console.ReadLine();
                                BO.CallType? callTypeFilter = Enum.TryParse(callTypeInput, out BO.CallType parsedCallType) ? parsedCallType : null;

                                Console.WriteLine("Enter Sort Field (or press Enter to skip):");
                                string? sortFieldInput = Console.ReadLine();
                                BO.OpenCallInListEnum? sortField = Enum.TryParse(sortFieldInput, out BO.OpenCallInListEnum parsedSortField) ? parsedSortField : null;

                                var openCalls = s_bl.Call.GetOpenCallsForVolunteer(volunteerId, callTypeFilter, sortField);

                                Console.WriteLine("\nOpen Calls Available for Volunteer:");
                                foreach (var call in openCalls)
                                {
                                    Console.WriteLine(call);
                                }
                            }
                            else
                            {
                                throw new BO.BlFormatException("Invalid input. Volunteer ID must be a number.");
                            }
                        }
                        catch (Exception ex)
                        {
                            HandleException(ex);
                        }
                        break;
                    case 9:
                        try
                        {
                            Console.Write("Enter Volunteer ID: ");
                            if (!int.TryParse(Console.ReadLine(), out int volunteerId))
                                throw new BO.BlFormatException("Invalid input. Volunteer ID must be a number.");

                            Console.Write("Enter call ID: ");
                            if (!int.TryParse(Console.ReadLine(), out int callId))
                                throw new BO.BlFormatException("Invalid input. call ID must be a number.");

                            s_bl.Call.CancelCallTreatment(volunteerId, callId);
                            Console.WriteLine("The call was successfully canceled.");
                        }
                        catch (Exception ex)
                        {
                            HandleException(ex);
                        }
                        break;
                    case 10:
                        try
                        {
                            Console.Write("Enter Volunteer ID: ");
                            string? volunteerInput = Console.ReadLine();
                            if (!int.TryParse(volunteerInput, out int volunteerId))
                            {
                                throw new FormatException("Invalid input. Volunteer ID must be a number.");
                            }

                            Console.Write("Enter Assignment ID: ");
                            string? assignmentInput = Console.ReadLine();
                            if (!int.TryParse(assignmentInput, out int assignmentId))
                            {
                                throw new FormatException("Invalid input. Assignment ID must be a number.");
                            }

                            s_bl.Call.CompleteCallTreatment(volunteerId, assignmentId);

                            Console.WriteLine("Call completion updated successfully!");
                        }
                        catch (Exception ex)
                        {
                            HandleException(ex);
                        }
                        break;
                    case 11:
                        try
                        {
                            Console.Write("Enter Volunteer ID: ");
                            if (!int.TryParse(Console.ReadLine(), out int volunteerId))
                                throw new FormatException("Invalid input. Volunteer ID must be a number.");

                            Console.Write("Enter Call ID: ");
                            if (!int.TryParse(Console.ReadLine(), out int callId))
                                throw new FormatException("Invalid input. Call ID must be a number.");

                            s_bl.Call.SelectCallForTreatment(volunteerId, callId);
                            Console.WriteLine("The call has been successfully assigned to the volunteer.");
                        }
                        catch (Exception ex)
                        {
                            HandleException(ex);
                        }
                        break;
                    case 0:
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            HandleException(ex);
        }
    }
    static void UpDateCall()
    {
        Console.Write("Enter Call ID: ");
        int.TryParse(Console.ReadLine(), out int callId);
        Console.Write("Enter New Description (optional) : ");
        string description = Console.ReadLine();
        Console.Write("Enter New Full Address (optional) : ");
        string address = Console.ReadLine();
        Console.Write("Enter Call Type (optional) : ");
        BO.CallType? callType = Enum.TryParse(Console.ReadLine(), out BO.CallType parsedType) ? parsedType : (BO.CallType?)null;
        Console.Write("Enter Max Finish Time (hh:mm , (optional)): ");
        TimeSpan? maxFinishTime = TimeSpan.TryParse(Console.ReadLine(), out TimeSpan parsedTime) ? parsedTime : (TimeSpan?)null;
        try
        {
            var callToUpdate = s_bl.Call.GetCallDetails(callId);
            if (callToUpdate == null)
                throw new BO.BlDoesNotExistException($"Call with ID{callId} does not exist!");
            var newUpdatedCall = new BO.Call
            {
                Id = callId,
                Description = !string.IsNullOrWhiteSpace(description) ? description : callToUpdate.Description,
                Address = !string.IsNullOrWhiteSpace(address) ? address : /*callToUpdate. FullAddress*/"No Address",
                CreationTime = callToUpdate.CreationTime,
                MaxFinishTime = (maxFinishTime.HasValue ? DateTime.Now.Date + maxFinishTime.Value : callToUpdate.MaxFinishTime),
                CallType = callType ?? callToUpdate.CallType
            };
            s_bl.Call.UpdateCall(newUpdatedCall);
            Console.WriteLine("Call updated successfully.");
        }
        catch (Exception ex)
        {
            HandleException(ex);
        }
    }
    static void HandleException(Exception ex)
    {
        Console.WriteLine($"Exception: {ex.GetType().Name},");
        Console.WriteLine($"Message: {ex.Message}");
        if (ex.InnerException != null)
        {
            Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
        }
    }
}

