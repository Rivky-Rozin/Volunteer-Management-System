namespace DalTest;

using Dal;
using DalApi;
using DO;
using Microsoft.VisualBasic;
using System;
using System.Reflection;

public static class Initialization
{
    private static ICall? s_dalCall = new CallImplementation(); //stage 1
    private static IAssignment? s_dalAssignment = new AssignmentImplementation(); //stage 1
    private static IVolunteer? s_dalVolunteer = new VolunteerImplementation(); //stage 1
    private static IConfig? s_dalConfig = new ConfigImplementation(); //stage 1
    private static readonly Random s_rand = new();



    private static void createVolunteers()
    {

        string[] volunteerNames = { "David Rosen", "Yael Cohen", "Moshe Levi", "Shiran Bar", "Noa Amar", "Lior Avraham" };
        string[] volunteerPhones = { "0501234567", "0527654321", "0549876543", "0531237894", "0505678910", "0523456789" };
        string[] volunteerEmails = { "david@example.com", "yael@example.com", "moshe@example.com", "shiran@example.com", "noa@example.com", "lior@example.com" };
        string[] volunteerAddresses = { "Jerusalem", "Tel Aviv", "Haifa", "Beer Sheva", "Ashdod", "Netanya" };

        for (int index = 0; index < volunteerNames.Length; index++)
        {
            int id;
            do
                id = s_rand.Next(200000000, 400000000);
            while (s_dalVolunteer!.Read(id) != null);
            string name = volunteerNames[index];
            string phone = volunteerPhones[index];
            string email = volunteerEmails[index];
            string? address = volunteerAddresses[index];
            double? latitude = index % 2 == 0 ? s_rand.NextDouble() * 2 + 31 : null; // בין 31 ל-33
            double? longitude = index % 2 == 0 ? s_rand.NextDouble() * 2 + 34 : null; // בין 34 ל-36

            Enum role = index % 5 == 0 ? VolunteerRole.Manager : VolunteerRole.Regular;
            bool isActive = index % 2 == 0;

            Enum distanceKind = index % 2 == 0 ? DistanceKind.Aerial : DistanceKind.Ground;
            double? maxDistance = index % 2 == 0 ? s_rand.Next(1, 50) : null; // טווח בין 1 ל-50 ק"מ
            //Console.WriteLine(name);
            s_dalVolunteer!.Create(new(id, name, phone, email, role, isActive, distanceKind, address, latitude, longitude, null, maxDistance));

        }
    }

    private static void createCalls()
    {
        // מערך כתובות אפשריות
        string[] addresses =
        {"123 Main Street", "45 Elm Avenue", "78 Oak Lane", "22 Maple Drive","56 Pine Boulevard", "89 Cedar Road", "101 Ash Street"};

        // מערך סוגי קריאות
        string[] callTypes = { "Technical", "Food", "Medical", "Emergency", "Other" };
            // לולאה ליצירת קריאות
            foreach (var address in addresses)
        {
            int id = 0;

            // בחירת מספר רנדומלי בין 0 ל-4
            int randomIndex = s_rand.Next(0, 5);

            // קבלת הערך ממערך סוגי הקריאות
            string selectedCallType = callTypes[randomIndex];

            // המרת המחרוזת לערך באנסום CallType
            Enum callType = (Enum)Enum.Parse(typeof(DO.CallType), selectedCallType);

            // יצירת תיאור רנדומלי
            string description = $"Call regarding {callType.ToString().ToLower()} support at {address}.";

            // הגדרת מיקום גיאוגרפי רנדומלי
            double latitude = s_rand.NextDouble() * 180 - 90;  // טווח עבור קווי רוחב (-90 עד 90)
            double longitude = s_rand.NextDouble() * 360 - 180; // טווח עבור קווי אורך (-180 עד 180)

            // יצירת זמן פתיחת הקריאה
            DateTime openTime = s_dalConfig.Clock.AddDays(-s_rand.Next(365 * 2)); // טווח של עד שנתיים אחורה
            openTime = openTime.AddMinutes(s_rand.Next(0, 1440)); // הוספת דקות רנדומליות ליום

            // יצירת זמן סגירה מקסימלי
            DateTime? maxCallTime = openTime.AddHours(s_rand.Next(1, 48)); // טווח של עד 48 שעות מסיום הקריאה

            // יצירת קריאה חדשה
            s_dalCall!.Create(new(id, callType, address, latitude, longitude, openTime, description, maxCallTime));

        }
    }


    private static void createAssignments()
    {
        // רשימות לדוגמה של מזהי מתנדבים ושיחות
        var volunteerIds = s_dalVolunteer!.ReadAll().Select(v => v.Id).ToList();
        var callIds = s_dalCall!.ReadAll().Select(c => c.Id).ToList();

        // בדיקת קיום רשומות נתונים קיימות
        if (!volunteerIds.Any() || !callIds.Any())
        {
            throw new InvalidOperationException("אין מספיק מתנדבים או שיחות במערכת ליצירת משימות.");
        }

        // יצירת משימות
        foreach (var volunteerId in volunteerIds)
        {
            int assignmentId = 0;

            // בחירת שיחה רנדומלית מתוך רשימת השיחות
            var callId = callIds[s_rand.Next(callIds.Count)];

            // יצירת תאריך התחלת הטיפול
            DateTime startTreatment = new DateTime(s_dalConfig.Clock.Year - 2, 1, 1); //stage 1
            int range = (s_dalConfig.Clock - startTreatment).Days; //stage 1
            startTreatment = startTreatment.AddDays(s_rand.Next(range));


            // יצירת תאריך סיום הטיפול (רנדומלי או null)
            DateTime? endTreatment = (s_rand.Next(0, 2) == 0) // רנדומליות לסיום הטיפול
                ? startTreatment.AddMinutes(s_rand.Next(10, 120)) // זמן טיפול הגיוני
                : null;

            // בחירת סוג טיפול (רנדומלי או null)
            Enum? treatmentType = (s_rand.Next(0, 2) == 0)
                ? (TreatmentType)s_rand.Next(Enum.GetValues(typeof(TreatmentType)).Length)
                : null;

            s_dalAssignment!.Create(new Assignment(assignmentId, volunteerId, callId, startTreatment, endTreatment, treatmentType));
        }

    }

    public static void Do(ICall? dalCall, IVolunteer? dalVolunteer, IAssignment? dalAssignment, IConfig? dalConfig)
    {
        s_dalCall = dalCall ?? throw new NullReferenceException("DAL object can not be null!"); //stage 1
        s_dalVolunteer = dalVolunteer ?? throw new NullReferenceException("DAL object can not be null!"); //stage 1                                                                                              
        s_dalAssignment = dalAssignment ?? throw new NullReferenceException("DAL object can not be null!"); //stage 1                                                                                              
        s_dalConfig = dalConfig ?? throw new NullReferenceException("DAL object can not be null!"); //stage 1                                                                                              

        Console.WriteLine("Reset Configuration values and List values...");
        s_dalConfig.Reset(); //stage 1
        s_dalCall.DeleteAll(); //stage 1
        s_dalVolunteer.DeleteAll(); //stage 1
        s_dalVolunteer.DeleteAll(); //stage 1


        Console.WriteLine("Initializing Students list ...");

        createVolunteers();
        createCalls();
        createAssignments();



    }
}
