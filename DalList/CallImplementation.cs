namespace Dal;

using System.Collections.Generic;
using DalApi;
using DO;

public class CallImplementation : ICall
{
    public void Create(Call item)
    {
        int id = Config.NextCallId;
        Call copy = new(id,item.CallType,item.FullAddress,item.Latitude,item.Longitude,item.OpenTime,item.Description,item.MaxCallTime);   
        DataSource.Calls.Add(copy);
    }

    public void Delete(int id)
    {
        if (Read(id) == null)
        {
            throw new Exception($"A Call with this ID={id} does not exist");
        }
        DataSource.Calls.Remove(Read(id));
    }

    public void DeleteAll()
    {
        DataSource.Calls.Clear();
    }

    public Call? Read(int id)
    {
        Call? found;
        found = DataSource.Calls.Find(call => call.Id == id);
        return found;
    }

    public List<Call> ReadAll()
    {
        return new List<Call>(DataSource.Calls);
    }

    public void Update(Call item)
    {
        Delete(item.Id);
        Create(item);
    }
}
