namespace BlImplementation;

using System;
using BlApi;
using Helpers;

internal class AdminImplementation : IAdmin
{
    #region Stage 5
    public void AddClockObserver(Action clockObserver) =>
    AdminManager.ClockUpdatedObservers += clockObserver;
    public void RemoveClockObserver(Action clockObserver) =>
    AdminManager.ClockUpdatedObservers -= clockObserver;
    public void AddConfigObserver(Action configObserver) =>
   AdminManager.ConfigUpdatedObservers += configObserver;
    public void RemoveConfigObserver(Action configObserver) =>
    AdminManager.ConfigUpdatedObservers -= configObserver;
    #endregion Stage 5
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
        return AdminManager.RiskTimeSpan;
    }

    public void InitializeDatabase()
    {
        //DalTest.Initialization.Do();
        //AdminManager.UpdateClock(AdminManager.Now); //ADMINMANAGER כבר עושה את זה
        AdminManager.InitializeDB();
    }

    public void ResetDatabase()
    {
        AdminManager.ResetDB();
        //AdminManager.UpdateClock(AdminManager.Now); //ADMINMANAGER כבר עושה את זה
    }

    public void SetRiskTimeSpan(TimeSpan riskTimeSpan)
    {
        AdminManager.RiskTimeSpan = riskTimeSpan;
    }

    public TimeSpan GetTreatmentTime()
    {
        return AdminManager.TreatmentTime;
    }

    public void SetTreatmentTime(TimeSpan treatmentTime)
    {
        AdminManager.TreatmentTime = treatmentTime;
    }
}
