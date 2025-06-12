using BO;
using MyApp;
using PL.Volunteer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PL.Call
{
    public partial class CallInListWindow : Window
    {
        private readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private BO.CallStatus? _currentFilterStatus;

        // Use ObservableCollection for automatic UI updates
        public ObservableCollection<BO.CallInList> Calls { get; set; }

        public IEnumerable<BO.CallInList> CallList
        {
            get { return (IEnumerable<BO.CallInList>)GetValue(CallListProperty); }
            set { SetValue(CallListProperty, value); }
        }

        public static readonly DependencyProperty CallListProperty =
            DependencyProperty.Register("CallList", typeof(IEnumerable<BO.CallInList>), typeof(CallInListWindow), new PropertyMetadata(null));

        public CallInListField CallSelectMenus { get; set; } = BO.CallInListField.None;
        public object? FilterValue { get; set; }

        public CallInListWindow(BO.CallStatus? initialFilterStatus = null)
        {
            InitializeComponent();
            _currentFilterStatus = initialFilterStatus; // שמירת הסטטוס הראשוני
            LoadCalls(); // טעינת הנתונים עם הסינון הראשוני
            DataContext = this;
        }
        public void ApplyFilter(BO.CallStatus? newFilterStatus)
        {
            _currentFilterStatus = newFilterStatus;
            RefreshCallList(); // טוען מחדש את הנתונים עם הסינון החדש
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
                    if (InputBox("הכנס תאריך פתיחה (yyyy-MM-dd או HH:mm:ss yyyy-MM-dd):", "סינון", out string dateStr)
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

            // אם קיים סינון חיצוני דרך ה-Constructor/ApplyFilter (כלומר _currentFilterStatus),
            // הוא יקבל עדיפות. אחרת, נשתמש בסינון מה-UI.
            if (_currentFilterStatus.HasValue)
            {
                // סינון לפי הסטטוס שהועבר מבחוץ
                CallList = s_bl.Call.GetCallList(BO.CallInListField.Status, _currentFilterStatus.Value, null).ToList();
            }
            else if (CallSelectMenus == BO.CallInListField.None) // סינון מה-UI לא נבחר
            {
                CallList = s_bl.Call.GetCallList(null, null, null).ToList();
            }
            else // סינון מה-UI נבחר
            {
                CallList = s_bl.Call.GetCallList(CallSelectMenus, FilterValue, null).ToList();
            }
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
            // Load calls from the BL and wrap in ObservableCollection
            var callList = s_bl.Call.GetCallList();
            Calls = new ObservableCollection<BO.CallInList>(callList);
            CallsListView.ItemsSource = Calls;
            RefreshCallList();
            QueryCallList();
        }

        // Method to refresh the list after adding a call
        public void RefreshCalls()
        {
            LoadCalls();
        }

        // אירוע טעינת חלון - הרשמה למשקיף
        private void Window_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Call.AddObserver(CallListObserver);

        // אירוע סגירת חלון - הסרת המשקיף
        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Call.RemoveObserver(CallListObserver);


        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGrid dataGrid && dataGrid.SelectedItem is BO.CallInList selectedCallInList)
            {
                // Get the full Call object using the CallId from the selected CallInList
                var call = s_bl.Call.GetCallDetails(selectedCallInList.CallId);
                var updateWindow = new CallWindow(call);
                updateWindow.ShowDialog();
                // Refresh the list if needed
                RefreshCallList();
            }
        }

        private void DeleteCall_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int callId)
            {
                var result = MessageBox.Show("Are you sure you want to delete this call?", "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result != MessageBoxResult.Yes)
                    return;

                try
                {
                    s_bl.Call.DeleteCall(callId);
                    MessageBox.Show("The call was deleted successfully.");
                    RefreshCallList();
                }
                catch (BO.BlCannotDeleteException ex)
                {
                    MessageBox.Show(
                        $"Unable to delete the call (Reason: The call is not open or has assignments).\n\nError: {ex.Message}\nInner error: {ex.InnerException?.Message}",
                        "Delete Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (BO.BlDoesNotExistException ex)
                {
                    MessageBox.Show(
                        $"The call does not exist or was already deleted.\n\nError: {ex.Message}\nInner error: {ex.InnerException?.Message}",
                        "Delete Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"General error while deleting the call.\n\nError: {ex.Message}\nInner error: {ex.InnerException?.Message}",
                        "Delete Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void CancelAssignment_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int callId)
            {
                int managerId = int.Parse(LoginWindow.LoggedInManagerId);

                var result = MessageBox.Show(
                    "Are you sure you want to cancel the current assignment for this call?",
                    "Cancel Assignment",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes)
                    return;

                try
                {
                    s_bl.Call.CancelCallTreatment(managerId, callId);
                    MessageBox.Show("Assignment was cancelled successfully.");
                    RefreshCallList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Unable to cancel the assignment.\n\nError: {ex.Message}\nInner error: {ex.InnerException?.Message}",
                        "Cancel Assignment Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void AddNewCall_Click(object sender, RoutedEventArgs e)
        {
            AddCallWindow _AddCallWindow = new AddCallWindow();
            _AddCallWindow.Show();
        }
    }
}