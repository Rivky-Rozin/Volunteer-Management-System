//using System;
//using BlApi;
//using BO;

//namespace BlTest
//{
//    internal class Program
//    {
//        static readonly IBl s_bl = Factory.Get();

//        static void Main()
//        {
//            bool exit = false;
//            while (!exit)
//            {
//                Console.WriteLine("\n📋 תפריט ראשי:");
//                Console.WriteLine("1. ניהול מערכת");
//                Console.WriteLine("2. טיפול בקריאות");
//                Console.WriteLine("3. טיפול במתנדבים");
//                Console.WriteLine("0. יציאה");
//                Console.Write("בחר אפשרות: ");

//                switch (Console.ReadLine())
//                {
//                    case "1": AdminMenu(); break;
//                    case "2": CallMenu(); break;
//                    case "3": VolunteerMenu(); break;
//                    case "0": exit = true; break;
//                    default: Console.WriteLine("❗ בחירה לא חוקית"); break;
//                }
//            }
//        }

//        static void AdminMenu()
//        {
//            bool back = false;
//            while (!back)
//            {
//                Console.WriteLine("\n🔧 ניהול מערכת:");
//                Console.WriteLine("1. אתחול מסד נתונים");
//                Console.WriteLine("2. איפוס מסד נתונים");
//                Console.WriteLine("3. הצגת שעה נוכחית");
//                Console.WriteLine("4. קידום זמן");
//                Console.WriteLine("5. הגדרת זמן סיכון");
//                Console.WriteLine("6. הצגת זמן סיכון");
//                Console.WriteLine("0. חזרה");
//                Console.Write("בחר אפשרות: ");

//                switch (Console.ReadLine())
//                {
//                    case "1": s_bl.Admin.InitializeDatabase(); Console.WriteLine("📥 אתחול בוצע."); break;
//                    case "2": s_bl.Admin.ResetDatabase(); Console.WriteLine("📤 איפוס בוצע."); break;
//                    case "3": Console.WriteLine($"🕒 שעה נוכחית: {s_bl.Admin.GetCurrentTime()}"); break;
//                    case "4":
//                        Console.Write("בחר יחידת זמן (0-דקה, 1-שעה, 2-יום): ");
//                        if (int.TryParse(Console.ReadLine(), out int unit) && Enum.IsDefined(typeof(TimeUnit), unit))
//                        {
//                            s_bl.Admin.AdvanceTime((TimeUnit)unit);
//                            Console.WriteLine("⏩ זמן הוקדם.");
//                        }
//                        else Console.WriteLine("❗ קלט לא חוקי");
//                        break;
//                    case "5":
//                        Console.Write("הזן זמן סיכון בדקות: ");
//                        if (int.TryParse(Console.ReadLine(), out int minutes) && minutes >= 0)
//                        {
//                            s_bl.Admin.SetRiskTimeSpan(TimeSpan.FromMinutes(minutes));
//                            Console.WriteLine("⏱️ זמן סיכון הוגדר.");
//                        }
//                        else Console.WriteLine("❗ קלט לא חוקי");
//                        break;
//                    case "6":
//                        TimeSpan riskTime = s_bl.Admin.GetRiskTimeSpan();
//                        Console.WriteLine($"⏱️ זמן סיכון נוכחי: {riskTime.TotalMinutes} דקות");
//                        break;
//                    case "0": back = true; break;
//                    default: Console.WriteLine("❗ בחירה לא חוקית"); break;
//                }
//            }
//        }

//        static void CallMenu()
//        {
//            bool back = false;
//            while (!back)
//            {
//                Console.WriteLine("\n📞 תפריט קריאות:");
//                Console.WriteLine("1. הוספת קריאה");
//                Console.WriteLine("2. עדכון קריאה");
//                Console.WriteLine("3. מחיקת קריאה");
//                Console.WriteLine("4. צפייה בפרטי קריאה");
//                Console.WriteLine("5. צפייה בכל הקריאות");
//                Console.WriteLine("6. סיום טיפול");
//                Console.WriteLine("7. ביטול טיפול");
//                Console.WriteLine("8. בחירת קריאה לטיפול");
//                Console.WriteLine("9. הצגת קריאות סגורות של מתנדב");
//                Console.WriteLine("10. הצגת קריאות פתוחות למתנדב");
//                Console.WriteLine("0. חזרה לתפריט הראשי");
//                Console.Write("בחר אפשרות: ");

//                switch (Console.ReadLine())
//                {
//                    case "1":
//                        try
//                        {
//                            BO.Call call = new BO.Call();

//                            Console.Write("הזן סוג קריאה (0-חירום, 1-רפואי, 2-אספקה): ");
//                            if (int.TryParse(Console.ReadLine(), out int callType) && Enum.IsDefined(typeof(CallType), callType))
//                                call.CallType = (CallType)callType;
//                            else
//                            {
//                                Console.WriteLine("❗ סוג קריאה לא חוקי");
//                                break;
//                            }

//                            Console.Write("הזן תיאור: ");
//                            call.Description = Console.ReadLine();

//                            Console.Write("הזן כתובת: ");
//                            call.Address = Console.ReadLine() ?? string.Empty;

//                            Console.Write("הזן קו רוחב (Latitude): ");
//                            double.TryParse(Console.ReadLine(), out double lat);
//                            call.Latitude = lat;


//                            Console.Write("הזן קו אורך (Longitude): ");
//                            double.TryParse(Console.ReadLine(), out double lon);
//                            call.Longitude = lon;

//                            call.CreationTime = s_bl.Admin.GetCurrentTime();
//                            call.Status = CallStatus.Open;

//                            s_bl.Call.AddCall(call);
//                            Console.WriteLine("✅ קריאה נוספה בהצלחה");
//                        }
//                        catch (Exception ex)
//                        {
//                            Console.WriteLine($"❌ שגיאה: {ex.GetType().Name} - {ex.Message}");
//                            if (ex.InnerException != null)
//                                Console.WriteLine($"שגיאה פנימית: {ex.InnerException.Message}");
//                        }
//                        break;

//                    case "2":
//                        try
//                        {
//                            Console.Write("הזן מספר קריאה לעדכון: ");
//                            if (int.TryParse(Console.ReadLine(), out int updateId))
//                            {
//                                BO.Call callToUpdate = s_bl.Call.GetCallDetails(updateId);

//                                Console.Write($"סוג קריאה נוכחי: {callToUpdate.CallType}, הזן סוג חדש (0-חירום, 1-רפואי, 2-אספקה) או הקש Enter לדלג: ");
//                                string? input = Console.ReadLine();
//                                if (!string.IsNullOrEmpty(input) && int.TryParse(input, out int newCallType) && Enum.IsDefined(typeof(CallType), newCallType))
//                                    callToUpdate.CallType = (CallType)newCallType;

//                                Console.Write($"תיאור נוכחי: {callToUpdate.Description}, הזן תיאור חדש או הקש Enter לדלג: ");
//                                input = Console.ReadLine();
//                                if (!string.IsNullOrEmpty(input))
//                                    callToUpdate.Description = input;

//                                Console.Write($"כתובת נוכחית: {callToUpdate.Address}, הזן כתובת חדשה או הקש Enter לדלג: ");
//                                input = Console.ReadLine();
//                                if (!string.IsNullOrEmpty(input))
//                                    callToUpdate.Address = input;

//                                Console.Write($"קו רוחב נוכחי: {callToUpdate.Latitude}, הזן קו רוחב חדש או הקש Enter לדלג: ");
//                                input = Console.ReadLine();
//                                if (!string.IsNullOrEmpty(input) && double.TryParse(input, out double newLat))
//                                    callToUpdate.Latitude = newLat;

