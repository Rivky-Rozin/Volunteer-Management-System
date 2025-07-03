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

        public ObservableCollection<BO.CallInList> Calls { get; set; } = new();

        public IEnumerable<CallInList> CallList
        {
            get => Calls;
        }

        public CallInListField CallSelectMenus { get; set; } = BO.CallInListField.None;
        public object? FilterValue { get; set; }

        public CallInListWindow(BO.CallStatus? initialFilterStatus = null)
        {
            InitializeComponent();
            _currentFilterStatus = initialFilterStatus;
            DataContext = this;
            LoadCalls();
        }

        public void ApplyFilter(BO.CallStatus? newFilterStatus)
        {
            _currentFilterStatus = newFilterStatus;
            RefreshCallList();
        }

        private void RefreshCallList()
        {
            object? filterValue = null;
            if (_currentFilterStatus == null && CallSelectMenus != CallInListField.None)
            {
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
                }
            }

            FilterValue = filterValue;

            IEnumerable<CallInList> newList = Enumerable.Empty<CallInList>();

            try
            {
                if (_currentFilterStatus.HasValue)
                    newList = s_bl.Call.GetCallList(CallInListField.Status, _currentFilterStatus.Value, null);
                else if (CallSelectMenus == CallInListField.None)
                    newList = s_bl.Call.GetCallList(null, null, null);
                else
                    newList = s_bl.Call.GetCallList(CallSelectMenus, FilterValue, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"שגיאה בטעינת קריאות: {ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Calls.Clear();
            foreach (var item in newList)
                Calls.Add(item);
        }

        private void CallComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is CallInListField selectedField)
                CallSelectMenus = selectedField;

            _currentFilterStatus = null; // ביטול סינון סטטוס חיצוני כשמשתמש בוחר מהתפריט
            RefreshCallList();
        }

        private static bool InputBox(string prompt, string title, out string value)
        {
            value = Microsoft.VisualBasic.Interaction.InputBox(prompt, title, "");
            return !string.IsNullOrWhiteSpace(value);
        }

        private void QueryCallList()
        {
            IEnumerable<CallInList> list = Enumerable.Empty<CallInList>();

            try
            {
                if (CallSelectMenus == CallInListField.None)
                    list = s_bl.Call.GetCallList(null, null, null);
                else
                    list = s_bl.Call.GetCallList(CallSelectMenus, FilterValue, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"שגיאה בטעינת קריאות: {ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Calls.Clear();
            foreach (var call in list)
                Calls.Add(call);
        }

        private void CallListObserver()
        {
            if (!Dispatcher.CheckAccess())
                Dispatcher.Invoke(QueryCallList);
            else
                QueryCallList();
        }

        private void LoadCalls()
        {
            RefreshCallList();
        }

        public void RefreshCalls()
        {
            RefreshCallList();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Call.AddObserver(CallListObserver);

        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Call.RemoveObserver(CallListObserver);

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGrid dataGrid && dataGrid.SelectedItem is CallInList selectedCallInList)
            {
                var call = s_bl.Call.GetCallDetails(selectedCallInList.CallId);
                var updateWindow = new CallWindow(call);
                updateWindow.ShowDialog();
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
                    MessageBox.Show($"Unable to delete the call.\n\nError: {ex.Message}\nInner: {ex.InnerException?.Message}", "Delete Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (BO.BlDoesNotExistException ex)
                {
                    MessageBox.Show($"The call does not exist.\n\nError: {ex.Message}\nInner: {ex.InnerException?.Message}", "Delete Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"General error.\n\nError: {ex.Message}\nInner: {ex.InnerException?.Message}", "Delete Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CancelAssignment_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int callId)
            {
                int managerId = int.Parse(LoginWindow.LoggedInManagerId);

                var result = MessageBox.Show("Are you sure you want to cancel the current assignment for this call?", "Cancel Assignment", MessageBoxButton.YesNo, MessageBoxImage.Question);
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
                    MessageBox.Show($"Unable to cancel the assignment.\n\nError: {ex.Message}\nInner: {ex.InnerException?.Message}", "Cancel Assignment Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void AddNewCall_Click(object sender, RoutedEventArgs e)
        {
            AddCallWindow _AddCallWindow = new();
            _AddCallWindow.Show();
        }
    }
}
