using Helpers;
namespace BO;

/// <summary>
/// Creates a new instance of <see cref="ClosedCallInList"/> representing a read-only view of a closed call,
/// used for display in volunteer call history or similar lists.
/// </summary>
/// <param name="id">Running identifier of the call, taken from DO.Call.</param>
/// <param name="callType">Type of the call (e.g., Medical, Technical), taken from DO.Call.</param>
/// <param name="fullAddress">Full address where the call took place, taken from DO.Call.</param>
/// <param name="openTime">The timestamp when the call was initially opened, from DO.Call.</param>
/// <param name="entryToTreatmentTime">The timestamp when the volunteer began handling the call, from DO.Assignment.</param>
/// <param name="actualTreatmentEndTime">
/// Nullable. The timestamp when the treatment ended. Null if treatment has not been completed yet. From DO.Assignment.
/// </param>
/// <param name="treatmentEndType">
/// Nullable. Type indicating how the treatment ended (e.g., Completed, Canceled). From DO.Assignment.
/// </param>

public class ClosedCallInList
{

    public int Id { get; init; }

    public CallType CallType { get; init; }
    public string FullAddress { get; init; }
    public DateTime OpenTime { get; init; }
    public DateTime EntryToTreatmentTime { get; init; }
    public DateTime? ActualTreatmentEndTime { get; init; }
    public TreatmentEndTypeEnum? TreatmentEndType { get; init; }
    public override string ToString() => this.ToStringProperty();
}