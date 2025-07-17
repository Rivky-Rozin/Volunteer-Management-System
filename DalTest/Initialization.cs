namespace DalTest;

using Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;

public static class Initialization
{
    private static IDal? s_dal;

    // הערה: הסרנו את האובייקט Random כדי להבטיח שכל הנתונים יהיו קבועים בכל הרצה
    // private static readonly Random s_rand = new();

    public static void Do()
    {
        s_dal = Factory.Get;
        Console.WriteLine("Resetting Database...");
        s_dal.ResetDB();
        Console.WriteLine("Initializing All lists with a large, consistent dataset...");

        createVolunteers(); // יצירת 50 מתנדבים
        createCalls();      // יצירת 150 קריאות
        createAssignments();// יצירת שיבוצים מורכבים

        Console.WriteLine("Initialization Complete.");
    }

    private static void createVolunteers()
    {
        Console.WriteLine("Creating 50 Volunteers with passwords...");

        // ... (כל המערכים של השמות, הכתובות וכו' נשארים זהים)
        var locations = new[]
        {
        // (רשימת המיקומים המלאה...)
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
        new { Address = "Keren HaYesod 12, Eilat", Latitude = 29.5581, Longitude = 34.9482 },
        new { Address = "17 Arlozorov St, Ramat Gan", Latitude = 32.0853, Longitude = 34.8058 },
        new { Address = "25 Bialik St, Petah Tikva", Latitude = 32.0911, Longitude = 34.8856 },
        new { Address = "3 Hahagana St, Ashkelon", Latitude = 31.6661, Longitude = 34.5772 },
        new { Address = "7 Tchernikhovski St, Kfar Saba", Latitude = 32.1798, Longitude = 34.9042 },
        new { Address = "11 Ha'atzmaut Square, Afula", Latitude = 32.6109, Longitude = 35.2903 },
        new { Address = "5 Rothschild St, Kiryat Ono", Latitude = 32.0594, Longitude = 34.8524 },
        new { Address = "14 Herzl St, Nahariya", Latitude = 33.0069, Longitude = 35.0931 },
        new { Address = "9 Kook St, Lod", Latitude = 31.9515, Longitude = 34.8963 },
        new { Address = "22 HaNevi'im St, Ramla", Latitude = 31.9273, Longitude = 34.8697 },
        new { Address = "6 Sokolov St, Hod HaSharon", Latitude = 32.1528, Longitude = 34.8878 },
        new { Address = "18 Ben Yehuda St, Jerusalem", Latitude = 31.7820, Longitude = 35.2185 },
        new { Address = "70 Ibn Gabirol St, Tel Aviv", Latitude = 32.0800, Longitude = 34.7821 },
        new { Address = "12 Menachem Begin Rd, Ramat HaSharon", Latitude = 32.1444, Longitude = 34.8427 },
        new { Address = "33 Derech HaShalom, Givatayim", Latitude = 32.0683, Longitude = 34.8062 },
        new { Address = "8 HaBanim St, Karmiel", Latitude = 32.9123, Longitude = 35.2936 },
        new { Address = "15 Moriah Blvd, Haifa", Latitude = 32.7997, Longitude = 34.9897 },
        new { Address = "2 Balfour St, Bat Yam", Latitude = 32.0199, Longitude = 34.7505 },
        new { Address = "1 Derech Yavne, Rehovot", Latitude = 31.8974, Longitude = 34.8091 },
        new { Address = "50 Eli Cohen St, Ashkelon", Latitude = 31.6745, Longitude = 34.5701 },
        new { Address = "29 HaMeyasdim St, Zichron Yaakov", Latitude = 32.5721, Longitude = 34.9547 },
        new { Address = "4 HaAliya St, Tiberias", Latitude = 32.7917, Longitude = 35.5310 },
        new { Address = "76 HaHistadrut, Holon", Latitude = 32.0255, Longitude = 34.7865 },
        new { Address = "23 Trumpeldor St, Beer Sheva", Latitude = 31.2464, Longitude = 34.7934 },
        new { Address = "11 Sderot Weizmann, Ness Ziona", Latitude = 31.9304, Longitude = 34.7963 },
        new { Address = "6 Eliyahu Golomb St, Kiryat Ata", Latitude = 32.8118, Longitude = 35.1111 },
        new { Address = "19 HaShikma St, Sderot", Latitude = 31.5262, Longitude = 34.5976 },
        new { Address = "10 David Remez St, Dimona", Latitude = 31.0664, Longitude = 35.0336 },
        new { Address = "3 HaGefen St, Eilat", Latitude = 29.5601, Longitude = 34.9531 },
        new { Address = "27 Katznelson St, Givatayim", Latitude = 32.0722, Longitude = 34.8105 },
        new { Address = "14 Bialik St, Tel Aviv", Latitude = 32.0708, Longitude = 34.7712 }
    };
        string[] names = {
        "David Rosen", "Yael Cohen", "Moshe Levi", "Shiran Bar", "Noa Amar", "Lior Avraham", "Nir Shalev", "Maya Ben Ami", "Elior Hadad", "Tamar Azulay",
        "Hila Noy", "Avi Malka", "Roni Kaplan", "Omer Turgeman", "Dana Regev", "Yossi Gabay", "Michal Shani", "Alon Sharabi", "Neta Bachar", "Gili Mizrahi",
        "Itay Segal", "Adi Katz", "Galit Ohana", "Roee Peretz", "Shir Dahan", "Ben Edri", "Karin Biton", "Tal Friedman", "Liron Maimon", "Matan David",
        "Shani Goldstein", "Idan Vaknin", "Orly Suissa", "Eyal Atias", "Hadar Maman", "Amir Azulay", "Moran Lankri", "Guy Dayan", "Sapir Elimelech", "Daniel Swisa",
        "Inbar Asayag", "Yarin Elbaz", "Ofir Ohayon", "Stav Lugasi", "Dorin Amsalem", "Snir Malka", "Eden Guetta", "Elad Sabag", "Chen Revivo", "Liav Zrihen"
    };
        string[] phones = {
        "0501234567", "0522345678", "0533456789", "0544567890", "0555678901", "0506789012", "0527890123", "0538901234", "0549012345", "0550123456",
        "0501122334", "0522233445", "0533344556", "0544455667", "0555566778", "0506677889", "0527788990", "0538899001", "0549900112", "0550011223",
        "0501111111", "0522222222", "0533333333", "0544444444", "0555555555", "0506666666", "0527777777", "0538888888", "0549999999", "0550000000",
        "0501212121", "0523434343", "0535656565", "0547878787", "0559090909", "0501010101", "0522020202", "0534040404", "0546060606", "0558080808",
        "0501357911", "0522468012", "0531123581", "0541234567", "0557654321", "0509876543", "0521239876", "0534561237", "0547894561", "0551597532"
    };
        double?[] maxDistances = {
        25, null, 15, null, 40, null, 10, null, 30, null, 5, null, 45, null, 20, null, 35, null, 8, null,
        22, null, 18, null, 33, null, 12, null, 28, null, 7, null, 42, null, 23, null, 38, null, 9, null,
        26, null, 16, null, 41, null, 11, null, 31, null
    };

        for (int i = 0; i < names.Length; i++)
        {
            int id = 200000000 + i;
            string name = names[i];
            string phone = phones[i];
            string email = $"user{i + 1}@example.com";

            // --- הוספת סיסמה קבועה וייחודית לכל מתנדב ---
            string password = $"pw{id}"; // לדוגמה: "pw200000000", "pw200000001" וכו'
                                         // --- סוף השינוי ---

            var location = locations[i % locations.Length];
            VolunteerRole role = i % 10 == 0 ? VolunteerRole.Manager : VolunteerRole.Regular;
            bool isActive = (i % 15 != 0);
            DistanceKind distanceKind = i % 2 == 0 ? DistanceKind.Aerial : DistanceKind.Ground;
            double? maxDistance = maxDistances[i];

            // --- עדכון הקריאה לבנאי עם הסיסמה החדשה ---
            // החלפנו את ה-null שהיה בעבר במשתנה password
            s_dal!.Volunteer.Create(new(id, name, phone, email, role, isActive, distanceKind, location.Address, location.Latitude, location.Longitude, password, maxDistance));
        }
    }
    private static void createCalls()
    {
        Console.WriteLine("Creating 150 Calls...");

        var callLocations = s_dal!.Volunteer.ReadAll().Select(v => new { v.Address, v.Latitude, v.Longitude }).ToArray();

        var callTypeDescriptions = new Dictionary<CallType, string[]>
        {
            { CallType.Technical, new[] { "בעיה טכנית במכשיר רפואי", "החלפת סוללה במערכת התראה", "תיקון תקשורת בלחצן מצוקה", "התקנת ציוד חדש" }},
            { CallType.Medical, new[] { "סיוע בקבלת תרופות", "הובלת חולה לקופת חולים", "בדיקת מצב רפואי כללי", "ליווי לבדיקה דחופה" }},
            { CallType.Food, new[] { "חלוקת מזון לקשישים", "הבאת חבילת מצרכים", "העברת ארוחה חמה", "סיוע בקניות בסופר" }},
            { CallType.Emergency, new[] { "נפילה בבית", "דיווח על קוצר נשימה", "תאונה קלה בקרבת הבית", "תחושת לחץ בחזה" }},
            { CallType.Other, new[] { "עזרה בהעברת חפצים", "סיוע במילוי טפסים", "ליווי לסידורים", "אירוח חברה לקשיש בודד" }}
        };

        DateTime now = s_dal!.Config.Clock;
        var callTypes = Enum.GetValues(typeof(CallType)).Cast<CallType>().Where(ct => ct != CallType.None).ToArray();

        for (int i = 0; i < 150; i++)
        {
            var location = callLocations[i % callLocations.Length];
            CallType callType = callTypes[i % callTypes.Length];
            string description = callTypeDescriptions[callType][i % callTypeDescriptions[callType].Length];

            int mode = i % 6; // 6 מצבים שונים כדי ליצור מגוון רחב יותר

            DateTime openTime;
            DateTime? maxCallTime;

            // יצירת זמנים קבועים על בסיס האינדקס i
            switch (mode)
            {
                case 0: // הסתיימה מזמן (לפני 10-40 יום)
                    openTime = now.AddDays(-(10 + i % 30));
                    maxCallTime = openTime.AddHours(1 + (i % 4));
                    break;
                case 1: // פתוחה רגילה (נפתחה ב-5 השעות האחרונות)
                    openTime = now.AddHours(-(i % 5 + 1));
                    maxCallTime = now.AddHours(2 + (i % 6));
                    break;
                case 2: // פתוחה בסיכון (עומדת לפוג)
                    openTime = now.AddHours(-(i % 4 + 2));
                    maxCallTime = now.AddMinutes(15 + (i % 30));
                    break;
                case 3: // עתידית (תיפתח ב-6 השעות הקרובות)
                    openTime = now.AddHours(1 + (i % 6));
                    maxCallTime = openTime.AddHours(2 + (i % 4));
                    break;
                case 4: // לא טופלה ופג תוקפה
                    openTime = now.AddDays(-(2 + i % 5));
                    maxCallTime = openTime.AddHours(1 + (i % 3));
                    break;
                default: // הסתיימה לאחרונה (ביממה האחרונה)
                    openTime = now.AddHours(-(5 + i % 18));
                    maxCallTime = openTime.AddHours(2);
                    break;
            }

            s_dal.Call.Create(new(0, callType, location.Address, location.Latitude, location.Longitude, openTime, description, maxCallTime));
        }
    }