//                                Console.Write($"קו אורך נוכחי: {callToUpdate.Longitude}, הזן קו אורך חדש או הקש Enter לדלג: ");
//                                input = Console.ReadLine();
//                                if (!string.IsNullOrEmpty(input) && double.TryParse(input, out double newLon))
//                                    callToUpdate.Longitude = newLon;

//                                s_bl.Call.UpdateCall(callToUpdate);
//                                Console.WriteLine("✅ קריאה עודכנה בהצלחה");
//                            }
//                            else
//                                Console.WriteLine("❗ מספר קריאה לא חוקי");
//                        }
//                        catch (Exception ex)
//                        {
//                            Console.WriteLine($"❌ שגיאה: {ex.GetType().Name} - {ex.Message}");
//                            if (ex.InnerException != null)
//                                Console.WriteLine($"שגיאה פנימית: {ex.InnerException.Message}");
//                        }
//                        break;

//                    case "3":
//                        try
//                        {
//                            Console.Write("הזן מספר קריאה למחיקה: ");
//                            if (int.TryParse(Console.ReadLine(), out int deleteId))
//                            {
//                                s_bl.Call.DeleteCall(deleteId);
//                                Console.WriteLine("✅ קריאה נמחקה בהצלחה");
//                            }
//                            else
//                                Console.WriteLine("❗ מספר קריאה לא חוקי");
//                        }
//                        catch (Exception ex)
//                        {
//                            Console.WriteLine($"❌ שגיאה: {ex.GetType().Name} - {ex.Message}");
//                            if (ex.InnerException != null)
//                                Console.WriteLine($"שגיאה פנימית: {ex.InnerException.Message}");
//                        }
//                        break;

//                    case "4":
//                        try
//                        {
//                            Console.Write("הזן מספר קריאה להצגה: ");
//                            if (int.TryParse(Console.ReadLine(), out int viewId))
//                            {
//                                BO.Call call = s_bl.Call.GetCallDetails(viewId);
//                                Console.WriteLine(call);

//                                if (call.Assignments != null && call.Assignments.Count > 0)
//                                {
//                                    Console.WriteLine("\nשיוכים לקריאה זו:");
//                                    foreach (var assignment in call.Assignments)
//                                        Console.WriteLine(assignment);
//                                }
//                                else
//                                {
//                                    Console.WriteLine("אין שיוכים לקריאה זו");
//                                }
//                            }
//                            else
//                                Console.WriteLine("❗ מספר קריאה לא חוקי");
//                        }
//                        catch (Exception ex)
//                        {
//                            Console.WriteLine($"❌ שגיאה: {ex.GetType().Name} - {ex.Message}");
//                            if (ex.InnerException != null)
//                                Console.WriteLine($"שגיאה פנימית: {ex.InnerException.Message}");
//                        }
//                        break;

//                    case "5":
//                        try
//                        {
//                            Console.WriteLine("\nאפשרויות סינון:");
//                            Console.WriteLine("1. ללא סינון");
//                            Console.WriteLine("2. סינון לפי סוג קריאה");
//                            Console.WriteLine("3. סינון לפי סטטוס");
//                            Console.Write("בחר אפשרות: ");

//                            CallInListField? filterField = null;
//                            object? filterValue = null;

//                            string filterOption = Console.ReadLine() ?? "1";
//                            switch (filterOption)
//                            {
//                                case "2":
//                                    Console.Write("הזן סוג קריאה לסינון (0-חירום, 1-רפואי, 2-אספקה): ");
//                                    if (int.TryParse(Console.ReadLine(), out int callTypeFilter) && Enum.IsDefined(typeof(CallType), callTypeFilter))
//                                    {
//                                        filterField = CallInListField.CallType;
//                                        filterValue = (CallType)callTypeFilter;
//                                    }
//                                    else
//                                        Console.WriteLine("❗ סוג קריאה לא חוקי, מציג ללא סינון");
//                                    break;

//                                case "3":
//                                    Console.Write("הזן סטטוס קריאה לסינון (0-פתוח, 1-בטיפול, 2-סגור): ");
//                                    if (int.TryParse(Console.ReadLine(), out int statusFilter) && Enum.IsDefined(typeof(CallStatus), statusFilter))
//                                    {
//                                        filterField = CallInListField.Status;
//                                        filterValue = (CallStatus)statusFilter;
//                                    }
//                                    else
//                                        Console.WriteLine("❗ סטטוס לא חוקי, מציג ללא סינון");
//                                    break;
//                            }

//                            Console.WriteLine("\nאפשרויות מיון:");
//                            Console.WriteLine("1. ללא מיון מיוחד");
//                            Console.WriteLine("2. מיון לפי סוג קריאה");
//                            Console.WriteLine("3. מיון לפי זמן יצירה");
//                            Console.WriteLine("4. מיון לפי סטטוס");
//                            Console.Write("בחר אפשרות: ");

//                            CallInListField? sortField = null;

//                            string sortOption = Console.ReadLine() ?? "1";
//                            switch (sortOption)
//                            {
//                                case "2": sortField = CallInListField.CallType; break;
//                                case "3": sortField = CallInListField.OpenTime; break;
//                                case "4": sortField = CallInListField.Status; break;
//                            }

//                            Console.WriteLine("\nרשימת קריאות:");
//                            foreach (var callInList in s_bl.Call.GetCallList(filterField, filterValue, sortField))
//                                Console.WriteLine(callInList);
//                        }
//                        catch (Exception ex)
//                        {
//                            Console.WriteLine($"❌ שגיאה: {ex.GetType().Name} - {ex.Message}");
//                            if (ex.InnerException != null)
//                                Console.WriteLine($"שגיאה פנימית: {ex.InnerException.Message}");
//                        }
//                        break;

//                    case "6":
//                        try
//                        {
//                            Console.Write("הזן מספר מתנדב: ");
//                            if (int.TryParse(Console.ReadLine(), out int volunteerId))
//                            {
//                                Console.Write("הזן מספר שיוך: ");
//                                if (int.TryParse(Console.ReadLine(), out int assignmentId))
//                                {
//                                    s_bl.Call.CompleteCallTreatment(volunteerId, assignmentId);
//                                    Console.WriteLine("✅ טיפול בקריאה הושלם בהצלחה");
//                                }
//                                else
//                                    Console.WriteLine("❗ מספר שיוך לא חוקי");
//                            }
//                            else
//                                Console.WriteLine("❗ מספר מתנדב לא חוקי");
//                        }
//                        catch (Exception ex)
//                        {
//                            Console.WriteLine($"❌ שגיאה: {ex.GetType().Name} - {ex.Message}");
//                            if (ex.InnerException != null)
//                                Console.WriteLine($"שגיאה פנימית: {ex.InnerException.Message}");
//                        }
//                        break;

//                    case "7":
//                        try
//                        {
//                            Console.Write("הזן מספר מבקש: ");
//                            if (int.TryParse(Console.ReadLine(), out int requesterId))
//                            {
//                                Console.Write("הזן מספר שיוך: ");
//                                if (int.TryParse(Console.ReadLine(), out int assignmentId))
//                                {
//                                    s_bl.Call.CancelCallTreatment(requesterId, assignmentId);
//                                    Console.WriteLine("✅ טיפול בקריאה בוטל בהצלחה");
//                                }
//                                else
//                                    Console.WriteLine("❗ מספר שיוך לא חוקי");
//                            }
//                            else
//                                Console.WriteLine("❗ מספר מבקש לא חוקי");
//                        }
//                        catch (Exception ex)
//                        {
//                            Console.WriteLine($"❌ שגיאה: {ex.GetType().Name} - {ex.Message}");
//                            if (ex.InnerException != null)
//                                Console.WriteLine($"שגיאה פנימית: {ex.InnerException.Message}");
//                        }
//                        break;

