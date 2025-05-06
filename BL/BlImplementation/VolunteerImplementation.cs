namespace BlImplementation;

using Helpers;
using System.Collections.Generic;
using BlApi;



internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public BO.VolunteerRole Login(string username, string password)
    {
        DO.Volunteer volunteer = _dal.Volunteer.Read(v => v.Name == username)
            //todo
            ?? throw new BO.BlDoesNotExistException($"Volunteer '{username}' was not found");

        if (volunteer.Password != password)
            //todo
            throw new BO.BlValidationException("Incorrect password");

        return (BO.VolunteerRole)volunteer.Role;
    }

    public IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive = null, BO.VolunteerInListEnum? sortBy = null)
    {
        IEnumerable<DO.Volunteer> volunteers = _dal.Volunteer.ReadAll();

        // סינון לפי פעיל / לא פעיל
        if (isActive is not null)
            volunteers = volunteers.Where(v => v.IsActive == isActive.Value);

        // המרה ל־BO
        IEnumerable<BO.VolunteerInList> result = volunteers.Select(v => VolunteerManager.ToVolunteerInList(v));

        // מיון לפי שדה מבוקש
        result = sortBy switch
        {
            BO.VolunteerInListEnum.Id => result.OrderBy(v => v.Id),
            BO.VolunteerInListEnum.Name => result.OrderBy(v => v.Name),
            BO.VolunteerInListEnum.IsActive => result.OrderByDescending(v => v.IsActive),
            BO.VolunteerInListEnum.HandledCallsCount => result.OrderByDescending(v => v.HandledCallsCount),
            BO.VolunteerInListEnum.CancelledCallsCount => result.OrderByDescending(v => v.CancelledCallsCount),
            BO.VolunteerInListEnum.ExpiredHandledCallsCount => result.OrderByDescending(v => v.ExpiredHandledCallsCount),
            BO.VolunteerInListEnum.CallInProgressId => result.OrderByDescending(v => v.CallInProgressId ?? -1),
            BO.VolunteerInListEnum.CallInProgressType => result.OrderBy(v => v.CallInProgressType),
            _ => result.OrderBy(v => v.Id),
        };
        return result;
    }

    public BO.Volunteer GetVolunteerDetails(string id)
    {
        DO.Volunteer volunteer = _dal.Volunteer.Read(int.Parse(id))
            //todo
            ?? throw new BO.BlDoesNotExistException($"Volunteer with ID {id} does not exist");

        BO.Volunteer volunteerBO = VolunteerManager.ToBOVolunteer(volunteer);
        return volunteerBO;
    }

    public void AddVolunteer(BO.Volunteer volunteer)
    {
        try
        {
            VolunteerManager.ValidateVolunteer(volunteer);
        }

        catch (Exception e) {
            throw new BO.BlAlreadyExistsException($"יש נתונים שגויים");
        }

        if (_dal.Volunteer.Read(volunteer.Id) is not null)
            throw new BO.BlAlreadyExistsException($"Volunteer with ID {volunteer.Id} already exists");

        _dal.Volunteer.Create(VolunteerManager.ToDoVolunteer(volunteer));
    }

    public void UpdateVolunteer(string id, BO.Volunteer volunteer)
    {

        if (!int.TryParse(id, out int idInt) || idInt != volunteer.Id)
            throw new BO.BlValidationException("ID mismatch");

        DO.Volunteer existingVolunteer = _dal.Volunteer.Read(int.Parse(id))
            ?? throw new BO.BlDoesNotExistException($"Volunteer with ID {id} does not exist");

        if (existingVolunteer.Id != volunteer.Id && existingVolunteer.Role!=DO.VolunteerRole.Manager)
            throw new BO.BlPermissionException("Only an admin can change the role");
        try
        {
            VolunteerManager.ValidateVolunteer(volunteer);
        }
        catch (Exception e)
        {
            throw new BO.BlValidationException($"יש נתונים שגויים");
        }
        _dal.Volunteer.Update(VolunteerManager.ToDoVolunteer(volunteer));
    }

    public void DeleteVolunteer(string id)
    {
        DO.Volunteer volunteer = _dal.Volunteer.Read(int.Parse(id))
            ?? throw new BO.BlDoesNotExistException($"Volunteer with ID {id} does not exist");

        bool hasAssignments = _dal.Assignment.Read(a => a.VolunteerId == volunteer.Id) is not null;
        if (hasAssignments)
            throw new BO.BlOperationNotAllowedException("Volunteer cannot be deleted while having assignments");

        _dal.Volunteer.Delete(volunteer.Id);
    }
}