    private static void createAssignments()
    {
        Console.WriteLine("Creating deterministic and logical Assignments...");

        var volunteerIds = s_dal!.Volunteer.ReadAll().Select(v => v.Id).ToList();
        var allCalls = s_dal!.Call.ReadAll().ToList();
        DateTime now = s_dal.Config.Clock;

        // --- שינוי מרכזי: הפרדת הקריאות לקבוצות הגיוניות ---
        var pastCalls = allCalls.Where(c => c.MaxCallTime.HasValue && c.MaxCallTime < now).ToList();
        var openCalls = allCalls.Where(c => c.OpenTime < now && (!c.MaxCallTime.HasValue || c.MaxCallTime >= now)).ToList();
        // הקריאות העתידיות יישארו ללא שיבוץ, וזה הגיוני.

        int assignmentId = 1;
        int pastCallIndex = 0;
        int openCallIndex = 0;

        for (int v_idx = 0; v_idx < volunteerIds.Count; v_idx++)
        {
            var volunteerId = volunteerIds[v_idx];

            // 1. יצירת שיבוצים היסטוריים (רק מקריאות שכבר נסגרו)
            int numOldAssignments = (v_idx % 3) + 1; // 1-3 שיבוצים ישנים
            for (int i = 0; i < numOldAssignments; i++)
            {
                if (pastCallIndex >= pastCalls.Count) break; // אם נגמרו הקריאות מהעבר

                var call = pastCalls[pastCallIndex++];

                // תאריך התחלה וסיום נקבעים באופן קבוע ביחס לזמני הקריאה
                DateTime start = call.OpenTime.AddMinutes(5 + (v_idx % 10));
                // נוודא שתאריך הסיום לא יהיה אחרי תאריך התפוגה של הקריאה
                DateTime end = new[] { start.AddMinutes(20 + (i + v_idx) % 40), call.MaxCallTime!.Value }.Min();
                TreatmentType type = (TreatmentType)((v_idx + i) % Enum.GetValues(typeof(TreatmentType)).Length);

                s_dal.Assignment.Create(new(assignmentId++, volunteerId, call.Id, start, end, type));
            }

            // 2. יצירת שיבוץ פעיל (רק מקריאות שפתוחות כרגע)
            bool isVolunteerActive = s_dal.Volunteer.Read(volunteerId)!.IsActive;
            if (isVolunteerActive && (v_idx % 2 == 0)) // לחצי מהמתנדבים הפעילים
            {
                if (openCallIndex >= openCalls.Count) continue; // אם נגמרו הקריאות הפתוחות

                var call = openCalls[openCallIndex++];
                DateTime activeStart = now.AddMinutes(-(15 + v_idx % 45)); // התחיל לא מזמן

                s_dal.Assignment.Create(new(assignmentId++, volunteerId, call.Id, activeStart, null, null));
            }
        }
    }
}