namespace Dal;

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DalApi;
using DO;

internal class CallImplementation : ICall


{
    [MethodImpl(MethodImplOptions.Synchronized)]
    public int Create(Call item)
    {
        int id = Config.NextCallId;
        Call copy = item with { Id = id };
        DataSource.Calls.Add(copy);
        return copy.Id; //return the ID of the created call
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        if (Read(id) == null)
        {
            throw new DalDoesNotExistException($"A Call with this ID={id} does not exist");
        }
        DataSource.Calls.Remove(Read(id));
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        DataSource.Calls.Clear();
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Call? Read(int id)
    {
        Call? found;
        found = DataSource.Calls.FirstOrDefault(item => item.Id == id); //stage 2

        return found;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Call? Read(Func<Call, bool> filter)
    {
        return DataSource.Calls.FirstOrDefault(filter);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null) //stage 2
    {
        if (filter != null)
            return from item in DataSource.Calls
                   where filter(item)
                   select item;

        return from item in DataSource.Calls
               select item;
    }


    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Call item)
    {
        //Delete(item.Id);
        Create(item);
    }
}
