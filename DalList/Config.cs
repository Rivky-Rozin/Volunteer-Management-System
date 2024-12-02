

using System.Reflection.Metadata;

namespace Dal;
internal static class Config
{
    
    //מספור רץ למחלקת הקריאה
    internal const int startCallId = 1000;
    private static int nextCallId = startCallId;
    public static int NextCallId { get => nextCallId++; }
    
    //מספור רץ למחלקת השמה
    internal const int StartAssignmentId = 1000;
    private static int nextAssignmentId = StartAssignmentId;
    public static int  NextAssignmentId { get => nextAssignmentId++; }

    //שעון המערכת
    internal static DateTime Clock { get; set; } = DateTime.Now;

    internal static void Reset()
    {
        nextCallId = startCallId;
        nextAssignmentId = StartAssignmentId;    
        Clock = DateTime.Now;
        //כאן צריך להיות אתחול השדה timespan
    }
    //פונקציות שנותנות ערכים חדשים למספרים הרצים
    internal static void SetNextCallId(int newValue)
    {
        nextCallId=newValue;    
    }
    internal static void SetNextAssignmentId(int newValue)
    {
        nextAssignmentId = newValue;
    }
    //פונקציות שמציגות את המשתנים הרצים
    internal static void showNextCallId()
    {
        Console.WriteLine(nextCallId);
    }
    internal static void ShowNextAssignmentId()
    {
        Console.WriteLine(nextAssignmentId);
    }

}
