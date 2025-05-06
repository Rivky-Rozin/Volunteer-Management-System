
using BlApi;
using BO;

namespace BlTest
{
    internal class Program
    {
        static readonly IBl s_bl = Factory.Get();

        static void Main()
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\n📋 תפריט ראשי:");
                Console.WriteLine("1. ניהול מערכת");
                Console.WriteLine("2. טיפול בקריאות");
                Console.WriteLine("3. טיפול במתנדבים");
                Console.WriteLine("0. יציאה");
                Console.Write("בחר אפשרות: ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        AdminMenu();
                        break;
                    case "2":
                        CallMenu();
                        break;
                    case "3":
                        VolunteerMenu();
                        break;
                    case "0":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("❌ בחירה לא תקינה.");
                        break;
                }
            }
        }

        static void AdminMenu()
        {
            bool back = false;
            while (!back)
            {
                Console.WriteLine("\n🛠 תפריט ניהול:");
                Console.WriteLine("1. קבלת זמן נוכחי");
                Console.WriteLine("2. הזזת זמן");
                Console.WriteLine("3. קבלת זמן סיכון");
                Console.WriteLine("4. קביעת זמן סיכון");
                Console.WriteLine("5. איפוס מסד נתונים");
                Console.WriteLine("6. אתחול מסד נתונים");
                Console.WriteLine("0. חזרה לתפריט הראשי");
                Console.Write("בחר אפשרות: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        Console.WriteLine(s_bl.Admin.GetCurrentTime());
                        break;
                    case "2":
                        Console.WriteLine("בחר יחידת זמן: 0-שעה, 1-יום, 2-שבוע");
                        if (Enum.TryParse<TimeUnit>(Console.ReadLine(), out var unit))
                            s_bl.Admin.AdvanceTime(unit);
                        else Console.WriteLine("❌ יחידת זמן לא תקינה.");
                        break;
                    case "3":
                        Console.WriteLine(s_bl.Admin.GetRiskTimeSpan());
                        break;
                    case "4":
                        Console.Write("הכנס מספר ימים לזמן סיכון: ");
                        if (int.TryParse(Console.ReadLine(), out int days))
                            s_bl.Admin.SetRiskTimeSpan(TimeSpan.FromDays(days));
                        else Console.WriteLine("❌ מספר לא תקין.");
                        break;
                    case "5":
                        s_bl.Admin.ResetDatabase();
                        break;
                    case "6":
                        s_bl.Admin.InitializeDatabase();
                        break;
                    case "0":
                        back = true;
                        break;
                    default:
                        Console.WriteLine("❌ בחירה לא תקינה.");
                        break;
                }
            }
        }

        static void CallMenu()
        {
            bool back = false;
            while (!back)
            {
                Console.WriteLine("\n📞 תפריט קריאות:");
                Console.WriteLine("1. הוספת קריאה");
                Console.WriteLine("2. עדכון קריאה");
                Console.WriteLine("3. מחיקת קריאה");
                Console.WriteLine("4. צפייה בפרטי קריאה");
                Console.WriteLine("5. צפייה בכל הקריאות");
                Console.WriteLine("6. סיום טיפול");
                Console.WriteLine("7. ביטול טיפול");
                Console.WriteLine("8. בחירת קריאה לטיפול");
                Console.WriteLine("0. חזרה לתפריט הראשי");
                Console.Write("בחר אפשרות: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        var newCall = new Call
                        {
                            Id = 0,
                            RequesterId = 123456789,
                            Description = "בדיקת קריאה",
                            CallType = CallType.Medical
                        };
                        try
                        {
                            s_bl.Call.AddCall(newCall);
                            Console.WriteLine("✅ קריאה נוספה.");
                        }
                        catch (Exception ex) { Console.WriteLine($"❌ {ex.Message}"); }
                        break;

                    case "2":
                        Console.Write("הכנס מזהה קריאה: ");
                        if (int.TryParse(Console.ReadLine(), out int callId))
                        {
                            try
                            {
                                BO.Call call1 = s_bl.Call.GetCallDetails(callId);
                                call1.Description = "עודכן";
                                s_bl.Call.UpdateCall(call1);
                                Console.WriteLine("✅ עודכן בהצלחה.");
                            }
                            catch (Exception ex) { Console.WriteLine($"❌ {ex.Message}"); }
                        }
                        else Console.WriteLine("❌ מזהה לא תקין.");
                        break;

                    case "3":
                        Console.Write("הכנס מזהה קריאה למחיקה: ");
                        if (int.TryParse(Console.ReadLine(), out int deleteId))
                        {
                            try { s_bl.Call.DeleteCall(deleteId); }
                            catch (Exception ex) { Console.WriteLine($"❌ {ex.Message}"); }
                        }
                        break;

                    case "4":
                        Console.Write("הכנס מזהה קריאה לצפייה: ");
                        if (int.TryParse(Console.ReadLine(), out int readId))
                        {
                            try { Console.WriteLine(s_bl.Call.GetCallDetails(readId)); }
                            catch (Exception ex) { Console.WriteLine($"❌ {ex.Message}"); }
                        }
                        break;

                    case "5":
                        foreach (var c in s_bl.Call.GetCallList(null, null, null))
                            Console.WriteLine(c);
                        break;

                    case "6":
                        Console.Write("הכנס מזהה מתנדב: ");
                        int.TryParse(Console.ReadLine(), out int volId);
                        Console.Write("הכנס מזהה שיבוץ: ");
                        int.TryParse(Console.ReadLine(), out int assId);
                        try { s_bl.Call.CompleteCallTreatment(volId, assId); }
                        catch (Exception ex) { Console.WriteLine($"❌ {ex.Message}"); }
                        break;

                    case "7":
                        Console.Write("הכנס מזהה מבקש: ");
                        int.TryParse(Console.ReadLine(), out int reqId);
                        Console.Write("הכנס מזהה שיבוץ: ");
                        int.TryParse(Console.ReadLine(), out int aId);
                        try { s_bl.Call.CancelCallTreatment(reqId, aId); }
                        catch (Exception ex) { Console.WriteLine($"❌ {ex.Message}"); }
                        break;

                    case "8":
                        Console.Write("מזהה מתנדב: ");
                        int.TryParse(Console.ReadLine(), out int vol);
                        Console.Write("מזהה קריאה: ");
                        int.TryParse(Console.ReadLine(), out int call);
                        try { s_bl.Call.SelectCallForTreatment(vol, call); }
                        catch (Exception ex) { Console.WriteLine($"❌ {ex.Message}"); }
                        break;

                    case "0":
                        back = true;
                        break;

                    default:
                        Console.WriteLine("❌ בחירה לא תקינה.");
                        break;
                }
            }
        }

        static void VolunteerMenu()
        {
            bool back = false;
            while (!back)
            {
                Console.WriteLine("\n🤝 תפריט מתנדבים:");
                Console.WriteLine("1. הוספת מתנדב");
                Console.WriteLine("2. עדכון מתנדב");
                Console.WriteLine("3. מחיקת מתנדב");
                Console.WriteLine("4. צפייה בפרטי מתנדב");
                Console.WriteLine("5. צפייה ברשימת מתנדבים");
                Console.WriteLine("0. חזרה לתפריט הראשי");
                Console.Write("בחר אפשרות: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        var v = new Volunteer
                        {
                            Id = "123456789",
                            Name = "יוסי",
                            Phone = "0501234567",
                            Email = "yossi@example.com"
                        };
                        try
                        {
                            s_bl.Volunteer.AddVolunteer(v);
                            Console.WriteLine("✅ מתנדב נוסף.");
                        }
                        catch (Exception ex) { Console.WriteLine($"❌ {ex.Message}"); }
                        break;

                    case "2":
                        Console.Write("הכנס ת.ז. מתנדב: ");
                        var id = Console.ReadLine();
                        try
                        {
                            var existing = s_bl.Volunteer.GetVolunteerDetails(id);
                            existing.Name = "עודכן";
                            s_bl.Volunteer.UpdateVolunteer(id, existing);
                            Console.WriteLine("✅ עודכן בהצלחה.");
                        }
                        catch (Exception ex) { Console.WriteLine($"❌ {ex.Message}"); }
                        break;

                    case "3":
                        Console.Write("הכנס ת.ז. למחיקה: ");
                        var did = Console.ReadLine();
                        try { s_bl.Volunteer.DeleteVolunteer(did); }
                        catch (Exception ex) { Console.WriteLine($"❌ {ex.Message}"); }
                        break;

                    case "4":
                        Console.Write("הכנס ת.ז.: ");
                        var readId = Console.ReadLine();
                        try { Console.WriteLine(s_bl.Volunteer.GetVolunteerDetails(readId)); }
                        catch (Exception ex) { Console.WriteLine($"❌ {ex.Message}"); }
                        break;

                    case "5":
                        foreach (var item in s_bl.Volunteer.GetVolunteersList())
                            Console.WriteLine(item);
                        break;

                    case "0":
                        back = true;
                        break;

                    default:
                        Console.WriteLine("❌ בחירה לא תקינה.");
                        break;
                }
            }
        }
    }
}
