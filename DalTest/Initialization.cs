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


    //private static void createVolunteers()
    //{

    //    string[] volunteerNames = { "David Rosen", "Yael Cohen", "Moshe Levi", "Shiran Bar", "Noa Amar", "Lior Avraham" };
    //    string[] volunteerPhones = { "0501234567", "0527654321", "0549876543", "0531237894", "0505678910", "0523456789" };
    //    string[] volunteerEmails = { "david@example.com", "yael@example.com", "moshe@example.com", "shiran@example.com", "noa@example.com", "lior@example.com" };
    //    string[] volunteerAddresses = {
    //      "10 King David St, Jerusalem, Israel",
    //      "48 Allenby St, Tel Aviv-Yafo, Israel",
    //      "40 HaNassi Blvd, Haifa, Israel",
    //      "31 Rager Blvd, Beer Sheva, Israel",
    //      "1 Herzl St, Ashdod, Israel",
    //      "8 HaRav Kook St, Netanya, Israel"
    //    };
    //    for (int index = 0; index < volunteerNames.Length; index++)
    //    {
    //        int id;
    //        do
    //            id = s_rand.Next(200000000, 400000000);
    //        while (s_dal!.Volunteer.Read(id) != null);
    //        string name = volunteerNames[index];
    //        string phone = volunteerPhones[index];
    //        string email = volunteerEmails[index];
    //        string? address = volunteerAddresses[index];
    //        double? latitude = s_rand.NextDouble() * 2 + 31;
    //        double? longitude = s_rand.NextDouble() * 2 + 34;

    //        VolunteerRole role = index % 5 == 0 ? VolunteerRole.Manager : VolunteerRole.Regular;
    //        bool isActive = true;

    //        DistanceKind distanceKind = index % 2 == 0 ? DistanceKind.Aerial : DistanceKind.Ground;
    //        double? maxDistance = index % 2 == 0 ? s_rand.Next(1, 50) : null; // טווח בין 1 ל-50 ק"מ
    //        //Console.WriteLine(name);
    //        s_dal!.Volunteer.Create(new(id, name, phone, email, role, isActive, distanceKind, address, latitude, longitude, null, maxDistance));

    //    }
    //}
    //private static void createCalls()
    //{
    //    // מערך כתובות אפשריות
    //    string[] addresses =
    //    {"123 Main Street", "45 Elm Avenue", "78 Oak Lane", "22 Maple Drive","56 Pine Boulevard", "89 Cedar Road", "101 Ash Street"};

    //    // מערך סוגי קריאות
    //    string[] callTypes = { "Technical", "Food", "Medical", "Emergency", "Other" };
    //    // לולאה ליצירת קריאות
    //    foreach (var address in addresses)
    //    {
    //        int id = 0;

    //        // בחירת מספר רנדומלי בין 0 ל-4
    //        int randomIndex = s_rand.Next(0, 5);

    //        // קבלת הערך ממערך סוגי הקריאות
    //        string selectedCallType = callTypes[randomIndex];
    //        DO.CallType callType = (DO.CallType)Enum.GetValues(typeof(DO.CallType))
    //                                            .GetValue(s_rand.Next(0, Enum.GetValues(typeof(DO.CallType)).Length));

    //        // יצירת תיאור רנדומלי
    //        string description = $"Call regarding {callType.ToString().ToLower()} support at {address}.";

    //        // הגדרת מיקום גיאוגרפי רנדומלי
    //        double latitude = s_rand.NextDouble() * 180 - 90;  // טווח עבור קווי רוחב (-90 עד 90)
    //        double longitude = s_rand.NextDouble() * 360 - 180; // טווח עבור קווי אורך (-180 עד 180)

    //        // יצירת זמן פתיחת הקריאה
    //        DateTime openTime = s_dal!.Config.Clock.AddDays(-s_rand.Next(365 * 2)); // טווח של עד שנתיים אחורה
    //        openTime = openTime.AddMinutes(s_rand.Next(0, 1440)); // הוספת דקות רנדומליות ליום

    //        // יצירת זמן סגירה מקסימלי
    //        DateTime? maxCallTime = openTime.AddHours(s_rand.Next(1, 48)); // טווח של עד 48 שעות מסיום הקריאה

    //        // יצירת קריאה חדשה
    //        s_dal!.Call.Create(new(id, callType, address, latitude, longitude, openTime, description, maxCallTime));

    //    }
    //}
    //private static void createAssignments()
    //{
    //    // רשימות לדוגמה של מזהי מתנדבים ושיחות
    //    var volunteerIds = s_dal!.Volunteer.ReadAll().Select(v => v.Id).ToList();
    //    var callIds = s_dal!.Call.ReadAll().Select(c => c.Id).ToList();



    //    // יצירת משימות
    //    foreach (var volunteerId in volunteerIds)
    //    {
    //        int assignmentId = 0;

    //        // בחירת שיחה רנדומלית מתוך רשימת השיחות
    //        var callId = callIds[s_rand.Next(callIds.Count)];

    //        // יצירת תאריך התחלת הטיפול
    //        DateTime startTreatment = new DateTime(s_dal!.Config.Clock.Year - 2, 1, 1); //stage 1
    //        int range = (s_dal!.Config.Clock - startTreatment).Days; //stage 1
    //        startTreatment = startTreatment.AddDays(s_rand.Next(range));


    //        // יצירת תאריך סיום הטיפול (רנדומלי או null)
    //        DateTime? endTreatment = (s_rand.Next(0, 2) == 0) // רנדומליות לסיום הטיפול
    //            ? startTreatment.AddMinutes(s_rand.Next(10, 120)) // זמן טיפול הגיוני
    //            : null;

    //        // בחירת סוג טיפול (רנדומלי או null)
    //        TreatmentType? treatmentType = (s_rand.Next(0, 2) == 0)
    //            ? (TreatmentType)s_rand.Next(Enum.GetValues(typeof(TreatmentType)).Length)
    //            : null;

    //        s_dal!.Assignment.Create(new Assignment(assignmentId, volunteerId, callId, startTreatment, endTreatment, treatmentType));
    //    }

    //}
    private static void createVolunteers()
    {
        var locations = new[]
        {
        new { Address = "10 King David St, Jerusalem", Latitude = 31.7767, Longitude = 35.2296 },
        new { Address = "48 Allenby St, Tel Aviv", Latitude = 32.0635, Longitude = 34.7717 },
        new { Address = "1 Herzl St, Ashdod", Latitude = 31.8032, Longitude = 34.6553 },
        new { Address = "40 HaNassi Blvd, Haifa", Latitude = 32.7940, Longitude = 34.9896 },
        new { Address = "31 Rager Blvd, Beer Sheva", Latitude = 31.2518, Longitude = 34.7913 },
        new { Address = "8 HaRav Kook St, Netanya", Latitude = 32.3282, Longitude = 34.8560 },
        new { Address = "2 Weizmann St, Rehovot", Latitude = 31.8936, Longitude = 34.8113 },
        new { Address = "HaPalmach 2, Tiberias", Latitude = 32.7922, Longitude = 35.5315 },
        new { Address = "Herzl Blvd 1, Holon", Latitude = 32.0144, Longitude = 34.7744 },
        new { Address = "4 Jabotinsky St, Bnei Brak", Latitude = 32.0881, Longitude = 34.8396 },
        new { Address = "Rabin Square, Tel Aviv", Latitude = 32.0809, Longitude = 34.7806 },
        new { Address = "Yefet St 190, Jaffa", Latitude = 32.0495, Longitude = 34.7516 },
        new { Address = "Ben Gurion Blvd 1, Bat Yam", Latitude = 32.0226, Longitude = 34.7458 },
        new { Address = "Dizengoff St 50, Tel Aviv", Latitude = 32.0771, Longitude = 34.7744 },
        new { Address = "Givat Shaul, Jerusalem", Latitude = 31.7934, Longitude = 35.1814 },
        new { Address = "Golda Meir Blvd, Haifa", Latitude = 32.7993, Longitude = 35.0153 },
        new { Address = "Beit HaRishonim, Rishon Lezion", Latitude = 31.9638, Longitude = 34.8038 },
        new { Address = "HaHistadrut 2, Hadera", Latitude = 32.4370, Longitude = 34.9196 },
        new { Address = "HaPalmach St 5, Safed", Latitude = 32.9646, Longitude = 35.4962 },
        new { Address = "Keren HaYesod 12, Eilat", Latitude = 29.5581, Longitude = 34.9482 }
    };

        string[] names = {
        "David Rosen", "Yael Cohen", "Moshe Levi", "Shiran Bar", "Noa Amar", "Lior Avraham",
        "Nir Shalev", "Maya Ben Ami", "Elior Hadad", "Tamar Azulay",
        "Hila Noy", "Avi Malka", "Roni Kaplan", "Omer Turgeman", "Dana Regev",
        "Yossi Gabay", "Michal Shani", "Alon Sharabi", "Neta Bachar", "Gili Mizrahi"
    };

        for (int i = 0; i < names.Length; i++)
        {
            int id;
            do id = s_rand.Next(MIN_ID, MAX_ID);
            while (s_dal!.Volunteer.Read(id) != null);

            string name = names[i];
            string phone = $"05{s_rand.Next(10000000, 99999999)}";
            string email = $"user{i + 1}@example.com";

            var location = locations[i % locations.Length];
            string address = location.Address;
            double? latitude = location.Latitude;
            double? longitude = location.Longitude;

            VolunteerRole role = i % 5 == 0 ? VolunteerRole.Manager : VolunteerRole.Regular;
            bool isActive = true;
            DistanceKind distanceKind = i % 2 == 0 ? DistanceKind.Aerial : DistanceKind.Ground;
            double? maxDistance = distanceKind == DistanceKind.Aerial ? s_rand.Next(1, 50) : null;

            s_dal!.Volunteer.Create(new(id, name, phone, email, role, isActive, distanceKind, address, latitude, longitude, null, maxDistance));
        }
    }
    private static void createCalls()
    {
        var locations = new[]
        {
        new { Address = "48 Allenby St, Tel Aviv", Latitude = 32.0635, Longitude = 34.7717 },
        new { Address = "10 King David St, Jerusalem", Latitude = 31.7767, Longitude = 35.2296 },
        new { Address = "31 Rager Blvd, Beer Sheva", Latitude = 31.2518, Longitude = 34.7913 },
        new { Address = "HaPalmach 2, Tiberias", Latitude = 32.7922, Longitude = 35.5315 },
        new { Address = "1 Herzl St, Ashdod", Latitude = 31.8032, Longitude = 34.6553 },
        new { Address = "Herzl Blvd 1, Holon", Latitude = 32.0144, Longitude = 34.7744 },
        new { Address = "8 HaRav Kook St, Netanya", Latitude = 32.3282, Longitude = 34.8560 },
        new { Address = "Dizengoff St 50, Tel Aviv", Latitude = 32.0771, Longitude = 34.7744 },
        new { Address = "Yefet St 190, Jaffa", Latitude = 32.0495, Longitude = 34.7516 },
        new { Address = "Beit HaRishonim, Rishon Lezion", Latitude = 31.9638, Longitude = 34.8038 },
        new { Address = "Weizmann 2, Rehovot", Latitude = 31.8936, Longitude = 34.8113 },
        new { Address = "Keren HaYesod 12, Eilat", Latitude = 29.5581, Longitude = 34.9482 },
        new { Address = "Golda Meir Blvd, Haifa", Latitude = 32.7993, Longitude = 35.0153 },
        new { Address = "Palmach 7, Safed", Latitude = 32.9646, Longitude = 35.4962 },
        new { Address = "Hagana Blvd 8, Petah Tikva", Latitude = 32.0830, Longitude = 34.8878 }
    };

        var callTypeDescriptions = new Dictionary<DO.CallType, string[]>
    {
        { DO.CallType.Technical, new[] {
            "בעיה טכנית במכשיר רפואי",
            "קריאה להחלפת סוללה במערכת התראה",
            "חיבור מחדש לרשת חשמלית",
            "בדיקת ציוד תקשורת בבית המטופל"
        }},
        { DO.CallType.Medical, new[] {
            "סיוע בקבלת תרופות",
            "הובלת חולה לקופת חולים",
            "הגעה לאדם שמתקשה לנשום",
            "בדיקת מצב פיזי של קשיש"
        }},
        { DO.CallType.Food, new[] {
            "חלוקת מזון לקשישים",
            "הבאת חבילת מצרכים למשפחה נזקקת",
            "העברת תרומה של ארוחה חמה",
            "סיוע בשוק קהילתי לנזקקים"
        }},
        { DO.CallType.Emergency, new[] {
            "אירוע חירום רפואי בבית",
            "דיווח על שריפה בבניין מגורים",
            "תאונת דרכים סמוך למרכז קהילתי",
            "ילד ננעל ברכב"
        }},
        { DO.CallType.Other, new[] {
            "בקשה לעזרה בהעברת חפצים",
            "סיוע במילוי טפסים ממשלתיים",
            "בקשה לליווי לקופת חולים",
            "סיוע לשכנה מבוגרת בהפעלת מזגן"
        }},
        { DO.CallType.None, new[] {
            "אין תאור"
                    }}
    };

        DateTime now = s_dal!.Config.Clock;

        for (int i = 0; i < 40; i++)
        {
            var location = locations[i % locations.Length];
            DO.CallType callType = (DO.CallType)s_rand.Next(Enum.GetValues(typeof(DO.CallType)).Length);
            string description = callTypeDescriptions[callType][s_rand.Next(callTypeDescriptions[callType].Length)];

            // מצב: 0 – סגור, 1 – פתוח רגיל, 2 – פתוח בסיכון, 3 – עתידי, 4 – לא טופל בזמן
            int mode = i % 5;

            DateTime openTime;
            DateTime? maxCallTime;

            switch (mode)
            {
                case 0: // ✅ הסתיימה מזמן
                    openTime = now.AddDays(-s_rand.Next(10, 30));
                    maxCallTime = openTime.AddHours(1 + s_rand.Next(1, 4));
                    break;
                case 1: // ⏳ פתוחה רגילה
                    openTime = now.AddHours(-s_rand.Next(1, 5));
                    maxCallTime = now.AddHours(s_rand.Next(2, 6));
                    break;
                case 2: // ⚠️ פתוחה בסיכון
                    openTime = now.AddHours(-s_rand.Next(3, 6));
                    maxCallTime = now.AddMinutes(s_rand.Next(5, 40)); // עוד מעט נגמר
                    break;
                case 3: // 🕒 עתידית
                    openTime = now.AddHours(s_rand.Next(1, 6));
                    maxCallTime = openTime.AddHours(2 + s_rand.Next(1, 4));
                    break;
                default: // ❌ לא טופלה ופג
                    openTime = now.AddDays(-s_rand.Next(2, 4));
                    maxCallTime = openTime.AddHours(1 + s_rand.Next(1, 3));
                    break;
            }

            s_dal.Call.Create(new(0, callType, location.Address, location.Latitude, location.Longitude, openTime, description, maxCallTime));
        }
    }

    private static void createAssignments()
    {
        var volunteerIds = s_dal!.Volunteer.ReadAll().Select(v => v.Id).ToList();
        //var callIds = s_dal!.Call.ReadAll().Select(c => c.Id).ToList();
        var callIds = s_dal!.Call.ReadAll().Select(c => c.Id).ToList();
        callIds = callIds.OrderBy(x => s_rand.Next()).ToList(); // ערבוב
        callIds = callIds.Take((int)(callIds.Count * 0.7)).ToList(); // רק 70% מהקריאות יקבלו טיפול

        var usedCallIds = new HashSet<int>();
        int assignmentId = 1;

        foreach (var volunteerId in volunteerIds)
        {
            int numOldAssignments = s_rand.Next(1, 4); // 1–3 טיפולים קודמים

            for (int i = 0; i < numOldAssignments; i++)
            {
                // קריאה חדשה שלא בשימוש
                int callId = getAvailableCallId(callIds, usedCallIds);
                if (callId == -1) break;

                DateTime start = s_dal.Config.Clock.AddDays(-s_rand.Next(30, 180));
                DateTime? end = s_rand.Next(2) == 0 ? start.AddMinutes(s_rand.Next(15, 90)) : null;
                TreatmentType? type = end != null
                    ? (TreatmentType?)s_rand.Next(Enum.GetValues(typeof(TreatmentType)).Length)
                    : null;

                s_dal.Assignment.Create(new(assignmentId++, volunteerId, callId, start, end, type));
            }

            // טיפול פעיל אחד עכשיו
            int activeCallId = getAvailableCallId(callIds, usedCallIds);
            if (activeCallId == -1) continue;

            DateTime activeStart = s_dal.Config.Clock.AddHours(-s_rand.Next(1, 3));
            DateTime? activeEnd = null;
            TreatmentType? activeType = null;

            s_dal.Assignment.Create(new(assignmentId++, volunteerId, activeCallId, activeStart, activeEnd, activeType));
        }
    }

    // מחזיר קריאה שלא שויכה עדיין
    private static int getAvailableCallId(List<int> callIds, HashSet<int> used)
    {
        foreach (var id in callIds)
        {
            if (!used.Contains(id))
            {
                used.Add(id);
                return id;
            }
        }
        return -1;
    }

}
