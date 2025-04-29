

namespace BlImplementation;

using System;
using BlApi;
//using BO;

using Helpers;

internal class AdminImplementation : IAdmin
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void AdvanceTime(BO.TimeUnit timeUnit)
    {
        DateTime newTime = ClockManager.Now;

        switch (timeUnit)
        {
            case BO.TimeUnit.Minute:
                newTime = newTime.AddMinutes(1);
                break;
            case BO.TimeUnit.Hour:
                newTime = newTime.AddHours(1);
                break;
            case BO.TimeUnit.Day:
                newTime = newTime.AddDays(1);
                break;
            case BO.TimeUnit.Month:
                newTime = newTime.AddMonths(1);
                break;
            case BO.TimeUnit.Year:
                newTime = newTime.AddYears(1);
                break;
        }

        ClockManager.UpdateClock(newTime);
    }

    public DateTime GetCurrentTime()
    {
        return ClockManager.Now;
    }

    public TimeSpan GetRiskTimeSpan()
    {
        return _dal.Config.RiskTimeSpan;
    }

    public void InitializeDatabase()
    {
        DalTest.Initialization.Do();
        ClockManager.UpdateClock(ClockManager.Now);
    }

    public void ResetDatabase()
    {
        _dal.ResetDB();
        ClockManager.UpdateClock(ClockManager.Now);
    }

    public void SetRiskTimeSpan(TimeSpan riskTimeSpan)
    {
        _dal.Config.RiskTimeSpan = riskTimeSpan;
    }
}
