namespace BlApi;

using System;



public interface IAdmin
{
    DateTime GetCurrentTime();
    void AdvanceTime(BO.TimeUnit timeUnit);
    TimeSpan GetRiskTimeSpan();
    void SetRiskTimeSpan(TimeSpan riskTimeSpan);
    void ResetDatabase();
    void InitializeDatabase();
}