//                    case "8":
//                        try
//                        {
//                            Console.Write("הזן מספר מתנדב: ");
//                            if (int.TryParse(Console.ReadLine(), out int volunteerId))
//                            {
//                                Console.Write("הזן מספר קריאה: ");
//                                if (int.TryParse(Console.ReadLine(), out int callId))
//                                {
//                                    s_bl.Call.SelectCallForTreatment(volunteerId, callId);
//                                    Console.WriteLine("✅ קריאה נבחרה לטיפול בהצלחה");
//                                }
//                                else
//                                    Console.WriteLine("❗ מספר קריאה לא חוקי");
//                            }
//                            else
//                                Console.WriteLine("❗ מספר מתנדב לא חוקי");
//                        }
//                        catch (Exception ex)
//                        {
//                            Console.WriteLine($"❌ שגיאה: {ex.GetType().Name} - {ex.Message}");
//                            if (ex.InnerException != null)
//                                Console.WriteLine($"שגיאה פנימית: {ex.InnerException.Message}");
//                        }
//                        break;

//                    case "9":
//                        try
//                        {
//                            Console.Write("הזן מספר מתנדב: ");
//                            if (int.TryParse(Console.ReadLine(), out int volunteerId))
//                            {
//                                Console.WriteLine("\nאפשרויות סינון לפי סוג קריאה:");
//                                Console.WriteLine("0. ללא סינון");
//                                Console.WriteLine("1. חירום");
//                                Console.WriteLine("2. רפואי");
//                                Console.WriteLine("3. אספקה");
//                                Console.Write("בחר אפשרות: ");

//                                CallType? callTypeFilter = null;
//                                string filterOption = Console.ReadLine() ?? "0";
//                                if (filterOption != "0" && int.TryParse(filterOption, out int callTypeInt) &&
//                                    callTypeInt >= 1 && callTypeInt <= 3)
//                                {
//                                    callTypeFilter = (CallType)(callTypeInt - 1);
//                                }

//                                Console.WriteLine("\nאפשרויות מיון:");
//                                Console.WriteLine("0. ללא מיון מיוחד");
//                                Console.WriteLine("1. מיון לפי זמן סיום");
//                                Console.WriteLine("2. מיון לפי סוג קריאה");
//                                Console.Write("בחר אפשרות: ");

//                                ClosedCallInListEnum? sortField = null;
//                                string sortOption = Console.ReadLine() ?? "0";
//                                if (sortOption != "0" && int.TryParse(sortOption, out int sortInt))
//                                {
//                                    if (sortInt == 1)
//                                        sortField = ClosedCallInListEnum.ActualTreatmentEndTime;
//                                    else if (sortInt == 2)
//                                        sortField = ClosedCallInListEnum.CallType;
//                                }

//                                Console.WriteLine("\nרשימת קריאות סגורות:");
//                                foreach (var closedCall in s_bl.Call.GetClosedCallsOfVolunteer(volunteerId, callTypeFilter, sortField))
//                                    Console.WriteLine(closedCall);
//                            }
//                            else
//                                Console.WriteLine("❗ מספר מתנדב לא חוקי");
//                        }
//                        catch (Exception ex)
//                        {
//                            Console.WriteLine($"❌ שגיאה: {ex.GetType().Name} - {ex.Message}");
//                            if (ex.InnerException != null)
//                                Console.WriteLine($"שגיאה פנימית: {ex.InnerException.Message}");
//                        }
//                        break;

//                    case "10":
//                        try
//                        {
//                            Console.Write("הזן מספר מתנדב: ");
//                            if (int.TryParse(Console.ReadLine(), out int volunteerId))
//                            {
//                                Console.WriteLine("\nאפשרויות סינון לפי סוג קריאה:");
//                                Console.WriteLine("0. ללא סינון");
//                                Console.WriteLine("1. חירום");
//                                Console.WriteLine("2. רפואי");
//                                Console.WriteLine("3. אספקה");
//                                Console.Write("בחר אפשרות: ");

//                                CallType? callTypeFilter = null;
//                                string filterOption = Console.ReadLine() ?? "0";
//                                if (filterOption != "0" && int.TryParse(filterOption, out int callTypeInt) &&
//                                    callTypeInt >= 1 && callTypeInt <= 3)
//                                {
//                                    callTypeFilter = (CallType)(callTypeInt - 1);
//                                }
//                                //todo להוסיף עוד מיונים
//                                Console.WriteLine("\nאפשרויות מיון:");
//                                Console.WriteLine("0. ללא מיון מיוחד");
//                                Console.WriteLine("1. מיון לפי דחיפות");
//                                Console.WriteLine("2. מיון לפי מרחק");
//                                Console.WriteLine("3. מיון לפי סוג קריאה");
//                                Console.Write("בחר אפשרות: ");

//                                OpenCallInListEnum? sortField = null;
//                                string sortOption = Console.ReadLine() ?? "0";
//                                if (sortOption != "0" && int.TryParse(sortOption, out int sortInt))
//                                {
//                                    if (sortInt == 1)
//                                        sortField = OpenCallInListEnum.MaxEndTime;
//                                    else if (sortInt == 2)
//                                        sortField = OpenCallInListEnum.DistanceFromVolunteer;
//                                    else if (sortInt == 3)
//                                        sortField = OpenCallInListEnum.CallType;
//                                }

//                                Console.WriteLine("\nרשימת קריאות פתוחות:");
//                                foreach (var openCall in s_bl.Call.GetOpenCallsForVolunteer(volunteerId, callTypeFilter, sortField))
//                                    Console.WriteLine(openCall);
//                            }
//                            else
//                                Console.WriteLine("❗ מספר מתנדב לא חוקי");
//                        }
//                        catch (Exception ex)
//                        {
//                            Console.WriteLine($"❌ שגיאה: {ex.GetType().Name} - {ex.Message}");
//                            if (ex.InnerException != null)
//                                Console.WriteLine($"שגיאה פנימית: {ex.InnerException.Message}");
//                        }
//                        break;

//                    case "0": back = true; break;
//                    default: Console.WriteLine("❗ בחירה לא חוקית"); break;
//                }
//            }
//        }

//        static void VolunteerMenu()
//        {
//            bool back = false;
//            while (!back)
//            {
//                Console.WriteLine("\n🤝 תפריט מתנדבים:");
//                Console.WriteLine("1. הוספת מתנדב");
//                Console.WriteLine("2. עדכון מתנדב");
//                Console.WriteLine("3. מחיקת מתנדב");
//                Console.WriteLine("4. צפייה בפרטי מתנדב");
//                Console.WriteLine("5. צפייה ברשימת מתנדבים");
//                Console.WriteLine("6. התחברות למערכת");
//                Console.WriteLine("0. חזרה לתפריט הראשי");
//                Console.Write("בחר אפשרות: ");

//                switch (Console.ReadLine())
//                {
//                    case "1":
//                        try
//                        {
//                            BO.Volunteer volunteer;
//                            Console.Write("הזן ת.ז.: ");
//                            if (int.TryParse(Console.ReadLine(), out int id))
//                            {
//                                volunteer = new BO.Volunteer() { Id = id };
//                            }

//                            else
//                            {
//                                Console.WriteLine("❗ ת.ז. לא חוקי");
//                                break;
//                            }

//                            Console.Write("הזן שם: ");
//                            volunteer.Name = Console.ReadLine() ?? string.Empty;

