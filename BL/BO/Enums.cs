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

public enum CallField
{
    Id,
    CallType,
    FullAddress,
    OpenTime,
    EntryToTreatmentTime,
    ActualTreatmentEndTime,
    TreatmentEndType
}


public enum CallInListField
{
    Id,
    CallId,
    CallType,
    OpenTime,
    TimeUntilAssigning,
    LastVolunteerName,
    totalTreatmentTime,
    Status,
    NumberOfAssignments
}

public enum OpenCallInListEnum
{
    Id,
    CallType,
    Description,
    FullAddress,
    OpenTime,
    MaxEndTime,
    DistanceFromVolunteer
}
