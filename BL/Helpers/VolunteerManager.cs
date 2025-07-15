namespace Helpers;

using System.Numerics;
using System.Text.RegularExpressions;
using BlImplementation;
using DalApi;
using DO;

internal static class VolunteerManager
{
    internal static ObserverManager Observers = new(); //stage 5 
    private static IDal s_dal = Factory.Get; //stage 4

    public record AssignmentStats(int Handled, int Cancelled, int Expired);

    public static AssignmentStats GetVolunteerAssignmentStats(IEnumerable<DO.Assignment> assignments)
    {
        int handled = 0, cancelled = 0, expired = 0;

        foreach (DO.Assignment a in assignments)
        {
            if (a.TreatmentType == DO.TreatmentType.ManagerCancelled || a.TreatmentType == DO.TreatmentType.UserCancelled)
            {
                cancelled++;
            }
            else if (a.EndTreatment != null)
            {
                handled++;

                // בדיקה אם הקריאה נחשבת כפג תוקף לפי CallManager
                BO.CallStatus status = CallManager.GetCallStatus(a.CallId);
                if (status == BO.CallStatus.Expired)
                {
                    expired++;
                }
            }
        }

        return new AssignmentStats(handled, cancelled, expired);
    }

    public static BO.VolunteerInList ToVolunteerInList(DO.Volunteer volunteer)
    {
        IEnumerable<DO.Assignment> validAssignments;
        lock (AdminManager.BlMutex)
        {

            validAssignments = s_dal.Assignment.ReadAll()
           .Where(a => a.VolunteerId == volunteer.Id).Where(a =>
{
    var call = s_dal.Call.Read(a.CallId);
    if (call == null) return false;
    var status = CallManager.GetCallStatus(call.Id);
    return a.StartTreatment != null && status != BO.CallStatus.Expired;
})
           .OrderByDescending(a => a.StartTreatment)
           .ToList();
        }
        DO.Assignment? activeAssignment = validAssignments.FirstOrDefault();


        AssignmentStats stats = GetVolunteerAssignmentStats(validAssignments);


        int? callInProgressId = activeAssignment?.CallId;

        BO.CallType callInProgressType = BO.CallType.None;

        if (callInProgressId != null)
        {
            DO.Call? call;
            lock (AdminManager.BlMutex)
            {
                call = s_dal.Call.Read(callInProgressId.Value);
            }
            if (call != null)
            {
                callInProgressType = (BO.CallType)call.CallType;
            }
        }

        return new BO.VolunteerInList
        {
            Id = volunteer.Id,
            Name = volunteer.Name,
            IsActive = volunteer.IsActive,
            HandledCallsCount = stats.Handled,
            CancelledCallsCount = stats.Cancelled,
            ExpiredHandledCallsCount = stats.Expired,
            CallInProgressId = callInProgressId,
            CallInProgressType = callInProgressType
        };
    }


    private static BO.CallInProgressStatus MapCallStatusToInProgressStatus(BO.CallStatus status)
    {
        return status switch
        {
            BO.CallStatus.Open => BO.CallInProgressStatus.InProgress,
            BO.CallStatus.OpenAtRisk => BO.CallInProgressStatus.InProgressAtRisk,
            BO.CallStatus.InProgress => BO.CallInProgressStatus.InProgress,
            BO.CallStatus.InProgressAtRisk => BO.CallInProgressStatus.InProgressAtRisk,
            // כל סטטוס אחר (למשל Expired, Closed) – לא יוצג כקריאה בטיפול
            _ => BO.CallInProgressStatus.None
        };
    }
    public static BO.Volunteer ToBOVolunteer(DO.Volunteer volunteer)
    {
        IEnumerable<DO.Assignment> assignments;
        lock (AdminManager.BlMutex)
        {

            assignments = s_dal.Assignment.ReadAll()
            .Where(a => a.VolunteerId == volunteer.Id);
        }

        AssignmentStats stats = GetVolunteerAssignmentStats(assignments);

        DO.Assignment? activeAssignment = assignments

.Where(a =>
{
    DO.Call call;

    lock (AdminManager.BlMutex)
    {

        call = s_dal.Call.Read(a.CallId);
    }
    if (call == null) return false;

    var status = CallManager.GetCallStatus(call.Id);

    // להציג קריאה גם אם הסתיימה, **כל עוד לא Expired**
    return status != BO.CallStatus.Expired;
})

            .OrderByDescending(a => a.StartTreatment)
            .FirstOrDefault();

        BO.CallInProgress? callInProgress = null;
        if (activeAssignment != null)
        {
            lock (AdminManager.BlMutex)
            {

                DO.Call? call = s_dal.Call.Read(activeAssignment.CallId);
                if (call is not null)
                {
                    TimeSpan RiskTimeSpan = s_dal.Config.RiskTimeSpan;

                    DateTime now = AdminManager.Now;
                    DateTime? maxResolutionTime = call.MaxCallTime;

                    var status = CallManager.GetCallStatus(call.Id);

                    callInProgress = new BO.CallInProgress
                    {
                        Id = activeAssignment.Id,
                        CallId = call.Id,
                        CallType = (BO.CallType)call.CallType,
                        Description = call.Description,
                        FullAddress = call.FullAddress,
                        OpenTime = call.OpenTime,
                        MaxResolutionTime = call.MaxCallTime,
                        EntryToTreatmentTime = activeAssignment.StartTreatment,
                        DistanceFromVolunteer = Tools.GetDistance(volunteer, call),
                        status = MapCallStatusToInProgressStatus(CallManager.GetCallStatus(call.Id))
                    };
                }
            }
        }

        return new BO.Volunteer
        {
            Id = volunteer.Id,
            Name = volunteer.Name,
            Phone = volunteer.Phone,
            Email = volunteer.Email,
            Password = volunteer.Password,
            Address = volunteer.Address,
            Latitude = volunteer.Latitude,
            Longitude = volunteer.Longitude,
            Role = (BO.VolunteerRole)volunteer.Role,
            IsActive = volunteer.IsActive,
            MaxDistance = volunteer.MaxDistance,
            DistanceKind = (BO.DistanceKind?)volunteer.DistanceKind,
            HandledCallsCount = stats.Handled,
            CancelledCallsCount = stats.Cancelled,
            ExpiredHandledCallsCount = stats.Expired,
            CallInProgress = callInProgress
        };
    }


