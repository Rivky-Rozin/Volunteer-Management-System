
namespace Dal;

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

//using System.Collections.Generic;
using DalApi;
using DO;

internal class AssignmentImplementation : IAssignment
{
    [MethodImpl(MethodImplOptions.Synchronized)]
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

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        if (Read(id) == null)
        {
            throw new DalDoesNotExistException($"An assignment with this ID={id} does not exist");
        }
        DataSource.Assignments.Remove(Read(id));
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        DataSource.Volunteers.Clear();
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Assignment? Read(int id)
    {
        Assignment? found;
        found = DataSource.Assignments.FirstOrDefault(item => item.Id == id); //stage 2

        return found;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Assignment? Read(Func<Assignment, bool> filter)
    {
        return DataSource.Assignments.FirstOrDefault(filter);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null) //stage 2
    {
        if (filter != null)
            return from item in DataSource.Assignments
                   where filter(item)
                   select item;

        return from item in DataSource.Assignments
               select item;
    }




    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Assignment item)
    {
        Delete(item.Id);
        Create(item);
    }
}
