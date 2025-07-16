using BlApi;
using BO;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace PL.Volunteer
{
    public partial class VolunteerCallHistoryWindow : Window, INotifyPropertyChanged
    {
        private static readonly IBl s_bl = Factory.Get();
        private readonly int _volunteerId;

        public event PropertyChangedEventHandler? PropertyChanged;

        private ObservableCollection<ClosedCallInList> _closedCalls;
        public ObservableCollection<ClosedCallInList> ClosedCalls
        {
            get => _closedCalls;
            set
            {
                _closedCalls = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ClosedCalls)));
            }
        }

        private CallInListField _selectedFilterField;
        public CallInListField SelectedFilterField
        {
            get => _selectedFilterField;
            set
            {
                _selectedFilterField = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedFilterField)));
                RefreshList();
            }
        }

        private string _filterValue = "";
        public string FilterValue
        {
            get => _filterValue;
            set
            {
                _filterValue = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FilterValue)));
                RefreshList();
            }
        }

        public ObservableCollection<CallInListField> FilterFields { get; }

        public VolunteerCallHistoryWindow(int volunteerId)
        {
            InitializeComponent();
            DataContext = this;
            _volunteerId = volunteerId;

            // Initialize collections
            _closedCalls = new ObservableCollection<ClosedCallInList>();
            FilterFields = new ObservableCollection<CallInListField>(
                Enum.GetValues(typeof(CallInListField))
                    .Cast<CallInListField>()
                    .Where(f => f != CallInListField.None)
            );

            // Initial load
            LoadClosedCalls();

            // Register for updates
            s_bl.Call.AddObserver(CallListObserver);
        }

        private void LoadClosedCalls()
        {
            try
            {
                // Corrected method call based on the provided ICall interface
                var calls = s_bl.Call.GetClosedCallsOfVolunteer(_volunteerId, null, null);
                ClosedCalls = new ObservableCollection<ClosedCallInList>(calls);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "אירעה שגיאה בטעינת היסטוריית הקריאות",
                    "שגיאה",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void RefreshList()
        {
            if (string.IsNullOrEmpty(FilterValue) || SelectedFilterField == CallInListField.None)
            {
                LoadClosedCalls();
                return;
            }

            try
            {
                CallType? callTypeFilter = Enum.TryParse<CallType>(FilterValue, out var parsedCallType) ? parsedCallType : null;
                ClosedCallInListEnum? sortField = Enum.TryParse<ClosedCallInListEnum>(SelectedFilterField.ToString(), out var parsedSortField) ? parsedSortField : null;

                var filtered = s_bl.Call.GetClosedCallsOfVolunteer(_volunteerId, callTypeFilter, sortField);
                ClosedCalls = new ObservableCollection<ClosedCallInList>(filtered);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "אירעה שגיאה בסינון הרשימה",
                    "שגיאה",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        private volatile DispatcherOperation? _observerOperation1 = null; //stage 7
        private void CallListObserver()
        {

            if (_observerOperation1 is null || _observerOperation1.Status == DispatcherOperationStatus.Completed)
                _observerOperation1 = Dispatcher.BeginInvoke(() =>
                {

                    RefreshList();
                });
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Call.RemoveObserver(CallListObserver);
        }
    }
}