//                            Console.Write("הזן טלפון: ");
//                            volunteer.Phone = Console.ReadLine() ?? string.Empty;

//                            Console.Write("הזן דוא\"ל: ");
//                            volunteer.Email = Console.ReadLine() ?? string.Empty;

//                            Console.Write("הזן סיסמה: ");
//                            volunteer.Password = Console.ReadLine();

//                            Console.Write("הזן כתובת: ");
//                            volunteer.Address = Console.ReadLine();

//                            Console.Write("הזן קו רוחב (Latitude) או הקש Enter לדלג: ");
//                            string? latInput = Console.ReadLine();
//                            if (!string.IsNullOrEmpty(latInput) && double.TryParse(latInput, out double lat))
//                                volunteer.Latitude = lat;

//                            Console.Write("הזן קו אורך (Longitude) או הקש Enter לדלג: ");
//                            string? lonInput = Console.ReadLine();
//                            if (!string.IsNullOrEmpty(lonInput) && double.TryParse(lonInput, out double lon))
//                                volunteer.Longitude = lon;

//                            Console.Write("בחר תפקיד (0-מתנדב, 1-מנהל): ");
//                            if (int.TryParse(Console.ReadLine(), out int role) && Enum.IsDefined(typeof(VolunteerRole), role))
//                                volunteer.Role = (VolunteerRole)role;
//                            else
//                            {
//                                Console.WriteLine("❗ תפקיד לא חוקי");
//                                break;
//                            }

//                            Console.Write("האם פעיל? (true/false): ");
//                            if (bool.TryParse(Console.ReadLine(), out bool isActive))
//                                volunteer.IsActive = isActive;
//                            else
//                            {
//                                Console.WriteLine("❗ ערך לא חוקי");
//                                break;
//                            }

//                            Console.Write("הזן מרחק מקסימלי או הקש Enter לדלג: ");
//                            string? maxDistInput = Console.ReadLine();
//                            if (!string.IsNullOrEmpty(maxDistInput) && double.TryParse(maxDistInput, out double maxDist))
//                                volunteer.MaxDistance = maxDist;

//                            Console.Write("בחר סוג מרחק (0-קווי, 1-נסיעה) או הקש Enter לדלג: ");
//                            string? distKindInput = Console.ReadLine();
//                            if (!string.IsNullOrEmpty(distKindInput) && int.TryParse(distKindInput, out int distKind) &&
//                                Enum.IsDefined(typeof(DistanceKind), distKind))
//                                volunteer.DistanceKind = (DistanceKind)distKind;

//                            s_bl.Volunteer.AddVolunteer(volunteer);
//                            Console.WriteLine("✅ מתנדב נוסף בהצלחה");
//                        }
//                        catch (Exception ex)
//                        {
//                            Console.WriteLine($"❌ שגיאה: {ex.GetType().Name} - {ex.Message}");
//                            if (ex.InnerException != null)
//                                Console.WriteLine($"שגיאה פנימית: {ex.InnerException.Message}");
//                        }
//                        break;

//                    case "2":
//                        try
//                        {
//                            Console.Write("הזן ת.ז. של המתנדב לעדכון: ");
//                            string? idInput = Console.ReadLine();
//                            if (!string.IsNullOrEmpty(idInput))
//                            {
//                                BO.Volunteer volunteer = s_bl.Volunteer.GetVolunteerDetails(idInput);

//                                Console.Write($"שם נוכחי: {volunteer.Name}, הזן שם חדש או הקש Enter לדלג: ");
//                                string? nameInput = Console.ReadLine();
//                                if (!string.IsNullOrEmpty(nameInput))
//                                    volunteer.Name = nameInput;

//                                Console.Write($"טלפון נוכחי: {volunteer.Phone}, הזן טלפון חדש או הקש Enter לדלג: ");
//                                string? phoneInput = Console.ReadLine();
//                                if (!string.IsNullOrEmpty(phoneInput))
//                                    volunteer.Phone = phoneInput;

//                                Console.Write($"דוא\"ל נוכחי: {volunteer.Email}, הזן דוא\"ל חדש או הקש Enter לדלג: ");
//                                string? emailInput = Console.ReadLine();
//                                if (!string.IsNullOrEmpty(emailInput))
//                                    volunteer.Email = emailInput;

//                                Console.Write("הזן סיסמה חדשה או הקש Enter לדלג: ");
//                                string? passwordInput = Console.ReadLine();
//                                if (!string.IsNullOrEmpty(passwordInput))
//                                    volunteer.Password = passwordInput;

//                                Console.Write($"כתובת נוכחית: {volunteer.Address}, הזן כתובת חדשה או הקש Enter לדלג: ");
//                                string? addressInput = Console.ReadLine();
//                                if (!string.IsNullOrEmpty(addressInput))
//                                    volunteer.Address = addressInput;

//                                Console.Write($"קו רוחב נוכחי: {volunteer.Latitude}, הזן קו רוחב חדש או הקש Enter לדלג: ");
//                                string? latInput = Console.ReadLine();
//                                if (!string.IsNullOrEmpty(latInput) && double.TryParse(latInput, out double newLat))
//                                    volunteer.Latitude = newLat;

//                                Console.Write($"קו אורך נוכחי: {volunteer.Longitude}, הזן קו אורך חדש או הקש Enter לדלג: ");
//                                string? lonInput = Console.ReadLine();
//                                if (!string.IsNullOrEmpty(lonInput) && double.TryParse(lonInput, out double newLon))
//                                    volunteer.Longitude = newLon;

//                                Console.Write($"תפקיד נוכחי: {volunteer.Role}, בחר תפקיד חדש (0-מתנדב, 1-מנהל) או הקש Enter לדלג: ");
//                                string? roleInput = Console.ReadLine();
//                                if (!string.IsNullOrEmpty(roleInput) && int.TryParse(roleInput, out int newRole) &&
//                                    Enum.IsDefined(typeof(VolunteerRole), newRole))
//                                    volunteer.Role = (VolunteerRole)newRole;

//                                Console.Write($"פעיל כעת: {volunteer.IsActive}, האם פעיל? (true/false) או הקש Enter לדלג: ");
//                                string? activeInput = Console.ReadLine();
//                                if (!string.IsNullOrEmpty(activeInput) && bool.TryParse(activeInput, out bool newActive))
//                                    volunteer.IsActive = newActive;

//                                Console.Write($"מרחק מקסימלי נוכחי: {volunteer.MaxDistance}, הזן מרחק מקסימלי חדש או הקש Enter לדלג: ");
//                                string? maxDistInput = Console.ReadLine();
//                                if (!string.IsNullOrEmpty(maxDistInput) && double.TryParse(maxDistInput, out double newMaxDist))
//                                    volunteer.MaxDistance = newMaxDist;

//                                Console.Write($"סוג מרחק נוכחי: {volunteer.DistanceKind}, בחר סוג מרחק חדש (0-קווי, 1-נסיעה) או הקש Enter לדלג: ");
//                                string? distKindInput = Console.ReadLine();
//                                if (!string.IsNullOrEmpty(distKindInput) && int.TryParse(distKindInput, out int newDistKind) &&
//                                    Enum.IsDefined(typeof(DistanceKind), newDistKind))
//                                    volunteer.DistanceKind = (DistanceKind)newDistKind;

//                                s_bl.Volunteer.UpdateVolunteer(idInput, volunteer);
//                                Console.WriteLine("✅ מתנדב עודכן בהצלחה");
//                            }
//                            else
//                            {

//                            }
//                        }
//                        catch (Exception ex)
//                        {
//                            Console.WriteLine($"❌ שגיאה: {ex.GetType().Name} - {ex.Message}");
//                            if (ex.InnerException != null)
//                                Console.WriteLine($"שגיאה פנימית: {ex.InnerException.Message}");
//                        }



