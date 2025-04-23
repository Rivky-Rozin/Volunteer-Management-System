namespace BO;
/// <summary>
/// Creates a new instance of <see cref="OpenCallInList"/>, representing a read-only open call available for selection
/// by a volunteer in the "Choose Call for Treatment" screen.
/// </summary>
/// <param name="id">Running identifier of the call. Taken from DO.Call.</param>
/// <param name="callType">Type of the call (e.g., Medical, Technical). Taken from DO.Call.</param>
/// <param name="description">Optional. A textual description of the call. Taken from DO.Call.</param>
/// <param name="fullAddress">Full address of the call location. Taken from DO.Call.</param>
/// <param name="openTime">Time the call was opened. Taken from DO.Call.</param>
/// <param name="maxEndTime">
/// Optional. The latest allowed time for the call to be completed. Taken from DO.Call.
/// </param>
/// <param name="distanceFromVolunteer">
/// Distance in kilometers between the volunteer and the call location.
/// Calculated in the logic layer based on the volunteer's current position when viewing the list.
/// </param>

internal class OpenCallInList
{
    public int Id { get; }
    public CallType CallType { get; }

    public string Description { get; }
    public string FullAddress { get; }
    public DateTime OpenTime { get; }
    public DateTime? MaxEndTime { get; }
    public double DistanceFromVolunteer { get; }

}
