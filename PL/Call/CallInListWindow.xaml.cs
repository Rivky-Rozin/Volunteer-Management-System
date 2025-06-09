using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PL.Call
{
    public partial class CallInListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public IEnumerable<BO.CallInList> CallList
        {
            get { return (IEnumerable<BO.CallInList>)GetValue(CallListProperty); }
            set { SetValue(CallListProperty, value); }
        }

        public static readonly DependencyProperty CallListProperty =
            DependencyProperty.Register("CallList", typeof(IEnumerable<BO.CallInList>), typeof(CallInListWindow), new PropertyMetadata(null));

        public CallInListField CallSelectMenus { get; set; } = BO.CallInListField.None;
        public object? FilterValue { get; set; }

        public CallInListWindow()
        {
            InitializeComponent();
            LoadCalls();
        }


        private void RefreshCallList()
        {
            // בקשת ערך לסינון רק אם צריך
            object? filterValue = null;
            switch (CallSelectMenus)
            {
                case CallInListField.Id:
                case CallInListField.CallId:
                case CallInListField.NumberOfAssignments:
                    if (InputBox("הכנס מספר לסינון:", "סינון", out string idStr) && int.TryParse(idStr, out int idVal))
                        filterValue = idVal;
                    break;
                case CallInListField.CallType:
                    if (InputBox("הכנס סוג קריאה (Technical, Food, Medical, Emergency, Other, None):", "סינון", out string typeStr)
                        && Enum.TryParse(typeof(CallType), typeStr, true, out object? typeVal))
                        filterValue = typeVal;
                    break;
                case CallInListField.OpenTime:
                    if (InputBox("הכנס תאריך פתיחה (yyyy-MM-dd או yyyy-MM-dd HH:mm):", "סינון", out string dateStr)
                        && DateTime.TryParse(dateStr, out DateTime dateVal))
                        filterValue = dateVal;
                    break;
                case CallInListField.TimeUntilAssigning:
                    if (InputBox("הכנס משך זמן עד שיבוץ (hh:mm:ss):", "סינון", out string tsStr)
                        && TimeSpan.TryParse(tsStr, out TimeSpan tsVal))
                        filterValue = tsVal;
                    break;
                case CallInListField.LastVolunteerName:
                    if (InputBox("הכנס שם מתנדב לסינון:", "סינון", out string nameStr))
                        filterValue = nameStr;
                    break;
                case CallInListField.totalTreatmentTime:
                    if (InputBox("הכנס משך טיפול כולל (hh:mm:ss):", "סינון", out string tttStr)
                        && TimeSpan.TryParse(tttStr, out TimeSpan tttVal))
                        filterValue = tttVal;
                    break;
                case CallInListField.Status:
                    if (InputBox("הכנס סטטוס (Open, InProgress, Closed, Expired, OpenAtRisk, InProgressAtRisk):", "סינון", out string statusStr)
                        && Enum.TryParse(typeof(CallStatus), statusStr, true, out object? statusVal))
                        filterValue = statusVal;
                    break;
                case CallInListField.None:
                default:
                    break;
            }

            FilterValue = filterValue;

            if (CallSelectMenus == BO.CallInListField.None)
                CallList = s_bl.Call.GetCallList(null, null, null).ToList();
            else
                CallList = s_bl.Call.GetCallList(CallSelectMenus, FilterValue, null).ToList();
        }

        // Event handler for ComboBox selection change
        private void CallComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // עדכון השדה בלבד, לא סינון ישיר
            //if (e.AddedItems.Count > 0 && e.AddedItems[0] is CallInListField selectedField)
            //    CallSelectMenus = selectedField;

            RefreshCallList();
        }

        // פונקציה פשוטה לקבלת קלט מהמשתמש (MessageBox עם TextBox)
        private static bool InputBox(string prompt, string title, out string value)
        {
            value = Microsoft.VisualBasic.Interaction.InputBox(prompt, title, "");
            return !string.IsNullOrWhiteSpace(value);
        }




        // עוטפת את השאילתא לסינון הרשימה
        private void QueryCallList()
        {
            if (CallSelectMenus == BO.CallInListField.None)
                CallList = s_bl.Call.GetCallList(null, null, null).ToList();
            else
                CallList = s_bl.Call.GetCallList(CallSelectMenus, FilterValue, null).ToList();
        }

        // מתודת ההשקפה (Observer)
        private void CallListObserver()
        {
            if (!Dispatcher.CheckAccess())
                Dispatcher.Invoke(QueryCallList);
            else
                QueryCallList();
        }

        // טוען את הרשימה הראשונית

        private void LoadCalls()
        {
            RefreshCallList();
            QueryCallList();
        }
        // אירוע טעינת חלון - הרשמה למשקיף
        private void Window_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Call.AddObserver(CallListObserver);

        // אירוע סגירת חלון - הסרת המשקיף
        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Call.RemoveObserver(CallListObserver);

    }
}