//                        break;
//                    case "3":
//                        try
//                        {
//                            Console.Write("הזן ת.ז. של המתנדב למחיקה: ");
//                            string? idToDelete = Console.ReadLine();
//                            s_bl.Volunteer.DeleteVolunteer(idToDelete);
//                            Console.WriteLine("🗑️ מתנדב נמחק בהצלחה");
//                        }
//                        catch (Exception ex)
//                        {
//                            Console.WriteLine($"❌ שגיאה: {ex.Message}");
//                        }
//                        break;

//                    case "4":
//                        try
//                        {
//                            Console.Write("הזן ת.ז. של המתנדב: ");
//                            string? idToView = Console.ReadLine();
//                            var volunteer1 = s_bl.Volunteer.GetVolunteerDetails(idToView);
//                            Console.WriteLine(volunteer1);
//                        }
//                        catch (Exception ex)
//                        {
//                            Console.WriteLine($"❌ שגיאה: {ex.Message}");
//                        }
//                        break;

//                    case "5":
//                        try
//                        {
//                            var volunteers = s_bl.Volunteer.GetVolunteersList();
//                            foreach (var v in volunteers)
//                                Console.WriteLine(v);
//                        }
//                        catch (Exception ex)
//                        {
//                            Console.WriteLine($"❌ שגיאה: {ex.Message}");
//                        }
//                        break;

//                    case "6":
//                        try
//                        {
//                            Console.Write("הזן ת.ז.: ");
//                            string? idLogin = Console.ReadLine();

//                            Console.Write("הזן סיסמה: ");
//                            string? password = Console.ReadLine();
//                            BO.VolunteerRole role = s_bl.Volunteer.Login(idLogin, password);
//                        }
//                        catch (Exception ex)
//                        {
//                            Console.WriteLine($"❌ התחברות נכשלה: {ex.Message}");
//                        }
//                        break;

//                    case "0":
//                        back = true;
//                        break;

