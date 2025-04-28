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

public enum CallType
{
    Technical,
    Food,
    Medical,
    Emergency,
    Other,
    None
}

//איפה משתמשים?
public enum Status
{
    Open,
    InProgress,
    Closed
}

public enum CallInProgressStatus
{
    InProgress,
    InProgressAtRisk
}


public enum TreatmentEndTypeEnum
{
    Completed,
    Canceled,
    Timeout,
    Escalated
}

public enum CallStatus
{
    Open,
    InProgress,
    Closed,
    Expired,
    OpenAtRisk,
    InProgressAtRisk
}

public enum EndOfTreatmentType
{
    Simple,
    Complex,
    Emergency
}

public enum TimeUnit
{
    Minute,
    Hour,
    Day,
    Month,
    Year
}