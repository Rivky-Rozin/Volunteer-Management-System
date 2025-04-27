using Helpers;
namespace BO;
public class CallAssignInList
{
    public int? VolunteerId { get; init; }
    public string? VolunteerName { get; init; }
    public DateTime StartTreatmentTime { get; init; }
    public DateTime? FinishTreatmentTime { get; init; }
    public EndOfTreatmentType? EndOfTreatmentType { get; init; }
    public override string ToString() => this.ToStringProperty();
}
