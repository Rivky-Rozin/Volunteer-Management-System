namespace Dal;

using System.Runtime.CompilerServices;
using DalApi;
internal class ConfigImplementation : IConfig
{
    public DateTime Clock
    {
        get => Config.Clock;
        set => Config.Clock = value;
    }
    public TimeSpan RiskTimeSpan { get => Config.riskTimeSpan; set => Config.riskTimeSpan = value; }
    public TimeSpan TreatmentTime { get => Config.treatmentTime; set => Config.treatmentTime = value; }

    public void Reset()
    {
        Config.Reset();
    }
}
