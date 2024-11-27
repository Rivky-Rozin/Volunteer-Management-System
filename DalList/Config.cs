

using System.Reflection.Metadata;

namespace Dal;
internal static class Config
{
    
    //מספור רץ למחלקת הקריאה
    internal const int startCallId = 1000;
    private static int nextCallId = startCallId;
    public static int NextCallId { get => nextCallId++; }
    
    //מספור רץ למחלקת השמה
    internal const int StartAssignmentId = 0;
    private static int nextAssignmentId = StartAssignmentId;
    public static int  NextAssignmentId { get => nextAssignmentId++; }

    //שעון המערכת
    internal static DateTime Clock { get; set; } = DateTime.Now;

    //??? עדיין לא למדנו מה זה
    //כאן צריך להיות אתחול השדה timespan
    static TimeSpan RiskRange;
    internal static void Reset()
    {
        nextCallId = startCallId;
        nextAssignmentId = StartAssignmentId;    
        Clock = DateTime.Now;
        //כאן צריך להיות אתחול השדה timespan
    }

}
