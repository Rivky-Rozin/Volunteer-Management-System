namespace DalTest;

using Dal;
using DalApi;
using DO;
using Microsoft.VisualBasic;
using System;
using System.Reflection;

public static class Initialization
{
    private static IDal? s_dal; //stage 2
    private static readonly Random s_rand = new();
    private const int MIN_ID = 200000000;
    private const int MAX_ID = 400000000;
    public static void Do()//stage 4 
    {
        //מאותחל לפי מחלקת הפקטורי ששולחת את המחלקה המתאימה לפי קובץ הקונפיגורציה
        s_dal = DalApi.Factory.Get; // stage 4

        Console.WriteLine("Reset Configuration values and List values...");

        s_dal.ResetDB();//stage 

        Console.WriteLine("Initializing All lists ...");

        createVolunteers();
        createCalls();
        createAssignments();

    }


    private static void createVolunteers()
    {

        string[] volunteerNames = { "David Rosen", "Yael Cohen", "Moshe Levi", "Shiran Bar", "Noa Amar", "Lior Avraham" };
        string[] volunteerPhones = { "0501234567", "0527654321", "0549876543", "0531237894", "0505678910", "0523456789" };
        string[] volunteerEmails = { "david@example.com", "yael@example.com", "moshe@example.com", "shiran@example.com", "noa@example.com", "lior@example.com" };
        string[] volunteerAddresses = {
          "10 King David St, Jerusalem, Israel",
          "48 Allenby St, Tel Aviv-Yafo, Israel",
          "40 HaNassi Blvd, Haifa, Israel",
          "31 Rager Blvd, Beer Sheva, Israel",
          "1 Herzl St, Ashdod, Israel",
          "8 HaRav Kook St, Netanya, Israel"
        };
        for (int index = 0; index < volunteerNames.Length; index++)
        {
            int id;
            do
                id = s_rand.Next(200000000, 400000000);
            while (s_dal!.Volunteer.Read(id) != null);
            string name = volunteerNames[index];
            string phone = volunteerPhones[index];
            string email = volunteerEmails[index];
            string? address = volunteerAddresses[index];
            double? latitude = s_rand.NextDouble() * 2 + 31;
            double? longitude = s_rand.NextDouble() * 2 + 34;

            VolunteerRole role = index % 5 == 0 ? VolunteerRole.Manager : VolunteerRole.Regular;
            bool isActive = true;

            DistanceKind distanceKind = index % 2 == 0 ? DistanceKind.Aerial : DistanceKind.Ground;
            double? maxDistance = index % 2 == 0 ? s_rand.Next(1, 50) : null; // טווח בין 1 ל-50 ק"מ
            //Console.WriteLine(name);
            s_dal!.Volunteer.Create(new(id, name, phone, email, role, isActive, distanceKind, address, latitude, longitude, null, maxDistance));

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
            DO.CallType callType = (DO.CallType)Enum.GetValues(typeof(DO.CallType))
                                                .GetValue(s_rand.Next(0, Enum.GetValues(typeof(DO.CallType)).Length));

            // יצירת תיאור רנדומלי
            string description = $"Call regarding {callType.ToString().ToLower()} support at {address}.";

            // הגדרת מיקום גיאוגרפי רנדומלי
            double latitude = s_rand.NextDouble() * 180 - 90;  // טווח עבור קווי רוחב (-90 עד 90)
            double longitude = s_rand.NextDouble() * 360 - 180; // טווח עבור קווי אורך (-180 עד 180)

            // יצירת זמן פתיחת הקריאה
            DateTime openTime = s_dal!.Config.Clock.AddDays(-s_rand.Next(365 * 2)); // טווח של עד שנתיים אחורה
            openTime = openTime.AddMinutes(s_rand.Next(0, 1440)); // הוספת דקות רנדומליות ליום

            // יצירת זמן סגירה מקסימלי
            DateTime? maxCallTime = openTime.AddHours(s_rand.Next(1, 48)); // טווח של עד 48 שעות מסיום הקריאה

            // יצירת קריאה חדשה
            s_dal!.Call.Create(new(id, callType, address, latitude, longitude, openTime, description, maxCallTime));

        }
    }
    private static void createAssignments()
    {
        // רשימות לדוגמה של מזהי מתנדבים ושיחות
        var volunteerIds = s_dal!.Volunteer.ReadAll().Select(v => v.Id).ToList();
        var callIds = s_dal!.Call.ReadAll().Select(c => c.Id).ToList();



        // יצירת משימות
        foreach (var volunteerId in volunteerIds)
        {
            int assignmentId = 0;

            // בחירת שיחה רנדומלית מתוך רשימת השיחות
            var callId = callIds[s_rand.Next(callIds.Count)];

            // יצירת תאריך התחלת הטיפול
            DateTime startTreatment = new DateTime(s_dal!.Config.Clock.Year - 2, 1, 1); //stage 1
            int range = (s_dal!.Config.Clock - startTreatment).Days; //stage 1
            startTreatment = startTreatment.AddDays(s_rand.Next(range));


            // יצירת תאריך סיום הטיפול (רנדומלי או null)
            DateTime? endTreatment = (s_rand.Next(0, 2) == 0) // רנדומליות לסיום הטיפול
                ? startTreatment.AddMinutes(s_rand.Next(10, 120)) // זמן טיפול הגיוני
                : null;

            // בחירת סוג טיפול (רנדומלי או null)
            TreatmentType? treatmentType = (s_rand.Next(0, 2) == 0)
                ? (TreatmentType)s_rand.Next(Enum.GetValues(typeof(TreatmentType)).Length)
                : null;

            s_dal!.Assignment.Create(new Assignment(assignmentId, volunteerId, callId, startTreatment, endTreatment, treatmentType));
        }

    }

}
