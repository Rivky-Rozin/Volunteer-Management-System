using DalApi;

namespace Dal;

internal class ConfigImplementation : IConfig
{
    public DateTime Clock
    {
        get => Config.Clock;
        set => Config.Clock = value;
    }
    public TimeSpan RiskTimeSpan { get; set; }

    public void Reset()
    {
        Config.Reset();
    }
}

