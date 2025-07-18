

namespace DalApi;

public interface ICrud<T> where T : class
{
   
    int Create(T item); //Creates new entity object in DAL
    T? Read(int id); //Reads entity object by its ID 
    //List<T> ReadAll(); //stage 1 only, Reads all entity objects
    void Update(T item); //Updates entity object
    IEnumerable<T> ReadAll(Func<T, bool>? filter = null); // stage 2
    void Delete(int id); //Deletes an object by its Id
    void DeleteAll(); //Delete all entity objects
    T? Read(Func<T, bool> filter); // stage 2
}
