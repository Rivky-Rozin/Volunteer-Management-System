
namespace DO;
/// <summary>
/// Default constructor for the Assignment entity.
/// Initializes all properties with default values.
/// </summary>
/// <param name="callId">Unique ID of the call.</param>
/// <param name="volunteerId">Unique ID of the volunteer.</param>
/// <param name="startTreatment">Start time of the treatment.</param>
/// <param name="endTreatment">End time of the treatment (nullable).</param>
/// <param name="treatmentType">The type of treatment's conclusion.</param>
public record Assignment
(
   int Id,
   int VolunteerId,
   int CallId,
   DateTime StartTreatment ,
   DateTime? EndTreatment=null ,
   TreatmentType? TreatmentType=null,
   DateTime? EndTreatmentTime = null
)
{
    //public DateTime EndTreatmentTime;

    public Assignment()
    : this(0, 0, 0,DateTime.Now, null, default) { }

    public bool ShouldSerializeTreatmentType() => TreatmentType.HasValue;
    public bool ShouldSerializeEndTreatment() => EndTreatment.HasValue;
}


