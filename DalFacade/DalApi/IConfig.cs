
namespace DalApi;
using DO;
public interface IConfig
{
    DateTime Clock { get; set; }
    TimeSpan RiskTimeSpan { get; set; }

    TimeSpan TreatmentTime { get; set; }
    void Reset();
}
