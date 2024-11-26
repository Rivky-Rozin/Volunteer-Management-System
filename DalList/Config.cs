

using System.Reflection.Metadata;

namespace Dal;
internal static class Config
{
    

    internal const int startCallId = 1000;
    private static int nextCallId = startCallId;
    public static int NextCallId { get => nextCallId++; }
    
    internal const int StartAssignmentId = 0;
    private static int NextAssignmentId = StartAssignmentId;
    public static int NEXTASSIGNMENTID { get { int temp = NextAssignmentId; NextAssignmentId++; return temp; } }

    static DateTime Clock;
    static TimeSpan RiskRange;
}
