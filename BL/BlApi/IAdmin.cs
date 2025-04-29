using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlApi
{
    public interface IAdmin
    {
        DateTime GetCurrentTime();
        void AdvanceTime(BO.TimeUnit timeUnit);
        TimeSpan GetRiskTimeSpan();
        void SetRiskTimeSpan(TimeSpan riskTimeSpan);
        void ResetDatabase();
        void InitializeDatabase();
    }
}
