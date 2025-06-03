// PL/Enums.cs
using System.Collections;
using BO;

namespace PL;

/// <summary>
/// אוסף לבחירת סוג הצגת מתנדב (VolunteerInListEnum) עבור ComboBox.
/// </summary>
internal class VolunteerSelectMenuCollection : IEnumerable
{
    /// <summary>
    /// רשימת הערכים האפשריים של VolunteerInListEnum.
    /// </summary>
    static readonly IEnumerable<VolunteerInListEnum> s_enums =
        (Enum.GetValues(typeof(VolunteerInListEnum)) as IEnumerable<VolunteerInListEnum>)!;

    /// <summary>
    /// מחזיר Enumerator עבור הרשימה.
    /// </summary>
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

/// <summary>
/// אוסף סוגי שיחות (CallType) עבור ComboBox.
/// </summary>
internal class CallTypeCollection : IEnumerable
{
    /// <summary>
    /// רשימת הערכים האפשריים של CallType.
    /// </summary>
    static readonly IEnumerable<CallType> s_enums =
        (Enum.GetValues(typeof(CallType)) as IEnumerable<CallType>)!;

    /// <summary>
    /// מחזיר Enumerator עבור הרשימה.
    /// </summary>
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

/// <summary>
/// אוסף תפקידי מתנדבים (VolunteerRole) עבור ComboBox.
/// </summary>
internal class VolunteerRoleCollection : IEnumerable
{
    /// <summary>
    /// רשימת הערכים האפשריים של VolunteerRole.
    /// </summary>
    static readonly IEnumerable<VolunteerRole> s_enums =
        (Enum.GetValues(typeof(VolunteerRole)) as IEnumerable<VolunteerRole>)!;

    /// <summary>
    /// מחזיר Enumerator עבור הרשימה.
    /// </summary>
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

/// <summary>
/// אוסף סוגי מרחק (DistanceKind) עבור ComboBox.
/// </summary>
internal class DistanceKindCollection : IEnumerable
{
    /// <summary>
    /// רשימת הערכים האפשריים של DistanceKind.
    /// </summary>
    static readonly IEnumerable<DistanceKind> s_enums =
        (Enum.GetValues(typeof(DistanceKind)) as IEnumerable<DistanceKind>)!;

    /// <summary>
    /// מחזיר Enumerator עבור הרשימה.
    /// </summary>
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

internal class CallInListFieldCollection : IEnumerable
{
    static readonly IEnumerable<BO.CallInListField> s_enums =
(Enum.GetValues(typeof(BO.CallInListField)) as IEnumerable<BO.CallInListField>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

