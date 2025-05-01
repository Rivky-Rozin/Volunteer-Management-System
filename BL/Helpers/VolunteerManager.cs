using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BO;
using DalApi;

namespace Helpers
{
    internal static class VolunteerManager
    {
        private static IDal s_dal = Factory.Get; //stage 4

        public static VolunteerInList ToVolunteerInList(DO.Volunteer volunteer)
        {
            var allAssignments = _dal.Assignment.ReadAll()
                .Where(a => a.VolunteerId == volunteer.Id);

            int handled = allAssignments.Count(a => a.ActualTreatmentEndTime != null);
            int cancelled = allAssignments.Count(a => a.Cancelled == true);
            int expired = allAssignments.Count(a =>
                a.ActualTreatmentEndTime != null &&
                _dal.Call.Read(a.CallId)?.EmergencyLevel == EmergencyLevel.Critical &&
                a.ActualTreatmentEndTime > a.RequiredTreatmentTime);

            return new VolunteerInList
            {
                Id = volunteer.Id,
                Name = volunteer.Name,
                Phone = volunteer.Phone,
                Email = volunteer.Email,
                Role = (VolunteerRole)volunteer.Role,
                IsActive = volunteer.IsActive,
                HandledCallsCount = handled,
                CancelledCallsCount = cancelled,
                ExpiredHandledCallsCount = expired
            };
        }

        public static Volunteer ToVolunteer(DO.Volunteer volunteer)
        {
            return new Volunteer
            {
                Id = volunteer.Id,
                Name = volunteer.Name,
                Phone = volunteer.Phone,
                Email = volunteer.Email,
                Password = volunteer.Password,
                Role = (VolunteerRole)volunteer.Role,
                IsActive = volunteer.IsActive
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
