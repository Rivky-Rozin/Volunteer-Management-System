using BlApi;
using BO;
using PL.Call;
using PL.Volunteer;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace PL
{
    public partial class MainWindow : Window
    {
        static readonly IBl s_bl = Factory.Get();

        #region Dependency Properties
        public DateTime CurrentTime
        {
            get { return (DateTime)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }
        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow), new PropertyMetadata(default(DateTime)));

        public int RiskTimeSpan
        {
            get { return (int)GetValue(RiskTimeSpanProperty); }
            set { SetValue(RiskTimeSpanProperty, value); }
        }
        public static readonly DependencyProperty RiskTimeSpanProperty =
            DependencyProperty.Register("RiskTimeSpan", typeof(int), typeof(MainWindow), new PropertyMetadata(0));

        public int Interval
        {
            get { return (int)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(int), typeof(MainWindow), new PropertyMetadata(1));

        public bool IsSimulatorRunning
        {
            get { return (bool)GetValue(IsSimulatorRunningProperty); }
            set { SetValue(IsSimulatorRunningProperty, value); }
        }
        public static readonly DependencyProperty IsSimulatorRunningProperty =
            DependencyProperty.Register("IsSimulatorRunning", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        // Call Counts
        public int OpenCallsCount
        {
            get { return (int)GetValue(OpenCallsCountProperty); }
            set { SetValue(OpenCallsCountProperty, value); }
        }
        public static readonly DependencyProperty OpenCallsCountProperty =
            DependencyProperty.Register("OpenCallsCount", typeof(int), typeof(MainWindow), new PropertyMetadata(0));

        public int InProgressCallsCount
        {
            get { return (int)GetValue(InProgressCallsCountProperty); }
            set { SetValue(InProgressCallsCountProperty, value); }
        }
        public static readonly DependencyProperty InProgressCallsCountProperty =
            DependencyProperty.Register("InProgressCallsCount", typeof(int), typeof(MainWindow), new PropertyMetadata(0));

        public int ClosedCallsCount
        {
            get { return (int)GetValue(ClosedCallsCountProperty); }
            set { SetValue(ClosedCallsCountProperty, value); }
        }
        public static readonly DependencyProperty ClosedCallsCountProperty =
            DependencyProperty.Register("ClosedCallsCount", typeof(int), typeof(MainWindow), new PropertyMetadata(0));

        public int ExpiredCallsCount
        {
            get { return (int)GetValue(ExpiredCallsCountProperty); }
            set { SetValue(ExpiredCallsCountProperty, value); }
        }
        public static readonly DependencyProperty ExpiredCallsCountProperty =
            DependencyProperty.Register("ExpiredCallsCount", typeof(int), typeof(MainWindow), new PropertyMetadata(0));

        public int OpenAtRiskCallsCount
        {
            get { return (int)GetValue(OpenAtRiskCallsCountProperty); }
            set { SetValue(OpenAtRiskCallsCountProperty, value); }
        }
        public static readonly DependencyProperty OpenAtRiskCallsCountProperty =
            DependencyProperty.Register("OpenAtRiskCallsCount", typeof(int), typeof(MainWindow), new PropertyMetadata(0));

        public int InProgressAtRiskCallsCount
        {
            get { return (int)GetValue(InProgressAtRiskCallsCountProperty); }
            set { SetValue(InProgressAtRiskCallsCountProperty, value); }
        }
        public static readonly DependencyProperty InProgressAtRiskCallsCountProperty =
            DependencyProperty.Register("InProgressAtRiskCallsCount", typeof(int), typeof(MainWindow), new PropertyMetadata(0));
        #endregion

        private VolunteerListWindow? _volunteerListWindow;
        private CallInListWindow? _callListWindow;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            InitializeState();
            RegisterObservers();
        }

        private void InitializeState()
        {
            SetValue(CurrentTimeProperty, s_bl.Admin.GetCurrentTime());
            SetValue(RiskTimeSpanProperty, (int)s_bl.Admin.GetRiskTimeSpan().TotalMinutes);
            UpdateCallStatusCounts();
        }

        private void RegisterObservers()
        {
            //s_bl.Admin.SimulationStopped += OnSimulationStoppedHandler;
            s_bl.Admin.AddClockObserver(ClockObserver);
            s_bl.Admin.AddConfigObserver(ConfigObserver);
            s_bl.Call.AddObserver(CallListObserver);
        }

        private void UnregisterObservers()
        {
            //s_bl.Admin.SimulationStopped -= OnSimulationStoppedHandler;
            s_bl.Admin.RemoveClockObserver(ClockObserver);
            s_bl.Admin.RemoveConfigObserver(ConfigObserver);
            s_bl.Call.RemoveObserver(CallListObserver);
        }

        #region Observers from BL

        private volatile DispatcherOperation? _observerOperation = null;
        private void ClockObserver()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
            {
                _observerOperation = Dispatcher.BeginInvoke(() => SetValue(CurrentTimeProperty, s_bl.Admin.GetCurrentTime()));
            }
        }

        private volatile DispatcherOperation? _observerOperation1 = null;
        private void ConfigObserver()
        {
            if (_observerOperation1 is null || _observerOperation1.Status == DispatcherOperationStatus.Completed)
            {
                _observerOperation1 = Dispatcher.BeginInvoke(() => SetValue(RiskTimeSpanProperty, (int)s_bl.Admin.GetRiskTimeSpan().TotalMinutes));
            }
        }

        private volatile DispatcherOperation? _observerOperation2  = null;
        private void CallListObserver()
        {
            if (_observerOperation2 is null || _observerOperation2.Status == DispatcherOperationStatus.Completed)
            {
                _observerOperation2 = Dispatcher.BeginInvoke(UpdateCallStatusCounts);
            }
        }

        private volatile DispatcherOperation? _observerOperation3 = null;

        private void OnSimulationStoppedHandler(object? sender, EventArgs e)
        {
            if (_observerOperation3 is null || _observerOperation3.Status == DispatcherOperationStatus.Completed)
            {
                _observerOperation3 = Dispatcher.BeginInvoke(() => 
                {
                    IsSimulatorRunning = false;
                    MessageBox.Show("הסימולציה הופסקה.", "הודעת מערכת", MessageBoxButton.OK, MessageBoxImage.Information);
                });
            }
        }
        #endregion

        #region UI Event Handlers
        private void btnAddOneMinute_Click(object sender, RoutedEventArgs e) => s_bl.Admin.AdvanceTime(BO.TimeUnit.Minute);
        private void btnAddOneHour_Click(object sender, RoutedEventArgs e) => s_bl.Admin.AdvanceTime(BO.TimeUnit.Hour);
        private void btnAddOneDay_Click(object sender, RoutedEventArgs e) => s_bl.Admin.AdvanceTime(BO.TimeUnit.Day);
        private void btnAddOneMonth_Click(object sender, RoutedEventArgs e) => s_bl.Admin.AdvanceTime(BO.TimeUnit.Month);
        private void btnAddOneYear_Click(object sender, RoutedEventArgs e) => s_bl.Admin.AdvanceTime(BO.TimeUnit.Year);

        private void SimulatorButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsSimulatorRunning)
            {
                s_bl.Admin.StartSimulator(Interval);
                IsSimulatorRunning = true;
            }
            else
            {
                s_bl.Admin.StopSimulator();
                IsSimulatorRunning = false;
            }
        }

        private void btnUpdateRiskTimeSpan_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.SetRiskTimeSpan(TimeSpan.FromMinutes(RiskTimeSpan));
            MessageBox.Show("הערך עודכן בהצלחה ✅", "עדכון", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnUpdateTreatmentTime_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.SetTreatmentTime(TimeSpan.FromMinutes(RiskTimeSpan));
            MessageBox.Show("הערך עודכן בהצלחה ✅", "עדכון", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnVolunteers_Click(object sender, RoutedEventArgs e)
        {
            if (_volunteerListWindow == null || !_volunteerListWindow.IsLoaded)
            {
                _volunteerListWindow = new VolunteerListWindow();
                _volunteerListWindow.Closed += (s, _) => _volunteerListWindow = null;
                _volunteerListWindow.Show();
            }
            else
            {
                _volunteerListWindow.Activate();
            }
        }

        private void BtnCalls_Click(object sender, RoutedEventArgs e) => OpenCallListWindow(null);

        private void BtnInitializeDb_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to initialize the database?", "Confirm Initialization", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                s_bl.Admin.InitializeDatabase();
                MessageBox.Show("Database initialized successfully.");
            }
        }

        private void BtnResetDb_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to reset the database?", "Confirm Reset", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                s_bl.Admin.ResetDatabase();
                MessageBox.Show("Database reset successfully.");
            }
        }

        private void BtnOpenCalls_Click(object sender, RoutedEventArgs e) => OpenCallListWindow(BO.CallStatus.Open);
        private void BtnInProgressCalls_Click(object sender, RoutedEventArgs e) => OpenCallListWindow(BO.CallStatus.InProgress);
        private void BtnClosedCalls_Click(object sender, RoutedEventArgs e) => OpenCallListWindow(BO.CallStatus.Closed);
        private void BtnExpiredCalls_Click(object sender, RoutedEventArgs e) => OpenCallListWindow(BO.CallStatus.Expired);
        private void BtnOpenAtRiskCalls_Click(object sender, RoutedEventArgs e) => OpenCallListWindow(BO.CallStatus.OpenAtRisk);
        private void BtnInProgressAtRiskCalls_Click(object sender, RoutedEventArgs e) => OpenCallListWindow(BO.CallStatus.InProgressAtRisk);
        #endregion

        #region Helper Methods
        private void UpdateCallStatusCounts()
        {
            int[] statusCounts = s_bl.Call.GetCallStatusCounts();
            SetValue(OpenCallsCountProperty, statusCounts[(int)BO.CallStatus.Open]);
            SetValue(InProgressCallsCountProperty, statusCounts[(int)BO.CallStatus.InProgress]);
            SetValue(ClosedCallsCountProperty, statusCounts[(int)BO.CallStatus.Closed]);
            SetValue(ExpiredCallsCountProperty, statusCounts[(int)BO.CallStatus.Expired]);
            SetValue(OpenAtRiskCallsCountProperty, statusCounts[(int)BO.CallStatus.OpenAtRisk]);
            SetValue(InProgressAtRiskCallsCountProperty, statusCounts[(int)BO.CallStatus.InProgressAtRisk]);
        }

        private void OpenCallListWindow(BO.CallStatus? status)
        {
            if (_callListWindow == null || !_callListWindow.IsLoaded)
            {
                _callListWindow = new CallInListWindow(status);
                _callListWindow.Closed += (s, _) => _callListWindow = null;
                _callListWindow.Show();
            }
            else
            {
                _callListWindow.Activate();
                _callListWindow.ApplyFilter(status);
            }
        }
        #endregion

        protected override void OnClosing(CancelEventArgs e)
        {
            if (IsSimulatorRunning)
            {
                s_bl.Admin.StopSimulator();
            }
            UnregisterObservers();
            base.OnClosing(e);
        }
    }
}