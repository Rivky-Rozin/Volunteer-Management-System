

namespace Dal;
//using System.Collections.Generic;
//using System.Runtime.CompilerServices;
using DalApi;
using DO;

internal class VolunteerImplementation : IVolunteer
{
    //הוספת מתנדב חדש
    public void Create(Volunteer item)
    {
        if (!(Read(item.Id) == null))
        {
            throw new DalAlreadyExistsException($"A volunteer with this ID={item.Id} already exists");
        }
        DataSource.Volunteers.Add(item);
    }

    //מחיקת משתמש בודד
    public void Delete(int id)
    {
        if (Read(id)==null)
        {
            throw new DalDoesNotExistException($"A volunteer with this ID={id} does not exist");
        }
        DataSource.Volunteers.Remove(Read(id));
    }

    //מחיקת רשימת המתנדבים
    public void DeleteAll()
    {
        DataSource.Volunteers.Clear();
    }

    //קבלת מתנדב בודד
    public Volunteer? Read(int id)
    {
        Volunteer? found;
        found = DataSource.Volunteers.FirstOrDefault(item => item.Id == id); //stage 2

        return found;
    }

    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        return DataSource.Volunteers.FirstOrDefault(filter);
    }

    //קבלת עותק של הרשימה של כל המתנדבים
    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null) //stage 2
    {
        if (filter != null)
            return from item in DataSource.Volunteers
                   where filter(item)
                   select item;

        return from item in DataSource.Volunteers
               select item;
    }


    //עדכון מתנדב
    public void Update(Volunteer item)
    {
        Delete(item.Id);
        Create(item);
    }
}