//                    default:
//                        Console.WriteLine("❗ בחירה לא חוקית");
//                        break;
//                }
//            }
//        }
//    }
//}
using System;
using System.Globalization;
using System.Xml.Linq;
using BlApi;
using BO;
using DalApi;
namespace BlTest
{
    class Program
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        static void Main()
        {
            try
            {
                while (true)
                {
                    Console.WriteLine("\n--- BL Test System ---");
                    Console.WriteLine("1. Administration");
                    Console.WriteLine("2. Volunteers");
                    Console.WriteLine("3. Calls");
                    Console.WriteLine("0. Exit");
                    Console.Write("Choose an option: ");

                    if (int.TryParse(Console.ReadLine(), out int choice))
                    {
                        switch (choice)
                        {
                            case 1:
                                AdminMenu();
                                break;
                            case 2:
                                VolunteerMenu();
                                break;
                            case 3:
                                CallMenu();
                                break;
                            case 0:
                                return;
                            default:
                                Console.WriteLine("Invalid choice. Try again.");
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while displaying the menu: " + ex.Message);
            }
        }


        static void AdminMenu()
        {
            while (true)
            {
                Console.WriteLine("\n--- Administration ---");
                Console.WriteLine("1. Reset Database");
                Console.WriteLine("2. Initialize Database");
                Console.WriteLine("3. Advance Clock");
                Console.WriteLine("4. Show Clock");
                Console.WriteLine("5. Get Risk Time Range");
                Console.WriteLine("6. Set Risk Time Range");
                Console.WriteLine("0. Back");
                Console.Write("Choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                    continue;
                }

                try
                {
                    switch (choice)
                    {
                        case 1:
                            s_bl.Admin.ResetDatabase();
                            Console.WriteLine("Database reset successfully");
                            break;
                        case 2:
                            s_bl.Admin.InitializeDatabase();
                            Console.WriteLine("Database initialized successfully");
                            break;
                        case 3:
                            Console.Write("Enter time unit (Minute, Hour, Day, Month, Year): ");
                            if (Enum.TryParse(Console.ReadLine(), true, out BO.TimeUnit timeUnit))
                            {
                                s_bl.Admin.AdvanceTime(timeUnit);
                                Console.WriteLine("System clock advanced.");
                            }
                            else
                            {
                                throw new FormatException("Invalid time unit. Please enter: Minute, Hour, Day, Month, Year.");
                            }
                            break;
                        case 4:
                            Console.WriteLine($"Current System Clock: {s_bl.Admin.GetCurrentTime()}");
                            break;
                        case 5:
                            Console.WriteLine($"Current Risk Time Range: {s_bl.Admin.GetRiskTimeSpan()}");
                            break;
                        case 6:
                            Console.Write("Enter new risk time range (hh:mm:ss): ");
                            if (TimeSpan.TryParse(Console.ReadLine(), out TimeSpan timeRange))
                            {
                                s_bl.Admin.SetRiskTimeSpan(timeRange);
                                Console.WriteLine("Risk time range updated.");
                            }
                            else
                            {
                                throw new FormatException("Invalid time format. Please use hh:mm:ss.");
                            }
                            break;
                        case 0:
                            return;
                        default:
                            Console.WriteLine("Invalid choice. Try again.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }
        }

        static void VolunteerMenu()
        {
            while (true)
            {
                Console.WriteLine("\n--- Volunteer Management ---");
                Console.WriteLine("1. Login");
                Console.WriteLine("2. List Volunteers");
                Console.WriteLine("3. Get Filter/Sort volunteer");
                Console.WriteLine("4. Read Volunteer by ID");
                Console.WriteLine("5. Add Volunteer");
                Console.WriteLine("6. Remove Volunteer");
                Console.WriteLine("7. UpDate Volunteer");
                Console.WriteLine("0. Back");
                Console.Write("Choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                    throw new FormatException("The volunteer menu choice is not valid.");
                switch (choice)
                {
                    case 1:
                        try
                        {
                            Console.WriteLine("Please log in.");
                            Console.Write("Username: ");
                            string username = Console.ReadLine()!;

                            Console.Write("Enter Password (must be at least 8 characters, contain upper and lower case letters, a digit, and a special character): ");
                            string password = Console.ReadLine()!;

                            BO.VolunteerRole userRole = s_bl.Volunteer.Login(username, password);
                            Console.WriteLine($"Login successful! Your role is: {userRole}");
                        }
                        catch (Exception ex)
                        {
                            HandleException(ex);
                        }
                        break;
                    case 2:
                        try
                        {
                            foreach (var volunteer in s_bl.Volunteer.GetVolunteersList(true, VolunteerInListEnum.Name))
                                Console.WriteLine(volunteer);
                        }
                        catch (Exception ex)
                        {
                            HandleException(ex);
                        }
                        break;
                    case 3:
                        try
                        {
                            bool? isActive;
                            BO.VolunteerInListEnum? sortBy;
                            GetVolunteerFilterAndSortCriteria(out isActive, out sortBy);
                            var volunteersList = s_bl.Volunteer.GetVolunteersList(isActive, sortBy);
                            if (volunteersList != null)
                                foreach (var volunteer in volunteersList)
                                    Console.WriteLine(volunteer);
                            else
                                Console.WriteLine("No volunteers found matching the criteria.");
                        }
                        catch (Exception ex)
                        {
                            HandleException(ex);
                        }
                        break;
                    case 4:
                        try
                        {
                            Console.Write("Enter Volunteer ID: ");
                            string volunteerId = Console.ReadLine();
                            var volunteer = s_bl.Volunteer.GetVolunteerDetails(volunteerId);
                            Console.WriteLine(volunteer);

                        }
                        catch (Exception ex)
                        {
                            HandleException(ex);
                        }
                        break;
                    case 5:
                        try
                        {
                            BO.Volunteer volunteer;
                            Console.Write("הזן ת.ז.: ");
                            if (int.TryParse(Console.ReadLine(), out int id))
                            {
                                volunteer = new BO.Volunteer() { Id = id };
                            }

                            else
                            {
                                Console.WriteLine("❗ ת.ז. לא חוקי");
                                break;
                            }

                            Console.Write("הזן שם: ");
                            volunteer.Name = Console.ReadLine() ?? string.Empty;

                            Console.Write("הזן טלפון: ");
                            volunteer.Phone = Console.ReadLine() ?? string.Empty;

                            Console.Write("הזן דוא\"ל: ");
                            volunteer.Email = Console.ReadLine() ?? string.Empty;

                            Console.Write("הזן סיסמה: ");
                            volunteer.Password = Console.ReadLine();

                            Console.Write("הזן כתובת: ");
                            volunteer.Address = Console.ReadLine();

                            Console.Write("הזן קו רוחב (Latitude) או הקש Enter לדלג: ");
                            string? latInput = Console.ReadLine();
                            if (!string.IsNullOrEmpty(latInput) && double.TryParse(latInput, out double lat))
                                volunteer.Latitude = lat;

                            Console.Write("הזן קו אורך (Longitude) או הקש Enter לדלג: ");
                            string? lonInput = Console.ReadLine();
                            if (!string.IsNullOrEmpty(lonInput) && double.TryParse(lonInput, out double lon))
                                volunteer.Longitude = lon;

                            Console.Write("בחר תפקיד (0-מתנדב, 1-מנהל): ");
                            if (int.TryParse(Console.ReadLine(), out int role) && Enum.IsDefined(typeof(VolunteerRole), role))
                                volunteer.Role = (VolunteerRole)role;
                            else
                            {
                                Console.WriteLine("❗ תפקיד לא חוקי");
                                break;
                            }

                            Console.Write("האם פעיל? (true/false): ");
                            if (bool.TryParse(Console.ReadLine(), out bool isActive))
                                volunteer.IsActive = isActive;
                            else
                            {
                                Console.WriteLine("❗ ערך לא חוקי");
                                break;
                            }

                            Console.Write("הזן מרחק מקסימלי או הקש Enter לדלג: ");
                            string? maxDistInput = Console.ReadLine();
                            if (!string.IsNullOrEmpty(maxDistInput) && double.TryParse(maxDistInput, out double maxDist))
                                volunteer.MaxDistance = maxDist;

                            Console.Write("בחר סוג מרחק (0-קווי, 1-נסיעה) או הקש Enter לדלג: ");
                            string? distKindInput = Console.ReadLine();
                            if (!string.IsNullOrEmpty(distKindInput) && int.TryParse(distKindInput, out int distKind) &&
                                Enum.IsDefined(typeof(DistanceKind), distKind))
                                volunteer.DistanceKind = (DistanceKind)distKind;

                            s_bl.Volunteer.AddVolunteer(volunteer);
                            Console.WriteLine("✅ Volunteer added succesfully");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"❌ Exception: {ex.GetType().Name} - {ex.Message}");
                            if (ex.InnerException != null)
                                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                        }
                        break;


                    case 6:
                        try
                        {
                            Console.Write("Enter Volunteer ID: ");
                            string? idToDelete = Console.ReadLine();
                            s_bl.Volunteer.DeleteVolunteer(idToDelete);
                            Console.WriteLine("Volunteer deleted succesfully");
                        }
                        catch (Exception ex)
                        {
                            HandleException(ex);
                        }
                        break;

                    case 7:
                        UpDateVolunteer();
                        break;

                    case 0:
                        return;

                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        break;
                }

            }
        }
        public static void GetVolunteerFilterAndSortCriteria(out bool? isActive, out BO.VolunteerInListEnum? sortBy)
        {
            isActive = null;
            sortBy = null;

            try
            {

                Console.WriteLine("Is the volunteer active? (yes/no or leave blank for null): ");
                string activeInput = Console.ReadLine();

                if (!string.IsNullOrEmpty(activeInput))
                {
                    if (activeInput.Equals("yes", StringComparison.OrdinalIgnoreCase))
                        isActive = true;
                    else if (activeInput.Equals("no", StringComparison.OrdinalIgnoreCase))
                        isActive = false;
                    else
                        Console.WriteLine("Invalid input for active status. Defaulting to null.");
                }

                Console.WriteLine("Choose how to sort the volunteers by: ");
                Console.WriteLine("1. By FullName");
                Console.WriteLine("2. By HandledCallsCount");
                Console.WriteLine("3. By IsActive");
                Console.WriteLine("4. By CallInProgressId");
                Console.WriteLine("Select sorting option by number: ");
                string sortInput = Console.ReadLine();

                if (int.TryParse(sortInput, out int sortOption))
                {
                    switch (sortOption)
                    {
                        case 1:
                            sortBy = BO.VolunteerInListEnum.Name;
                            break;
                        case 2:
                            sortBy = BO.VolunteerInListEnum.HandledCallsCount;
                            break;
                        case 3:
                            sortBy = BO.VolunteerInListEnum.IsActive;
                            break;
                        case 4:
                            sortBy = BO.VolunteerInListEnum.CallInProgressId;
                            break;
                        default:
                            Console.WriteLine("Invalid selection. Defaulting to sorting by ID.");
                            break;
                    }
                }
                else
                {
                    throw new FormatException("Invalid input for sorting option. Defaulting to sorting by ID.");
                }
            }
            catch (BO.BlGeneralException ex)
            {
                Console.WriteLine($"Exception: {ex.GetType().Name}");
                Console.WriteLine($"Message: {ex.Message}");
            }
        }
        static BO.Volunteer CreateVolunteer(int requesterId)
        {

            Console.Write("Name: ");
            string? name = Console.ReadLine();

            Console.Write("Phone Number: ");
            string? phoneNumber = Console.ReadLine();

            Console.Write("Email: ");
            string? email = Console.ReadLine();

            Console.Write("IsActive? (true/false): ");
            if (!bool.TryParse(Console.ReadLine(), out bool active))
                throw new FormatException("Invalid input for IsActive.");

            Console.WriteLine("Please enter Role: 'Manager' or 'Volunteer'.");
            if (!Enum.TryParse(Console.ReadLine(), out BO.VolunteerRole role))
                throw new FormatException("Invalid role.");

            Console.Write("Password: ");
            string? password = Console.ReadLine();

            Console.Write("Address: ");
            string? address = Console.ReadLine();

            Console.WriteLine("Enter location details:");
            Console.Write("Latitude: ");
            if (!double.TryParse(Console.ReadLine(), out double latitude))
                throw new FormatException("Invalid latitude format.");

            Console.Write("Longitude: ");
            if (!double.TryParse(Console.ReadLine(), out double longitude))
                throw new FormatException("Invalid longitude format.");

            Console.Write("Largest Distance: ");
            if (!double.TryParse(Console.ReadLine(), out double MaxDistanceForCall))
                throw new FormatException("Invalid Max Distance For Call format.");

            Console.Write("Distance Type (Air, Drive or Walk): ");
            if (!Enum.TryParse(Console.ReadLine(), true, out BO.DistanceKind myDistanceType))
                throw new FormatException("Invalid distance type.");

            return new BO.Volunteer
            {
                Id = requesterId,
                Name = name,
                Phone = phoneNumber,
                Email = email,
                Password = password,
                Address = address,
                Latitude = latitude,
                Longitude = longitude,
                Role = role,
                IsActive = active,
                MaxDistance = MaxDistanceForCall,
                DistanceKind = myDistanceType,
            };
        }
        static void UpDateVolunteer()
        {
            try
            {
                Console.Write("Enter requester ID: ");
                string requesterId = Console.ReadLine();
                if (!int.TryParse(requesterId, out int requesterIdInt))
                {
                    throw new FormatException("Invalid input for requester ID.");
                }
                BO.Volunteer boVolunteer = CreateVolunteer(requesterIdInt);
                s_bl.Volunteer.UpdateVolunteer(requesterId, boVolunteer);
                Console.WriteLine("Volunteer updated successfully.");
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }


        static void CallMenu()
        {
            try
            {

                while (true)
                {
                    Console.WriteLine("\n--- Call Management ---");
                    Console.WriteLine("1. Get call quantities by status");
                    Console.WriteLine("2. Get Closed Calls Handled By Volunteer");
                    Console.WriteLine("3. Show All Calls");
                    Console.WriteLine("4. Read Call by ID");
                    Console.WriteLine("5. Add Call");
                    Console.WriteLine("6. Remove Call");
                    Console.WriteLine("7. Update Call");
                    Console.WriteLine("8. Get Open Calls For Volunteer");
                    Console.WriteLine("9. Mark Call As Canceled");
                    Console.WriteLine("10. Mark Call As Completed");
                    Console.WriteLine("11. Select Call For Treatment");
                    Console.WriteLine("0. Back");
                    Console.Write("Choose an option: ");

                    if (!int.TryParse(Console.ReadLine(), out int choice))
                        throw new FormatException("The call menu choice is not valid.");

                    switch (choice)
                    {
                        case 1:
                            try
                            {
                                IEnumerable<int> callQuantities = s_bl.Call.GetCallStatusCounts();

                                Console.WriteLine("Call quantities by status:");

                                int index = 0;
                                foreach (BO.CallStatus status in Enum.GetValues(typeof(BO.CallStatus)))
                                {
                                    Console.WriteLine($"{status}: {callQuantities.ElementAt(index)}");
                                    index++;
                                }
                            }
                            catch (Exception ex)
                            {
                                HandleException(ex);
                            }
                            break;
                        case 2:
                            try
                            {
                                Console.Write("Enter Volunteer ID: ");
                                if (int.TryParse(Console.ReadLine(), out int volunteerId))
                                {
                                    Console.WriteLine("Enter Call Type filter (Emergency,Equipment,Doctor,Training) or press Enter to skip:");
                                    string? callTypeInput = Console.ReadLine();
                                    BO.CallType? callTypeFilter = Enum.TryParse(callTypeInput, out BO.CallType parsedCallType) ? parsedCallType : null;

                                    Console.WriteLine("Enter Sort Field (Completed, SelfCancelled, ManagerCancelled, Expired) or press Enter to skip:");
                                    string? sortFieldInput = Console.ReadLine();
                                    BO.ClosedCallInListEnum? sortField = Enum.TryParse(sortFieldInput, out BO.ClosedCallInListEnum parsedSortField) ? (BO.ClosedCallInListEnum?)parsedSortField : null;

                                    var closedCalls = s_bl.Call.GetClosedCallsOfVolunteer(volunteerId, callTypeFilter, sortField);

                                    Console.WriteLine("\nClosed Calls Handled By Volunteer:");
                                    foreach (var call in closedCalls)
                                    {
                                        Console.WriteLine(call);
                                    }
                                }
                                else
                                {
                                    throw new BO.BlFormatException("Invalid input. Volunteer ID must be a number.");
                                }
                            }
                            catch (Exception ex)
                            {
                                HandleException(ex);
                            }
                            break;
                        case 3:
                            try
                            {
                                //GetCallsList(BO.CallType ? filterField, object ? filterValue, string ? sortField)
                                Console.WriteLine("Enter Call Type filter (Emergency,Equipment,Doctor,Training) or press Enter to skip:");
                                string? filterFieldInput = Console.ReadLine();
                                BO.CallType? filterValue = Enum.TryParse(filterFieldInput, out BO.CallType parsedFilterField) ? parsedFilterField : null;

                                Console.WriteLine("Enter sort field ( Status,  Type,  CallerName ) or press Enter to skip:");
                                string? sortFieldInput = Console.ReadLine();
                                BO.CallInListField? sortField = Enum.TryParse(sortFieldInput, out BO.CallInListField parsedSortField) ? (BO.CallInListField?)parsedSortField : null;

                                var callList = s_bl.Call.GetCallList(CallInListField.Status, filterValue, sortField);

                                foreach (var call in callList)
                                    Console.WriteLine(call);
                            }
                            catch (Exception ex)
                            {
                                HandleException(ex);
                            }
                            break;
                        case 4:
                            try
                            {
                                Console.Write("Enter Call ID: ");
                                if (int.TryParse(Console.ReadLine(), out int callId))
                                {
                                    var call = s_bl.Call.GetCallDetails(callId);
                                    Console.WriteLine(call);
                                }
                                else
                                {
                                    throw new FormatException("Invalid input. Volunteer ID must be a number.");
                                }
                            }
                            catch (Exception ex)
                            {
                                HandleException(ex);
                            }
                            break;
                        case 5:
                            try
                            {
                                BO.Call call = new BO.Call();

                                Console.Write("הזן סוג קריאה (0-חירום, 1-רפואי, 2-אספקה): ");
                                if (int.TryParse(Console.ReadLine(), out int callType) && Enum.IsDefined(typeof(CallType), callType))
                                    call.CallType = (CallType)callType;
                                else
                                {
                                    Console.WriteLine("❗ סוג קריאה לא חוקי");
                                    break;
                                }

                                Console.Write("הזן תיאור: ");
                                call.Description = Console.ReadLine();

                                Console.Write("הזן כתובת: ");
                                call.Address = Console.ReadLine() ?? string.Empty;

                                Console.Write("הזן קו רוחב (Latitude): ");
                                double.TryParse(Console.ReadLine(), out double lat);
                                call.Latitude = lat;


                                Console.Write("הזן קו אורך (Longitude): ");
                                double.TryParse(Console.ReadLine(), out double lon);
                                call.Longitude = lon;

                                call.CreationTime = s_bl.Admin.GetCurrentTime();
                                call.Status = CallStatus.Open;

                                s_bl.Call.AddCall(call);
                                Console.WriteLine("✅ קריאה נוספה בהצלחה");
                            }
                            catch (Exception ex)
                            {
                                HandleException(ex);
                            }
                            ;
                            break;
                        case 6:
                            try
                            {
                                Console.Write("Enter Call ID: ");
                                if (int.TryParse(Console.ReadLine(), out int cId))
                                {
                                    s_bl.Call.DeleteCall(cId);
                                    Console.WriteLine("Call removed.");
                                }
                                else
                                {
                                    throw new FormatException("Invalid input. Volunteer ID must be a number.");
                                }
                            }
                            catch (Exception ex)
                            {
                                HandleException(ex);
                            }
                            break;
                        case 7:
                            UpDateCall();
                            break;
                        case 8:
                            try
                            {
                                Console.Write("Enter Volunteer ID: ");
                                if (int.TryParse(Console.ReadLine(), out int volunteerId))
                                {
                                    Console.WriteLine("Enter Call Type filter (or press Enter to skip):");
                                    string? callTypeInput = Console.ReadLine();
                                    BO.CallType? callTypeFilter = Enum.TryParse(callTypeInput, out BO.CallType parsedCallType) ? parsedCallType : null;

                                    Console.WriteLine("Enter Sort Field (or press Enter to skip):");
                                    string? sortFieldInput = Console.ReadLine();
                                    BO.OpenCallInListEnum? sortField = Enum.TryParse(sortFieldInput, out BO.OpenCallInListEnum parsedSortField) ? parsedSortField : null;

                                    var openCalls = s_bl.Call.GetOpenCallsForVolunteer(volunteerId, callTypeFilter, sortField);

                                    Console.WriteLine("\nOpen Calls Available for Volunteer:");
                                    foreach (var call in openCalls)
                                    {
                                        Console.WriteLine(call);
                                    }
                                }
                                else
                                {
                                    throw new BO.BlFormatException("Invalid input. Volunteer ID must be a number.");
                                }
                            }
                            catch (Exception ex)
                            {
                                HandleException(ex);
                            }
                            break;
                        case 9:
                            try
                            {
                                Console.Write("Enter Volunteer ID: ");
                                if (!int.TryParse(Console.ReadLine(), out int volunteerId))
                                    throw new BO.BlFormatException("Invalid input. Volunteer ID must be a number.");

                                Console.Write("Enter call ID: ");
                                if (!int.TryParse(Console.ReadLine(), out int assignmentId))
                                    throw new BO.BlFormatException("Invalid input. call ID must be a number.");

                                s_bl.Call.CancelCallTreatment(volunteerId, assignmentId);
                                Console.WriteLine("The call was successfully canceled.");
                            }
                            catch (Exception ex)
                            {
                                HandleException(ex);
                            }
                            break;
                        case 10:
                            try
                            {
                                Console.Write("Enter Volunteer ID: ");
                                string? volunteerInput = Console.ReadLine();
                                if (!int.TryParse(volunteerInput, out int volunteerId))
                                {
                                    throw new FormatException("Invalid input. Volunteer ID must be a number.");
                                }

                                Console.Write("Enter Assignment ID: ");
                                string? assignmentInput = Console.ReadLine();
                                if (!int.TryParse(assignmentInput, out int assignmentId))
                                {
                                    throw new FormatException("Invalid input. Assignment ID must be a number.");
                                }

                                s_bl.Call.CompleteCallTreatment(volunteerId, assignmentId);

                                Console.WriteLine("Call completion updated successfully!");
                            }
                            catch (Exception ex)
                            {
                                HandleException(ex);
                            }
                            break;
                        case 11:
                            try
                            {
                                Console.Write("Enter Volunteer ID: ");
                                if (!int.TryParse(Console.ReadLine(), out int volunteerId))
                                    throw new FormatException("Invalid input. Volunteer ID must be a number.");

                                Console.Write("Enter Call ID: ");
                                if (!int.TryParse(Console.ReadLine(), out int callId))
                                    throw new FormatException("Invalid input. Call ID must be a number.");

                                s_bl.Call.SelectCallForTreatment(volunteerId, callId);
                                Console.WriteLine("The call has been successfully assigned to the volunteer.");
                            }
                            catch (Exception ex)
                            {
                                HandleException(ex);
                            }
                            break;
                        case 0:
                            return;
                        default:
                            Console.WriteLine("Invalid choice. Try again.");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        static BO.Call CreateCall(int id)
        {
            Console.WriteLine("Enter the call type (0 for Technical, 1 for Food, 2 for Medical, 3 for Emergency, 4 for Other, 5 for None):");
            if (!Enum.TryParse(Console.ReadLine(), out BO.CallType callType))
            {
                throw new FormatException("Invalid call type.");
            }

            Console.WriteLine("Enter the verbal description:");
            string verbalDescription = Console.ReadLine();

            Console.WriteLine("Enter the address:");
            string address = Console.ReadLine();



            Console.WriteLine("Enter the max finish time (yyyy-mm-dd) or leave empty:");
            string maxFinishTimeInput = Console.ReadLine();
            DateTime? maxFinishTime = string.IsNullOrEmpty(maxFinishTimeInput) ? null : DateTime.Parse(maxFinishTimeInput);

            Console.WriteLine("Enter the status (0 for InProgress, 1 for AtRisk, 2 for InProgressAtRisk, 3 for Opened, 4 for Closed, 5 for Expired):");
            if (!Enum.TryParse(Console.ReadLine(), out CallStatus status))
            {
                throw new FormatException("Invalid status.");
            }

            return new BO.Call
            {
                Id = id,
                CallType = callType,
                Description = verbalDescription,
                Address = address,
                Latitude = 0,
                Longitude = 0,
                CreationTime = s_bl.Admin.GetCurrentTime()
            };


        }
        static void UpDateCall()
        {
            Console.Write("Enter Call ID: ");
            int.TryParse(Console.ReadLine(), out int callId);
            Console.Write("Enter New Description (optional) : ");
            string description = Console.ReadLine();
            Console.Write("Enter New Full Address (optional) : ");
            string address = Console.ReadLine();
            Console.Write("Enter Call Type (optional) : ");
            BO.CallType? callType = Enum.TryParse(Console.ReadLine(), out BO.CallType parsedType) ? parsedType : (BO.CallType?)null;
            Console.Write("Enter Max Finish Time (hh:mm , (optional)): ");
            TimeSpan? maxFinishTime = TimeSpan.TryParse(Console.ReadLine(), out TimeSpan parsedTime) ? parsedTime : (TimeSpan?)null;
            try
            {
                var callToUpdate = s_bl.Call.GetCallDetails(callId);
                if (callToUpdate == null)
                    throw new BO.BlDoesNotExistException($"Call with ID{callId} does not exist!");
                var newUpdatedCall = new BO.Call
                {
                    Id = callId,
                    Description = !string.IsNullOrWhiteSpace(description) ? description : callToUpdate.Description,
                    Address = !string.IsNullOrWhiteSpace(address) ? address : /*callToUpdate. FullAddress*/"No Address",
                    CreationTime = callToUpdate.CreationTime,
                    MaxFinishTime = (maxFinishTime.HasValue ? DateTime.Now.Date + maxFinishTime.Value : callToUpdate.MaxFinishTime),
                    CallType = callType ?? callToUpdate.CallType
                };
                s_bl.Call.UpdateCall(newUpdatedCall);
                Console.WriteLine("Call updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.GetType().Name}, Message: {ex.Message}");
            }
        }
        static void HandleException(Exception ex)
        {
            switch (ex)
            {
                case BO.BlDoesNotExistException ex1:
                    Console.WriteLine($"Exception: {ex1.GetType().Name}, Message: {ex1.Message}");
                    break;
                case BO.BlInvalidOperationException ex2:
                    Console.WriteLine($"Exception: {ex2.GetType().Name}, Message: {ex2.Message}");
                    break;
                case BO.BlInvalidInputException ex3:
                    Console.WriteLine($"Exception: {ex3.GetType().Name}, Message: {ex3.Message}");
                    break;
                case BO.BlAlreadyExistsException ex4:
                    Console.WriteLine($"Exception: {ex4.GetType().Name}, Message: {ex4.Message}");
                    break;
                case BO.BlGeneralException ex5:
                    Console.WriteLine($"Exception: {ex5.GetType().Name}, Message: {ex5.Message}");
                    if (ex5.InnerException != null)
                    {
                        Console.WriteLine($"Inner Exception: {ex5.InnerException.Message}");
                    }
                    break;
                case BO.BlAuthorizationException ex9:
                    Console.WriteLine($"Unauthorized Access: {ex9.Message}");
                    break;
                case FormatException:
                    Console.WriteLine("Input format is incorrect. Please try again.");
                    break;
                case Exception exe:
                    Console.WriteLine($"An unexpected error occurred: {exe.Message}");
                    break;
            }
        }
    }

}





