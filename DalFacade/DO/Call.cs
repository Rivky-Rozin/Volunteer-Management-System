
namespace DO;

/// <summary>
/// ישות קריאה הכוללת פרטים עבור "קריאה", כולל מספר מזהה רץ ייחודי.
/// </summary>
/// <param name="Id">Unique ID of the call (auto-incremented).</param>
/// <param name="Type">Type of the call (e.g., Technical, Food, etc.).</param>
/// <param name="Description">Description of the call.</param>
/// <param name="Address">Address where the call was placed.</param>
/// <param name="Latitude">Latitude of the call location.</param>
/// <param name="Longitude">Longitude of the call location.</param>
/// <param name="OpeningTime">
/// The time (date and hour) when the call was opened by the manager.
/// This marks the creation time of the current call entity and will be set according to the system clock.
/// </param>
/// <param name="MaxClosingTime">
/// The maximum allowed time (deadline) for completing the call.
/// If this value is null, the call has no specific deadline. 
/// Tasks approaching this time within the configured risk time range will be marked as "at risk" in the logical layer.
/// This value must be greater than the OpeningTime (validated in the logical layer).
/// </param>


public record Call(
    int Id,
    Enum CallType,
    string FullAddress,
    double Latitude,
    double Longitude,
    DateTime OpenTime,
    string? Description = null,
    DateTime? MaxCallTime = null
    )

{
    public Call() : this(0, default, string.Empty, 0.0, 0.0, DateTime.Now, string.Empty, DateTime.Now) { }
}
