

namespace BlImplementation;

using System.Collections.Generic;
using BlApi;
using BO;

internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void AddVolunteer(Volunteer volunteer)
    {
        throw new NotImplementedException();
    }

    public void DeleteVolunteer(string id)
    {
        throw new NotImplementedException();
    }

    public Volunteer GetVolunteerDetails(string id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<VolunteerInList> GetVolunteersList(bool? isActive = null, VolunteerInList? sortBy = null)
    {
        throw new NotImplementedException();
    }

    public VolunteerRole Login(string username, string password)
    {
        throw new NotImplementedException();
    }

    public void UpdateVolunteer(string id, Volunteer volunteer)
    {
        throw new NotImplementedException();
    }
}

