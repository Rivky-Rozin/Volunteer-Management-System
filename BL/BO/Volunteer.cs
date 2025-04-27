
using Helpers;

namespace BO;
public class Volunteer
{
    public int Id { get; init; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string? Password { get; set; }
    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public VolunteerRole Role { get; set; }
    public bool IsActive { get; set; }
    public double? MaxDistance { get; set; }
    public DistanceKind? DistanceKind { get; set; }
    public int HandledCallsCount { get; init; }
    public int CancelledCallsCount { get; init; }
    public int ExpiredHandledCallsCount { get; init; }
    public BO.CallInProgress? CallInProgress { get; set; }
    public override string ToString() => this.ToStringProperty();
}

