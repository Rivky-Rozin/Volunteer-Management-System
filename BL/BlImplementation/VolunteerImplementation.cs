namespace BlImplementation;

using Helpers;
using System.Collections.Generic;
using BlApi;



internal class VolunteerImplementation : IVolunteer
{
    #region Stage 5
    public void AddObserver(Action listObserver) =>
    VolunteerManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
VolunteerManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
VolunteerManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
VolunteerManager.Observers.RemoveObserver(id, observer); //stage 5
    #endregion Stage 5
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public BO.VolunteerRole Login(string id, string password)
    {
        // המרה מת"ז למספר שלם
        if (!int.TryParse(id, out int volunteerId))
            throw new BO.BlValidationException("Invalid ID format");

        // ניסיון למצוא את המתנדב לפי הת"ז
        DO.Volunteer? volunteer = _dal.Volunteer.Read(volunteerId);

        if (volunteer == null)
            throw new BO.BlDoesNotExistException($"Volunteer with ID '{id}' was not found");

        // בדיקת סיסמה
        if (volunteer.Password!=null&&volunteer.Password != password)
            throw new BO.BlValidationException("Incorrect password");

        // החזרת תפקיד המתנדב
        return (BO.VolunteerRole)volunteer.Role;
    }


    public IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive = null, BO.VolunteerInListEnum? sortBy = null)
    {
        IEnumerable<DO.Volunteer> volunteers = _dal.Volunteer.ReadAll();
        Console.WriteLine($"מתנדבים קיימים ב-DAL: {volunteers.Count()}");
        Console.WriteLine($"isActive: {isActive}, sortBy: {sortBy}");

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
        return result.ToList();
    }

    public BO.Volunteer GetVolunteerDetails(string id)
    {
        DO.Volunteer volunteer = _dal.Volunteer.Read(int.Parse(id))
           
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
        VolunteerManager.Observers.NotifyListUpdated(); //stage 5  
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
        VolunteerManager.Observers.NotifyItemUpdated(volunteer.Id);  //stage 5
        VolunteerManager.Observers.NotifyListUpdated();  //stage 5

    }

    public void DeleteVolunteer(string id)
    {
        DO.Volunteer volunteer = _dal.Volunteer.Read(int.Parse(id))
            ?? throw new BO.BlDoesNotExistException($"Volunteer with ID {id} does not exist");

        bool hasAssignments = _dal.Assignment.Read(a => a.VolunteerId == volunteer.Id) is not null;
        if (hasAssignments)
            throw new BO.BlOperationNotAllowedException("Volunteer cannot be deleted while having assignments");

        _dal.Volunteer.Delete(volunteer.Id);
        VolunteerManager.Observers.NotifyListUpdated(); //stage 5
    }
}

