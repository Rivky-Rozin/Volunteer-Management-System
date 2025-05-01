

namespace BlImplementation;

using System.Collections.Generic;
using BlApi;


internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public BO.VolunteerRole Login(string username, string password)
    {
        var volunteer = _dal.Volunteer.Read(v => v.Email == username)
            ?? throw new BO.BlDoesNotExistException($"Volunteer with email '{username}' not found");

        if (volunteer.Password != password)
            throw new BO.BlValidationException("Incorrect password");

        return (VolunteerRole)volunteer.Role;
    }

    public IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive = null, BO.VolunteerInList? sortBy = null)
    {
        var volunteers = _dal.Volunteer.ReadAll();

        // סינון לפי פעיל / לא פעיל
        if (isActive is not null)
            volunteers = volunteers.Where(v => v.IsActive == isActive.Value);

        // המרה ל־BO
        var result = volunteers.Select(v => ToVolunteerInList(v));

        // מיון לפי שדה מבוקש
        result = sortBy switch
        {
            VolunteerInList.HandledCallsCount => result.OrderByDescending(v => v.HandledCallsCount),
            VolunteerInList.CancelledCallsCount => result.OrderByDescending(v => v.CancelledCallsCount),
            VolunteerInList.ExpiredHandledCallsCount => result.OrderByDescending(v => v.ExpiredHandledCallsCount),
            _ => result.OrderBy(v => v.Id),
        };

        return result;
    }

    public BO.Volunteer GetVolunteerDetails(string id)
    {
        var volunteer = _dal.Volunteer.Read(int.Parse(id))
            ?? throw new BO.BlDoesNotExistException($"Volunteer with ID {id} does not exist");

        var volunteerBO = ToVolunteer(volunteer);

        var call = _dal.Assignment.Read(a => a.VolunteerId == volunteer.Id && a.ActualTreatmentEndTime == null);
        if (call is not null)
        {
            var callDetails = _dal.Call.Read(call.CallId);
            if (callDetails is not null)
                volunteerBO.CallInProgress = ToCallInProgress(callDetails, call);
        }

        return volunteerBO;
    }

    public void AddVolunteer(BO.Volunteer volunteer)
    {
        ValidateVolunteer(volunteer);

        if (_dal.Volunteer.Read(volunteer.Id) is not null)
            throw new BO.BlAlreadyExistsException($"Volunteer with ID {volunteer.Id} already exists");

        _dal.Volunteer.Create(ToDalVolunteer(volunteer));
    }

    public void UpdateVolunteer(string id, BO.Volunteer volunteer)
    {
        if (!int.TryParse(id, out int idInt) || idInt != volunteer.Id)
            throw new BO.BlValidationException("ID mismatch");

        var existing = _dal.Volunteer.Read(idInt)
            ?? throw new BO.BlDoesNotExistException($"Volunteer with ID {id} does not exist");

        if (existing.Role != (DO.VolunteerRole)volunteer.Role)
            throw new BO.BlPermissionException("Only an admin can change the role");

        ValidateVolunteer(volunteer);

        _dal.Volunteer.Update(ToDalVolunteer(volunteer));
    }

    public void DeleteVolunteer(string id)
    {
        var volunteer = _dal.Volunteer.Read(int.Parse(id))
            ?? throw new BO.BlDoesNotExistException($"Volunteer with ID {id} does not exist");

        var hasAssignments = _dal.Assignment.Read(a => a.VolunteerId == volunteer.Id) is not null;
        if (hasAssignments)
            throw new BO.BlOperationNotAllowedException("Volunteer cannot be deleted while having assignments");

        _dal.Volunteer.Delete(volunteer.Id);
    }
}

