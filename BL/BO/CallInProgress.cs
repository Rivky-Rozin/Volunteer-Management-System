using DO;
using Helpers;

namespace BO;

public class CallInProgress
{
    public int Id { get; init; }
    public int CallId { get; set; }
    public CallType CallType { get; set; }
    public string? Description { get; set; }
    public string FullAddress { get; set; }
    //זמן פתיחה
    public DateTime? OpenTime { get; set; }
    //זמן מקסימלי לסיום הקריאה
    public DateTime? MaxResolutionTime { get; set; }
    //זמן כניסה לטיפול
    public DateTime? EntryToTreatmentTime { get; set; }

    //מרחק קריאה מהמתנדב המטפל
    public double DistanceFromVolunteer { get; set; }

    public CallInProgressStatus status { get; set; }

    public override string ToString() => this.ToStringProperty();
}
