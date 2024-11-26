

using System.Reflection.Metadata;

namespace Dal;
internal static class Config
{
    
    internal const int StartCallId = 0;
    private static int NextCallId = StartCallId;
    public static int NEXTCALLID { get { int temp = NextCallId; NextCallId++; return temp; } }
    
    internal const int StartAssignmentId = 0;
    private static int NextAssignmentId = StartAssignmentId;
    public static int NEXTASSIGNMENTID { get { int temp = NextAssignmentId; NextAssignmentId++; return temp; } }

    static DateTime Clock;
    static TimeSpan RiskRange;
}
