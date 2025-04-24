namespace BO;
public class VolunteerInList
{
    public int Id { get; init; }
    public string Name { get; init; }
    public bool IsActive {  get; init; }
    public int HandledCallsCount { get; init; }
    public int CancelledCallsCount { get; init; }
    public int ExpiredHandledCallsCount { get; init; }
    public int? CallInProgressId { get; init; }
    public CallType CallInProgressType { get; init; }

}
