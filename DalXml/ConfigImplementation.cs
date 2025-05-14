namespace Dal;
using DalApi;


internal class ConfigImplementation : IConfig
{
    public DateTime Clock
    {
        get => Config.Clock;
        set => Config.Clock = value;
    }
    public TimeSpan RiskTimeSpan
    {
        get => Config.RiskTimeSpan;
        set => Config.RiskTimeSpan = value;
    }
    public TimeSpan TreatmentTime
    {
        get => Config.TreatmentTime;
        set => Config.TreatmentTime = value;
    }

    public void Reset()
    {
        Config.Reset();
    }
}

