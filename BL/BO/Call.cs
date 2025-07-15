
using Helpers;

namespace BO;
public class Call
{
    public int Id { get; init; }
    public CallType CallType { get; set; }   
    public string? Description {  get; set; }
    public string Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime? MaxFinishTime { get; set; }
    public CallStatus Status { get; set; }
    public List<BO.CallAssignInList>? Assignments { get; set; }
    public override string ToString() => this.ToStringProperty();
}
