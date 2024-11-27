

namespace Dal;
//using System.Collections.Generic;
//using System.Runtime.CompilerServices;
using DalApi;
using DO;

public class VolunteerImplementation : IVolunteer
{
    //הוספת מתנדב חדש
    public void Create(Volunteer item)
    {
        if (!(Read(item.Id) == null))
        {
            throw new Exception("A volunteer with this ID already exists");
        }
        DataSource.Volunteers.Add(item);
    }

    //מחיקת משתמש בודד
    public void Delete(int id)
    {
        if (Read(id)==null)
        {
            throw new Exception("A volunteer with this ID does not exist");
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
        found = DataSource.Volunteers.Find(vol => vol.Id == id);
        return found;
    }

    //קבלת עותק של הרשימה של כל המתנדבים
    public List<Volunteer> ReadAll()
    {
        return new List<Volunteer>(DataSource.Volunteers);
    }

    //עדכון מתנדב
    public void Update(Volunteer item)
    {
        Delete(item.Id);
        Create(item);
    }
}
