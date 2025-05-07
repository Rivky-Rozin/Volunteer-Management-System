
namespace Dal;

using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Numerics;
using System.Xml.Linq;
using DalApi;
using DO;

internal class VolunteerImplementation : IVolunteer
{
    public void Create(Volunteer item)
    {
        XElement volunteersRootElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);
        volunteersRootElem.Add(createVolunteerElement(item));
        XMLTools.SaveListToXMLElement(volunteersRootElem, Config.s_volunteers_xml);
    }

    public void Delete(int id)
    {
        XElement volunteersRootElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);
        XElement? volunteerElem = volunteersRootElem.Elements()
            .FirstOrDefault(v => (int?)v.Element("Id") == id);
        if (volunteerElem == null)
            throw new DO.DalDoesNotExistException($"Volunteer with ID={id} does not exist");
        volunteerElem.Remove();
        XMLTools.SaveListToXMLElement(volunteersRootElem, Config.s_volunteers_xml);
    }

    public void DeleteAll()
    {
        XElement volunteersRootElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);
        volunteersRootElem.Elements().Remove();
        XMLTools.SaveListToXMLElement(volunteersRootElem, Config.s_volunteers_xml);
    }

    public Volunteer? Read(int id)
    {
        XElement? volunteerElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml).Elements().FirstOrDefault(st => (int?)st.Element("Id") == id);
        return volunteerElem is null ? null : getVolunteer(volunteerElem);
    }

    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        return XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml).Elements().Select(v => getVolunteer(v)).FirstOrDefault(filter);
    }

    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        XElement volunteersRootElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);
        var elements = volunteersRootElem.Elements();
        var volunteers = elements.Select(e => getVolunteer(e));
        return filter == null ? volunteers : volunteers.Where(filter);
    }

    public void Update(Volunteer item)
    {
        XElement volunteersRootElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);

        (volunteersRootElem.Elements().FirstOrDefault(v => (int?)v.Element("Id") == item.Id)
        ?? throw new DO.DalDoesNotExistException($"Volunteer with ID={item.Id} does Not exist"))
                .Remove();

        volunteersRootElem.Add(new XElement("Volunteer", createVolunteerElement(item)));

        XMLTools.SaveListToXMLElement(volunteersRootElem, Config.s_volunteers_xml);
    }

    private XElement createVolunteerElement(Volunteer item)
    {
        // יצירת אלמנט XML חדש עם כל השדות של ה-Volunteer
        return new XElement("Volunteer",
            new XElement("Id", item.Id),
            new XElement("Name", item.Name),
           new XElement("Phone", item.Phone),
            new XElement("Email", item.Email),
            new XElement("Role", item.Role),
            new XElement("IsActive", item.IsActive),
            new XElement("DistanceKind", item.DistanceKind),
           !string.IsNullOrEmpty(item.Address) ? new XElement("Address", item.Address) : null,
           item.Latitude.HasValue ? new XElement("Latitude", item.Latitude) : null,
           item.Longitude.HasValue ? new XElement("Longitude", item.Longitude) : null,
            !string.IsNullOrEmpty(item.Password) ? new XElement("Password", item.Password) : null,
           item.MaxDistance.HasValue ? new XElement("MaxDistance", item.MaxDistance) : null
        );
    }

    static Volunteer getVolunteer(XElement v)
    {

        return new DO.Volunteer()
        {

            Id = v.ToIntNullable("Id") ?? throw new FormatException("can't convert Id"),
            Name = (string?)v.Element("Name") ?? "",
            Phone = (string?)v.Element("Phone") ?? "",
            Email = (string?)v.Element("Email") ?? "",
            Role = v.ToEnumNullable<VolunteerRole>("Role") ?? throw new FormatException("can't convert Role"),
            IsActive = (bool?)v.Element("IsActive") ?? false,
            DistanceKind = v.ToEnumNullable<DistanceKind>("DistanceKind") ?? throw new FormatException("can't convert DistanceKind"),
            Address = (string?)v.Element("Address"),
            Latitude = v.ToDoubleNullable("Latitude"),
            Longitude = v.ToDoubleNullable("Longitude"),
            Password = (string?)v.Element("Password"),
            MaxDistance = v.ToDoubleNullable("MaxDistance")
        };
    }
}
