
namespace Dal;

using System.Collections.Generic;
using System.Linq;

//using System.Collections.Generic;
using DalApi;
using DO;

internal class AssignmentImplementation : IAssignment
{
    public void Create(Assignment item)
    {
        //ראינו את ההערה על עדכון הID, ולמרות זאת לא שינינו כי רצינו להתייעץ עם המורה על בעיה אחרת שזה יוצר לנו והשיעור רק מחר. נשמח להתחשבות! 😃
        Assignment copy;
        if (item.Id > 0)
        {
            copy = item with { Id = item.Id };
        }
        else
        {
            int id = Config.NextAssignmentId;
            copy = item with { Id = id };
        }
        DataSource.Assignments.Add(copy);
    }

    public void Delete(int id)
    {
        if (Read(id) == null)
        {
            throw new DalDoesNotExistException($"An assignment with this ID={id} does not exist");
        }
        DataSource.Assignments.Remove(Read(id));
    }

    public void DeleteAll()
    {
        DataSource.Volunteers.Clear();
    }

    public Assignment? Read(int id)
    {
        Assignment? found;
        found = DataSource.Assignments.FirstOrDefault(item => item.Id == id); //stage 2

        return found;
    }

    public Assignment? Read(Func<Assignment, bool> filter)
    {
        return DataSource.Assignments.FirstOrDefault(filter);
    }
 
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null) //stage 2
    {
        if (filter != null)
            return from item in DataSource.Assignments
                   where filter(item)
                   select item;

        return from item in DataSource.Assignments
               select item;
    }




    //public List<Assignment> ReadAll()
    //{
    //    return new List<Assignment>(DataSource.Assignments);
    //}

    public void Update(Assignment item)
    {
        Delete(item.Id);
        Create(item);
    }
}
