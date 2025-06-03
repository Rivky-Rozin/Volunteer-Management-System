namespace BO;


public enum VolunteerRole
{
    Manager,
    Regular,
    None
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

public enum TimeUnit
{
    Minute,
    Hour,
    Day,
    Month,
    Year
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
    NumberOfAssignments,
    None
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

public enum ClosedCallInListEnum
{
    Id,
    CallType,
    FullAddress,
    OpenTime,
    EntryToTreatmentTime,
    ActualTreatmentEndTime,
    TreatmentEndType
}

public enum VolunteerInListEnum
{
    Id,
    Name,
    IsActive,
    HandledCallsCount,
    CancelledCallsCount,
    ExpiredHandledCallsCount,
    CallInProgressId,
    CallInProgressType,
    None
}
