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
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
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
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.InitializeDB(); //stage 7
    }

    public void ResetDatabase()
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.ResetDB(); //stage 7
    }

    public void SetRiskTimeSpan(TimeSpan riskTimeSpan)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.RiskTimeSpan = riskTimeSpan;
    }

    public TimeSpan GetTreatmentTime()
    {
        return AdminManager.TreatmentTime;
    }

    public void SetTreatmentTime(TimeSpan treatmentTime)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.TreatmentTime = treatmentTime;
    }

    // stage 7
    public void StartSimulator(int interval)  //stage 7
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.Start(interval); //stage 7
    }
    public void StopSimulator()
        => AdminManager.Stop(); //stage 7
}
