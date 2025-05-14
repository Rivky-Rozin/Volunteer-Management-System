namespace BlApi;

using System;



public interface IAdmin
{
    #region Stage 5
    void AddConfigObserver(Action configObserver);
    void RemoveConfigObserver(Action configObserver);
    void AddClockObserver(Action clockObserver);
    void RemoveClockObserver(Action clockObserver);
    #endregion Stage 5
    DateTime GetCurrentTime();
    void AdvanceTime(BO.TimeUnit timeUnit);
    TimeSpan GetRiskTimeSpan();
    void SetRiskTimeSpan(TimeSpan riskTimeSpan);
    TimeSpan GetTreatmentTime();
    void SetTreatmentTime(TimeSpan treatmentTime);
    void ResetDatabase();
    void InitializeDatabase();
}