    public static void ValidateVolunteer(BO.Volunteer volunteer)
    {
        // בדיקת מזהה
        if (volunteer.Id <= 0)
            throw new BO.BlFormatException("תעודת זהות חייבת להיות מספר חיובי.");

        if (volunteer.Id < 99999999 || volunteer.Id > 999999999)
            throw new BO.BlFormatException("תעודת זהות אינה תקינה .");

        // בדיקת שם
        if (string.IsNullOrWhiteSpace(volunteer.Name))
            throw new BO.BlFormatException("שם המתנדב לא יכול להיות ריק.");

        // בדיקת טלפון
        if (string.IsNullOrWhiteSpace(volunteer.Phone))
            throw new BO.BlFormatException("מספר הטלפון אינו תקין.");

        // בדיקת אימייל
        if (!Regex.IsMatch(volunteer.Email.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new BO.BlFormatException("כתובת האימייל אינה תקינה.");

        // בדיקת כתובת – כאן אפשר להרחיב בעתיד עם חיבור למ
        if (string.IsNullOrWhiteSpace(volunteer.Address))
            throw new BO.BlFormatException("כתובת לא יכולה null.");
        //מיותר - בהוספה או בעידכון התכנית ממילא תיפול ולא תוסיף או תעדכן אם הכתובת לא תקינה. אין סיבה להוסיף קריאת רשת מיותרת
        //try
        //{
        //    var (x, y) = Tools.GetCoordinatesFromAddress(volunteer.Address);
        //}
        //catch (Exception)
        //{
        //    throw new BO.BlFormatException("כתובת לא יכולה להיות מוגדרת בלי קואורדינטות.");
        //}

        // בדיקת מרחק
        if (volunteer.MaxDistance is < 0)
            throw new BO.BlFormatException("מרחק מקסימלי לא יכול להיות שלילי.");
        // בדיקת מרחק
        if (volunteer.DistanceKind == null)
            throw new BO.BlFormatException("חובה להזין סוג מרחק");
    }
    public static DO.Volunteer ToDoVolunteer(BO.Volunteer boVolunteer)
    {
        if (boVolunteer == null)
            throw new BO.BlObjectCanNotBeNullException(nameof(boVolunteer));

        return new DO.Volunteer(
            Id: boVolunteer.Id,
            Name: boVolunteer.Name,
            Phone: boVolunteer.Phone,
            Email: boVolunteer.Email,
            Role: (DO.VolunteerRole)boVolunteer.Role,
            IsActive: boVolunteer.IsActive,
            DistanceKind: boVolunteer.DistanceKind.HasValue ? (DO.DistanceKind)boVolunteer.DistanceKind.Value : throw new Exception("לא יכול להיות null"),
            Address: boVolunteer.Address,
            Latitude: boVolunteer.Latitude,
            Longitude: boVolunteer.Longitude,
            Password: boVolunteer.Password,
            MaxDistance: boVolunteer.MaxDistance
        );
    }


    public static async Task UpdateVolunteerCoordinatesAsync(BO.Volunteer volunteer)
    {
        if (!string.IsNullOrWhiteSpace(volunteer.Address))
        {
            try
            {
                DO.Volunteer doVolunteer = ToDoVolunteer(volunteer);
                var (lat, lon) = await Tools.GetCoordinatesFromAddress(doVolunteer.Address);
                doVolunteer = doVolunteer with { Latitude = lat, Longitude = lon };

                lock (AdminManager.BlMutex)
                    s_dal.Volunteer.Update(doVolunteer); // עדכון ה־DAL עם הקואורדינטות

                VolunteerManager.Observers.NotifyListUpdated();
                VolunteerManager.Observers.NotifyItemUpdated(doVolunteer.Id);
            }
            catch (Exception ex)
            {
                throw new BO.BlFormatException("כתובת לא תקינה או לא נמצאה.", ex);
            }
        }
    }

    internal static void SimulateVolunteerActivity()
    {
        // First get all active volunteers - wrap the DAL call in a lock and convert to concrete list
        List<DO.Volunteer> activeVolunteers;
        lock (AdminManager.BlMutex)
        {
            activeVolunteers = s_dal.Volunteer.ReadAll(v => v.IsActive).ToList();
        }

        // Random for probability decisions
        Random random = new();

        foreach (var volunteer in activeVolunteers)
        {
            // Check if volunteer has an active assignment
            DO.Assignment? currentAssignment;
            lock (AdminManager.BlMutex)
            {
                currentAssignment = s_dal.Assignment.ReadAll()
                    .Where(a => a.VolunteerId == volunteer.Id && a.EndTreatment == null)
                    .FirstOrDefault();
            }

            if (currentAssignment == null)
            {
                // No active assignment - maybe choose a new call (20% chance)
                if (random.NextDouble() < 0.20)
                {
                    // Get available open calls with coordinates
                    List<DO.Call> availableCalls;
                    lock (AdminManager.BlMutex)
                    {
                        availableCalls = s_dal.Call.ReadAll()
                            .Where(c => c.Latitude != 0 && c.Longitude != 0
                                   && CallManager.GetCallStatus(c.Id) == BO.CallStatus.Open)
                            .ToList();
                    }

                    // Filter calls by distance and pick random one
                    var eligibleCalls = availableCalls
                        .Where(call => Tools.GetDistance(volunteer, call) <= volunteer.MaxDistance)
                        .ToList();

                    if (eligibleCalls.Any())
                    {
                        var selectedCall = eligibleCalls[random.Next(eligibleCalls.Count)];

                        // Create new assignment
                        var newAssignment = new DO.Assignment
                        {
                            CallId = selectedCall.Id,
                            VolunteerId = volunteer.Id,
                            StartTreatment = AdminManager.Now,
                            EndTreatment = null
                        };

                        lock (AdminManager.BlMutex)
                        {
                            s_dal.Assignment.Create(newAssignment);
                        }

                        // Notify outside lock
                        Observers.NotifyListUpdated();
                    }
                }
            }
            else
            {
                // Has active assignment - check if enough time passed
                DO.Call? call;
                lock (AdminManager.BlMutex)
                {
                    call = s_dal.Call.Read(currentAssignment.CallId);
                }

                if (call != null)
                {
                    double distance = Tools.GetDistance(volunteer, call);
                    // Base time on distance (1 minute per km) plus random 5-15 minutes
                    TimeSpan requiredTime = TimeSpan.FromMinutes(distance + random.Next(5, 15));

                    if (AdminManager.Now - currentAssignment.StartTreatment >= requiredTime)
                    {
                        // Complete the assignment
                        var updatedAssignment = new DO.Assignment(
                            currentAssignment.Id,
                            currentAssignment.VolunteerId,
                            currentAssignment.CallId,
                            currentAssignment.StartTreatment,
                            AdminManager.Now,
                            DO.TreatmentType.Treated
                        );

                        lock (AdminManager.BlMutex)
                        {
                            s_dal.Assignment.Update(updatedAssignment);
                        }

                        // Notify outside lock
                        Observers.NotifyListUpdated();
                    }
                    else if (random.NextDouble() < 0.10) // 10% chance to cancel
                    {
                        // Cancel the assignment
                        var updatedAssignment = new DO.Assignment(
                            currentAssignment.Id,
                            currentAssignment.VolunteerId,
                            currentAssignment.CallId,
                            currentAssignment.StartTreatment,
                            AdminManager.Now,
                            DO.TreatmentType.UserCancelled
                        );

                        lock (AdminManager.BlMutex)
                        {
                            s_dal.Assignment.Update(updatedAssignment);
                        }

                        // Notify outside lock
                        Observers.NotifyListUpdated();
                    }
                    else if (random.NextDouble() < 0.15) // 15% סיכוי לדוג'
                    {

                        CallManager.CreateSimulatedCall();

                        // Notify outside lock
                        Observers.NotifyListUpdated();
                    }
                }
            }
        }
    }
}
