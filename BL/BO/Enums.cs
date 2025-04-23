namespace BO;

public enum VolunteerRole
{
    Manager,
    Regular
}

public enum DistanceKind
{
    Aerial,
    Ground
    
}
public enum Status
{
    AtRiskActiveCall,
    ActiveCall
}

public enum CallType
{
    Technical,
    Food,
    Medical,
    Emergency,
    Other
}

public enum TreatmentEndTypeEnum
{
    Completed,
    Canceled,
    Timeout,
    Escalated
}