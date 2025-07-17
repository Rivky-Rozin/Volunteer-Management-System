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

        // --- שינוי: הוספת מערכים עם נתונים קבועים במקום אקראיים ---
        string[] phones = {
        "0501234567", "0522345678", "0533456789", "0544567890", "0555678901",
        "0506789012", "0527890123", "0538901234", "0549012345", "0550123456",
        "0501122334", "0522233445", "0533344556", "0544455667", "0555566778",
        "0506677889", "0527788990", "0538899001", "0549900112", "0550011223"
    };

        double?[] maxDistances = {
        25, null, 15, null, 40, null, 10, null, 30, null,
        5, null, 45, null, 20, null, 35, null, 8, null
    };
        // --- סוף השינוי ---

        for (int i = 0; i < names.Length; i++)
        {
            // --- שינוי: שימוש בת"ז קבועה במקום אקראית ---
            // אנו מתחילים מבסיס גבוה כדי להימנע מהתנגשויות עם ת"ז אמיתיות
            int id = 200000000 + i;

            // אין יותר צורך בלולאה שבודקת אם הת"ז קיימת, כי אנו מבטיחים שהיא ייחודית
            // do id = s_rand.Next(MIN_ID, MAX_ID);
            // while (s_dal!.Volunteer.Read(id) != null);

            string name = names[i];

            // --- שינוי: שימוש במספר טלפון קבוע מהמערך ---
            string phone = phones[i];
            string email = $"user{i + 1}@example.com";

            var location = locations[i % locations.Length];
            string address = location.Address;
            double? latitude = location.Latitude;
            double? longitude = location.Longitude;

            VolunteerRole role = i % 5 == 0 ? VolunteerRole.Manager : VolunteerRole.Regular;
            bool isActive = true;
            DistanceKind distanceKind = i % 2 == 0 ? DistanceKind.Aerial : DistanceKind.Ground;

            // --- שינוי: שימוש במרחק קבוע מהמערך ---
            double? maxDistance = maxDistances[i];

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
        }}
    };

        DateTime now = s_dal!.Config.Clock;

        for (int i = 0; i < 40; i++)
        {
            var location = locations[i % locations.Length];
            DO.CallType callType = (DO.CallType)s_rand.Next(Enum.GetValues(typeof(DO.CallType)).Length-1);
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
