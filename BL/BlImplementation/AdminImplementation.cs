namespace BlImplementation;

using System;
using BlApi;
using Helpers;

internal class AdminImplementation : IAdmin
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void AdvanceTime(BO.TimeUnit timeUnit)
    {
        DateTime newTime = AdminManager.Now;

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

        AdminManager.UpdateClock(newTime);
    }

    public DateTime GetCurrentTime()
    {
        return AdminManager.Now;
    }

    public TimeSpan GetRiskTimeSpan()
    {
        return _dal.Config.RiskTimeSpan;
    }

    public void InitializeDatabase()
    {
        DalTest.Initialization.Do();
        AdminManager.UpdateClock(AdminManager.Now);
    }

    public void ResetDatabase()
    {
        _dal.ResetDB();
        AdminManager.UpdateClock(AdminManager.Now);
    }

    public void SetRiskTimeSpan(TimeSpan riskTimeSpan)
    {
        _dal.Config.RiskTimeSpan = riskTimeSpan;
    }
}
