
using BO;
using DO;

namespace BlApi;

public interface IVolunteer
{
    /// <summary>
    /// Attempts to log in a user with the provided credentials.
    /// </summary>
    /// <param name="username">The volunteer's email address.</param>
    /// <param name="password">The provided password.</param>
    /// <returns>The role of the volunteer (e.g., Manager, Volunteer).</returns>
    /// <exception cref="WrongPasswordException">If the password is incorrect.</exception>
    /// <exception cref="NotFoundException">If the volunteer does not exist.</exception>
    public BO.VolunteerRole Login(string username, string password);

    /// <summary>
    /// Retrieves a list of volunteers filtered and sorted as requested.
    /// </summary>
    /// <param name="isActive">True for active, false for inactive, null for all.</param>
    /// <param name="sortBy">Field to sort by (enum).</param>
    /// <returns>Filtered and sorted list of volunteers in list format.</returns>
    public IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive = null, BO.VolunteerInList? sortBy = null);

    /// <summary>
    /// Retrieves full details about a volunteer including their call in progress if any.
    /// </summary>
    /// <param name="id">Volunteer ID (Teudat Zehut).</param>
    /// <returns>Full volunteer object with call in progress.</returns>
    /// <exception cref="NotFoundException">If the volunteer is not found.</exception>
    public BO.Volunteer GetVolunteerDetails(string id);
    /// <summary>
    /// Updates a volunteer's details.
    /// </summary>
    /// <param name="editorId">The ID of the user requesting the update.</param>
    /// <param name="updatedVolunteer">The updated volunteer object.</param>
    /// <exception cref="InvalidAccessException">If the editor is unauthorized to update certain fields.</exception>
    /// <exception cref="FormatException">If any data format is invalid.</exception>
    /// <exception cref="NotFoundException">If the volunteer does not exist.</exception>
    public void UpdateVolunteer(string id, BO.Volunteer volunteer);

    /// <summary>
    /// Deletes a volunteer from the system.
    /// </summary>
    /// <param name="id">ID of the volunteer to delete.</param>
    /// <exception cref="InvalidOperationException">If the volunteer has any active or past assignments.</exception>
    /// <exception cref="NotFoundException">If the volunteer does not exist.</exception>
    public void DeleteVolunteer(string id);
    /// <summary>
    /// Adds a new volunteer to the system.
    /// </summary>
    /// <param name="volunteer">The volunteer to add.</param>
    /// <exception cref="DuplicateIdException">If a volunteer with the same ID already exists.</exception>
    /// <exception cref="FormatException">If any data format is invalid.</exception>
    public void AddVolunteer(BO.Volunteer volunteer);
}
