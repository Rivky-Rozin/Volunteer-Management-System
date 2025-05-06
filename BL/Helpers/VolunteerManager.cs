namespace Helpers;

using System.Text.RegularExpressions;
using DalApi;

internal static class VolunteerManager
{
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
        IEnumerable<DO.Assignment> assignments = s_dal.Assignment.ReadAll()
            .Where(a => a.VolunteerId == volunteer.Id)
            .ToList();

        AssignmentStats stats = GetVolunteerAssignmentStats(assignments);

        // חיפוש ההקצאה הפעילה (שאין לה EndTreatment)
        DO.Assignment? activeAssignment = assignments.FirstOrDefault(a => a.EndTreatment == null);

        int? callInProgressId = activeAssignment?.CallId;

        BO.CallType callInProgressType = BO.CallType.None;

        if (callInProgressId != null)
        {
            var call = s_dal.Call.Read(callInProgressId.Value);
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

    public static BO.Volunteer ToBOVolunteer(DO.Volunteer volunteer)
    {
        IEnumerable<DO.Assignment> assignments = s_dal.Assignment.ReadAll()
            .Where(a => a.VolunteerId == volunteer.Id);

        AssignmentStats stats = GetVolunteerAssignmentStats(assignments);

        DO.Assignment? activeAssignment = assignments.FirstOrDefault(a => a.EndTreatment == null);

        BO.CallInProgress? callInProgress = null;
        if (activeAssignment != null)
        {
            DO.Call? call = s_dal.Call.Read(activeAssignment.CallId);
            if (call is not null)
            {
                TimeSpan RiskTimeSpan = s_dal.Config.RiskTimeSpan;

                DateTime now = ClockManager.Now;
                DateTime? maxResolutionTime = call.MaxCallTime;

                var status =
                    maxResolutionTime is null
                        ? BO.CallInProgressStatus.InProgress
                        : (maxResolutionTime.Value - now) <= RiskTimeSpan
                            ? BO.CallInProgressStatus.InProgressAtRisk
                            : BO.CallInProgressStatus.InProgress;

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
                    status = status
                };
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

        if (!IsValidIsraeliID(volunteer.Id.ToString()))
            throw new BO.BlFormatException("תעודת זהות אינה תקינה לפי ספרת ביקורת.");

        // בדיקת שם
        if (string.IsNullOrWhiteSpace(volunteer.Name))
            throw new BO.BlFormatException("שם המתנדב לא יכול להיות ריק.");

        // בדיקת טלפון
        if (!Regex.IsMatch(volunteer.Phone, @"^0\d{1,2}-?\d{7}$"))
            throw new BO.BlFormatException("מספר הטלפון אינו תקין.");

        // בדיקת אימייל
        if (!Regex.IsMatch(volunteer.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new BO.BlFormatException("כתובת האימייל אינה תקינה.");

        // בדיקת כתובת – כאן אפשר להרחיב בעתיד עם חיבור למ
        if (!string.IsNullOrWhiteSpace(volunteer.Address) && (volunteer.Latitude == null || volunteer.Longitude == null))
            throw new BO.BlFormatException("כתובת לא יכולה להיות מוגדרת בלי קואורדינטות.");
        try
        {
            var (x, y) = Tools.GetCoordinatesFromAddress(volunteer.Address);
        }
        catch (Exception)
        {
            throw new BO.BlFormatException("כתובת לא יכולה להיות מוגדרת בלי קואורדינטות.");
        }
        // בדיקת סיסמה אם קיימת
        if (!string.IsNullOrEmpty(volunteer.Password))
            throw new BO.BlFormatException("הסיסמה חייבת להכיל לפחות 6 תווים.");

        // בדיקת מרחק
        if (volunteer.MaxDistance is < 0)
            throw new BO.BlFormatException("מרחק מקסימלי לא יכול להיות שלילי.");
    }

    // פונקציית בדיקת ת"ז ישראלית
    private static bool IsValidIsraeliID(string id)
    {
        id = id.PadLeft(9, '0');
        int sum = 0;
        for (int i = 0; i < 9; i++)
        {
            int num = int.Parse(id[i].ToString());
            int mult = (i % 2 == 0) ? 1 : 2;
            int temp = num * mult;
            sum += (temp > 9) ? temp - 9 : temp;
        }
        return sum % 10 == 0;
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
            DistanceKind: boVolunteer.DistanceKind.HasValue ? (DO.DistanceKind)boVolunteer.DistanceKind.Value : throw new BO.BlFormatException("DistanceKind cannot be null."),
            Address: boVolunteer.Address,
            Latitude: boVolunteer.Latitude,
            Longitude: boVolunteer.Longitude,
            Password: boVolunteer.Password,
            MaxDistance: boVolunteer.MaxDistance
        );
    }
}
