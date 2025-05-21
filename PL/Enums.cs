// PL/Enums.cs
using System.Collections;
using BO;

namespace PL;
internal class VolunteerSelectMenuCollection : IEnumerable
{
    static readonly IEnumerable<VolunteerInListEnum> s_enums =
        (Enum.GetValues(typeof(VolunteerInListEnum)) as IEnumerable<VolunteerInListEnum>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

//namespace PL;
internal class CallTypeCollection : IEnumerable
{
    static readonly IEnumerable<CallType> s_enums =
        (Enum.GetValues(typeof(CallType)) as IEnumerable<CallType>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}
internal class VolunteerRoleCollection : IEnumerable
{
    static readonly IEnumerable<VolunteerRole> s_enums =
        (Enum.GetValues(typeof(VolunteerRole)) as IEnumerable<VolunteerRole>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}
internal class DistanceKindCollection : IEnumerable
{
    static readonly IEnumerable<DistanceKind> s_enums =
        (Enum.GetValues(typeof(DistanceKind)) as IEnumerable<DistanceKind>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}