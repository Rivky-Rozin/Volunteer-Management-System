

using System.Reflection.Metadata;
using System.Runtime.CompilerServices;

namespace Dal;
internal static class Config
{
    
    //מספור רץ למחלקת הקריאה
    internal const int startCallId = 1000;
    private static int nextCallId = startCallId;

    public static int NextCallId
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get
        {
            return nextCallId++;
        }
    }


    //מספור רץ למחלקת השמה
    internal const int StartAssignmentId = 1000;
    private static int nextAssignmentId = StartAssignmentId;
    public static int  NextAssignmentId
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get
        {
            return nextAssignmentId++;
        }
    }

    private static DateTime clock = DateTime.Now;
    private static TimeSpan treatmentTime;
    private static TimeSpan riskTimeSpan;

    internal static DateTime Clock
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => clock;
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => clock = value;
    }

    internal static TimeSpan TreatmentTime
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => treatmentTime;
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => treatmentTime = value;
    }

    internal static TimeSpan RiskTimeSpan
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => riskTimeSpan;
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => riskTimeSpan = value;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    internal static void Reset()
    {
        nextCallId = startCallId;
        nextAssignmentId = StartAssignmentId;
        Clock = DateTime.Now;
        treatmentTime = new TimeSpan(2, 0, 0);
        riskTimeSpan = new TimeSpan(2, 0, 0);
    }
}
