
namespace Dal;

//using System.Collections.Generic;
using DalApi;
using DO;

public class AssignmentImplementation : IAssignment
{
    public void Create(Assignment item)
    {
        int id=Config.NextAssignmentId;
        Assignment copy = item with { Id = id };
        DataSource.Assignments.Add(copy);
    }

    public void Delete(int id)
    {
        if (Read(id) == null)
        {
            throw new Exception($"An assignment with this ID={id} does not exist");
        }
        DataSource.Assignments.Remove(Read(id));
    }

    public void DeleteAll()
    {
        DataSource.Assignments.Clear();
    }

    public Assignment? Read(int id)
    {
        Assignment? found;
        found = DataSource.Assignments.Find(assignment => assignment.Id == id);
        return found;
    }

    public List<Assignment> ReadAll()
    {
        return new List<Assignment>(DataSource.Assignments);
    }

    public void Update(Assignment item)
    {
        Delete(item.Id);
        Create(item);
    }
}
