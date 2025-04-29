
namespace BlApi
{
    public interface IVolunteer
    {
        public BO.VolunteerRole Login(string username, string password);

        public IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive = null, BO.VolunteerInList? sortBy = null);

        public BO.Volunteer GetVolunteerDetails(string id);

        public void UpdateVolunteer(string id, BO.Volunteer volunteer);

        public void DeleteVolunteer(string id);

        public void AddVolunteer(BO.Volunteer volunteer);
    }
}
