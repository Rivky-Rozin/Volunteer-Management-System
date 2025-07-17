using BO;
using MyApp;
using PL.Volunteer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace PL.Call
{
    public partial class CallInListWindow : Window
    {
        private readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private BO.CallStatus? _currentFilterStatus;

        // מאפיין הרשימה, מגובה על ידי DependencyProperty
        public IEnumerable<CallInList> CallList
        {
            get { return (IEnumerable<CallInList>)GetValue(CallListProperty); }
            set { SetValue(CallListProperty, value); }
        }

        public static readonly DependencyProperty CallListProperty =
            DependencyProperty.Register("CallList", typeof(IEnumerable<CallInList>), typeof(CallInListWindow), new PropertyMetadata(new List<CallInList>()));


        public CallInListField CallSelectMenus { get; set; } = BO.CallInListField.None;
        public object? FilterValue { get; set; }

        public CallInListWindow(BO.CallStatus? initialFilterStatus = null)
        {
            InitializeComponent();
            _currentFilterStatus = initialFilterStatus;
            // אין צורך ב-DataContext = this כי ה-Binding ב-XAML הוא כבר ל-RelativeSource=Self
            LoadCalls();
        }

        public void ApplyFilter(BO.CallStatus? newFilterStatus)
        {
            _currentFilterStatus = newFilterStatus;
            RefreshCallList();
        }

        private void RefreshCallList(bool getNewFilterFromUser = false)
        {
            if (getNewFilterFromUser)
            {
                object? newFilterValue = null;
                if (_currentFilterStatus == null && CallSelectMenus != CallInListField.None)
                {
                    switch (CallSelectMenus)
                    {
                        case CallInListField.Id:
                        case CallInListField.CallId:
                        case CallInListField.NumberOfAssignments:
                            if (InputBox("הכנס מספר לסינון:", "סינון", out string idStr) && int.TryParse(idStr, out int idVal))
                                newFilterValue = idVal;
                            break;
                        case CallInListField.CallType:
                            if (InputBox("הכנס סוג קריאה (Technical, Food, Medical, Emergency, Other, None):", "סינון", out string typeStr)
                                && Enum.TryParse(typeof(CallType), typeStr, true, out object? typeVal))
                                newFilterValue = typeVal;
                            break;
                        case CallInListField.OpenTime:
                            if (InputBox("הכנס תאריך פתיחה (yyyy-MM-dd או HH:mm:ss yyyy-MM-dd):", "סינון", out string dateStr)
                                && DateTime.TryParse(dateStr, out DateTime dateVal))
                                newFilterValue = dateVal;
                            break;
                        case CallInListField.TimeUntilAssigning:
                            if (InputBox("הכנס משך זמן עד שיבוץ (hh:mm:ss):", "סינון", out string tsStr)
                                && TimeSpan.TryParse(tsStr, out TimeSpan tsVal))
                                newFilterValue = tsVal;
                            break;
                        case CallInListField.LastVolunteerName:
                            if (InputBox("הכנס שם מתנדב לסינון:", "סינון", out string nameStr))
                                newFilterValue = nameStr;
                            break;
                        case CallInListField.totalTreatmentTime:
                            if (InputBox("הכנס משך טיפול כולל (hh:mm:ss):", "סינון", out string tttStr)
                                && TimeSpan.TryParse(tttStr, out TimeSpan tttVal))
                                newFilterValue = tttVal;
                            break;
                        case CallInListField.Status:
                            if (InputBox("הכנס סטטוס (Open, InProgress, Closed, Expired, OpenAtRisk, InProgressAtRisk):", "סינון", out string statusStr)
                                && Enum.TryParse(typeof(CallStatus), statusStr, true, out object? statusVal))
                                newFilterValue = statusVal;
                            break;
                    }
                }
                FilterValue = newFilterValue;
            }

            IEnumerable<CallInList> newListFromBL;
            try
            {
                if (_currentFilterStatus.HasValue)
                    newListFromBL = s_bl.Call.GetCallList(CallInListField.Status, _currentFilterStatus.Value, null);
                else if (CallSelectMenus == CallInListField.None || FilterValue == null)
                    newListFromBL = s_bl.Call.GetCallList(null, null, null);
                else
                    newListFromBL = s_bl.Call.GetCallList(CallSelectMenus, FilterValue, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"שגיאה בטעינת קריאות: {ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
                newListFromBL = Enumerable.Empty<CallInList>();
            }

            CallList = newListFromBL.ToList();
        }

        private void CallComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is CallInListField selectedField)
                CallSelectMenus = selectedField;

            _currentFilterStatus = null;
            RefreshCallList(getNewFilterFromUser: true);
        }

        private static bool InputBox(string prompt, string title, out string value)
        {
            value = Microsoft.VisualBasic.Interaction.InputBox(prompt, title, "");
            return !string.IsNullOrWhiteSpace(value);
        }

        private volatile DispatcherOperation? _observerOperation = null;
        private void CallListObserver()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
            {
                _observerOperation = Dispatcher.BeginInvoke(() => RefreshCallList());
            }
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
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting call: {ex.Message}", "Delete Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    MessageBox.Show($"Error cancelling assignment: {ex.Message}", "Cancel Assignment Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void AddNewCall_Click(object sender, RoutedEventArgs e)
        {
            AddCallWindow _AddCallWindow = new();
            _AddCallWindow.Show();
        }

        private void ChangeTheme_Click(object sender, RoutedEventArgs e)
        {
            App.ChangeTheme();
        }
    }
}