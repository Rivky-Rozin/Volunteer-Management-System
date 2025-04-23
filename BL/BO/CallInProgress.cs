using DO;

namespace BO;

public class CallInProgress
{
    public int Id { get; init; }
    public int CallId { get; set; }
    CallType CallType { get; set; }
    public string? Description { get; set; }
    public string FullAddress { get; set; }
    //זמן פתיחה
    DateTime? OpenTime { get; set; }
    //זמן מקסימלי לסיום הקריאה
    DateTime? MaxResolutionTime { get; set; }
    //זמן כניסה לטיפול
    DateTime? EntryToTreatmentTime { get; set; }

    //מרחק קריאה מהמתנדב המטפל
    public double DistanceFromVolunteer { get; set; }

    Status status { get; set; }



}
