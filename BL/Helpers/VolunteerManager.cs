using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DalApi;

namespace Helpers
{
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

        public static DO.Volunteer ToDalVolunteer(Volunteer volunteer)
        {
            return new DO.Volunteer
            {
                Id = volunteer.Id,
                Name = volunteer.Name,
                Phone = volunteer.Phone,
                Email = volunteer.Email,
                Password = volunteer.Password,
                Role = (DO.VolunteerRole)volunteer.Role,
                IsActive = volunteer.IsActive
            };
        }

        public static CallInProgress ToCallInProgress(DO.Call call, DO.Assignment assignment)
        {
            return new CallInProgress
            {
                Id = call.Id,
                Start = assignment.ActualTreatmentStartTime,
                Address = call.Address,
                CallerName = call.CallerName,
                EmergencyLevel = call.EmergencyLevel switch
                {
                    EmergencyLevel.Low => "Low",
                    EmergencyLevel.Medium => "Medium",
                    EmergencyLevel.High => "High",
                    EmergencyLevel.Critical => "Critical",
                    _ => "Unknown"
                },
                Description = call.Description,
                RequiredTreatmentTime = assignment.RequiredTreatmentTime
            };
        }

        public static void ValidateVolunteer(Volunteer volunteer)
        {
            if (string.IsNullOrWhiteSpace(volunteer.Name))
                throw new BlValidationException("Name is required");

            if (string.IsNullOrWhiteSpace(volunteer.Email) || !volunteer.Email.Contains("@"))
                throw new BlValidationException("Invalid email");

            if (string.IsNullOrWhiteSpace(volunteer.Phone) || volunteer.Phone.Length < 7)
                throw new BlValidationException("Invalid phone number");

            if (string.IsNullOrWhiteSpace(volunteer.Password) || volunteer.Password.Length < 4)
                throw new BlValidationException("Password must be at least 4 characters long");
        }
    }
}
