namespace DO;
//enums
public enum MainMenu
{
    Exit,
    Calls,
    Volunteers,
    Assignments,
    Initialize,
    ViewAll,
    Config,
    Reset
}
public enum EntityMenu
{
    Exit,
    Create,
    Read,
    ReadAll,
    Update,
    Delete,
    DeleteAll
}

public enum DistanceKind
{
    Aerial,
    Ground
}

public enum VolunteerRole
{
    Manager,
    Regular
}

public enum TreatmentType { Treated, ManagerCancelled, UserCancelled, ExpiredCancel }
public enum CallType
{
    Technical,
    Food,
    Medical,
    Emergency,
    Other,
    None
}

public enum ConfigurationOption
{
    ExitSubmenu = 1,
    AdvanceClockByMinute = 2,
    AdvanceClockByHour = 3,
    AdvanceClockByDay = 4,
    AdvanceClockByYear = 5,
    ShowCurrentClock = 6,
    SetConfigurationVariable = 7,
    ShowConfigurationValue = 8,
    ResetAllConfigurations = 9
}

public enum MainMenuOption
{
    Exit = 0,
    DisplaySubmenuForEntityCall = 1,
    DisplaySubmenuForEntityAssignment = 2,
    DisplaySubmenuForEntityVolunteer = 3,
    InitializeData = 4,
    DisplayAllData = 5,
    DisplayConfigurationSubMenu = 6,
    ResetDatabase = 7
}

public enum CallMenuOption
{
    Exit = 0,
    Create = 1,
    ReadById = 2,
    ReadAll = 3,
    Update = 4,
    Delete = 5,
    DeleteAll = 6
}
public enum VolunteerMenuOption
{
    Exit = 0,
    Create = 1,
    ReadById = 2,
    ReadAll = 3,
    Update = 4,
    Delete = 5,
    DeleteAll = 6
}
public enum AssignmentMenuOption
{
    Exit = 0,
    Create = 1,
    ReadById = 2,
    ReadAll = 3,
    Update = 4,
    Delete = 5,
    DeleteAll = 6
}