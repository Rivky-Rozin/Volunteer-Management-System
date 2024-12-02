
namespace DO;
/// <summary>
/// Represents a volunteer and his various attributes.
/// </summary>
/// <param name="Id">A unique identifier for the volunteer.</param>
/// <param name="Name">The full name of the volunteer.</param>
/// <param name="Phone">The phone number of the volunteer.</param>
/// <param name="Email">The email address of the volunteer.</param>
/// <param name="Password">The password of the volunteer. Can be null if not set.</param>
/// <param name="Address">The home address of the volunteer. Can be null if not set.</param>
/// <param name="Latitude">The latitude of the volunteer's location.</param>
/// <param name="Longitude">The longitude (east-west coordinate) of the volunteer's location.</param>
/// <param name="Role">The role of the volunteer (e.g., manager, regular volunteer).</param>
/// <param name="IsActive">Indicates whether the volunteer is active. True if active, false otherwise.</param>
/// <param name="MaxDistance">The maximum distance (in kilometers) the volunteer can operate. Can be null if not defined.</param>
/// <param name="DistanceKind">The type of distance: aerial or ground-based.</param>

public record Volunteer
    (
    int Id,
    string Name,
    string Phone,
    string Email,
    VolunteerRole Role,
    bool IsActive,
    DistanceKind DistanceKind,
    string? Address = null,
    double? Latitude = null,
    double? Longitude = null,
    string? Password = null,
    double? MaxDistance = null
    )
{
    public Volunteer() : this(0,"","","",default,false,default,"",0.0,0.0,"",0.0)
    {
    }
}
