// PL/Enums.cs
using System.Collections;
using BO;

namespace PL;
internal class VolunteerSelectMenuCollection : IEnumerable
{
    static readonly IEnumerable<VolunteerSelectMenu> s_enums =
        (Enum.GetValues(typeof(VolunteerSelectMenu)) as IEnumerable<VolunteerSelectMenu>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}
