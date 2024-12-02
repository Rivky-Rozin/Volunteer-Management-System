
namespace DalApi;
using DO;
public interface IConfig
{
    DateTime Clock { get; set; }
    void Reset();
    void SetNextCallId(int newValue);
    void SetNextAssignmentId(int newValue);
    void showNextCallId();
    void ShowNextAssignmentId();
}
