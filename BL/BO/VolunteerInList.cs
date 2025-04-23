namespace BO;
public class VolunteerInList
{
    public int Id { get; init; }
    public string Name { get; }
    public bool IsActive {  get; }
    public int HandledCallsCount { get; }
    public int CancelledCallsCount { get; }
    public int ExpiredHandledCallsCount { get; }
    public int? CallInProgressId { get; }
    public CallType CallInProgressType { get; }

}
