namespace BO;

public class CallInList
{
    public int? Id { get; init; }
    public int CallId { get; init; }
    public CallType CallType { get; init; }
    public DateTime OpenTime { get; init; }
    public TimeSpan? TimeUntilAssigning { get; init; }
    public string? LastVolunteerName { get; init; }
    public TimeSpan? totalTreatmentTime { get; init; }
    public CallStatus Status { get; init; }
    public int NumberOfAssignments { get; init; }
}
