//using BlApi;
//using BO;
//using MyApp; // יש לוודא שה-namespace BO זמין ושה-enum CallStatus מעודכן בו
//using PL.Call;
//using PL.Volunteer;
//using System;
//using System.Configuration;
//using System.Windows;
//using System.Windows.Input;
//using System.Windows.Threading;
//using System.ComponentModel;
//namespace PL

//{
//    /// <summary>
//    /// חלון ראשי של האפליקציה, המכיל לוגיקה לניהול הזמן, עדכון הגדרות, וניהול מסכים נוספים.
//    /// </summary>
//    public partial class MainWindow : Window
//    {
//        private VolunteerListWindow? _volunteerListWindow;
//        private CallInListWindow? _callListWindow;

//        /// <summary>
//        /// מופע BL (Business Logic) עבור גישה לפונקציונליות הליבה של האפליקציה.
//        /// </summary>
//        static readonly IBl s_bl = Factory.Get();

//        #region כפתורים של השעון

//        /// <summary>
//        /// מוסיף דקה אחת לשעון המערכת.
//        /// </summary>
//        private void btnAddOneMinute_Click(object sender, RoutedEventArgs e)
//        {
//            s_bl.Admin.AdvanceTime(BO.TimeUnit.Minute);
//        }

//        /// <summary>
//        /// מוסיף שעה אחת לשעון המערכת.
//        /// </summary>
//        private void btnAddOneHour_Click(object sender, RoutedEventArgs e)
//        {
//            s_bl.Admin.AdvanceTime(BO.TimeUnit.Hour);
//        }

//        /// <summary>
//        /// מוסיף יום אחד לשעון המערכת.
//        /// </summary>
//        private void btnAddOneDay_Click(object sender, RoutedEventArgs e)
//        {
//            s_bl.Admin.AdvanceTime(BO.TimeUnit.Day);
//        }

//        /// <summary>
//        /// מוסיף שנה אחת לשעון המערכת.
//        /// </summary>
//        private void btnAddOneYear_Click(object sender, RoutedEventArgs e)
//        {
//            s_bl.Admin.AdvanceTime(BO.TimeUnit.Year);
//        }

//        /// <summary>
//        /// מוסיף חודש אחד לשעון המערכת.
//        /// </summary>
//        private void btnAddOneMonth_Click(object sender, RoutedEventArgs e)
//        {
//            s_bl.Admin.AdvanceTime(BO.TimeUnit.Month);
//        }

//        #endregion

//        /// <summary>
//        /// מאפיין כללי לדוגמה (לא בשימוש כאן).
//        /// </summary>
//        public int MyProperty { get; set; }

//        /// <summary>
//        /// מעדכן את משך זמן הסיכון במערכת, לפי הערך שהוזן.
//        /// </summary>
//        private void btnUpdateRiskTimeSpan_Click(object sender, RoutedEventArgs e)
//        {
//            s_bl.Admin.SetRiskTimeSpan(TimeSpan.FromMinutes(RiskTimeSpan));
//            MessageBox.Show("הערך עודכן בהצלחה ✅", "עדכון", MessageBoxButton.OK, MessageBoxImage.Information);
//        }

//        /// <summary>
//        /// מעדכן את משך זמן הטיפול במערכת, לפי הערך שהוזן.
//        /// </summary>
//        private void btnUpdateTreatmentTime_Click(object sender, RoutedEventArgs e)
//        {
//            s_bl.Admin.SetTreatmentTime(TimeSpan.FromMinutes(RiskTimeSpan)); // כאן יש לשים לב שאולי צריך שדה קלט נפרד לזמן טיפול
//            MessageBox.Show("הערך עודכן בהצלחה ✅", "עדכון", MessageBoxButton.OK, MessageBoxImage.Information);
//        }

//        /// <summary>
//        /// פותח את חלון רשימת המתנדבים.
//        /// </summary>
//        private void BtnVolunteers_Click(object sender, RoutedEventArgs e)
//        {
//            if (_volunteerListWindow == null || !_volunteerListWindow.IsLoaded)
//            {
//                _volunteerListWindow = new VolunteerListWindow();
//                _volunteerListWindow.Closed += (s, _) => _volunteerListWindow = null;
//                _volunteerListWindow.Show();
//            }
//            else
//            {
//                _volunteerListWindow.Activate();
//            }
//        }

//        private void BtnCalls_Click(object sender, RoutedEventArgs e)
//        {
//            // פותח את חלון רשימת הקריאות ללא סינון
//            OpenCallListWindow(null);
//        }

//        /// <summary>
//        /// מאתחל את מסד הנתונים לאחר אישור המשתמש.
//        /// סוגר את כל החלונות האחרים למעט החלון הראשי.
//        /// </summary>
//        private void BtnInitializeDb_Click(object sender, RoutedEventArgs e)
//        {
//            var result = MessageBox.Show("Are you sure you want to initialize the database?", "Confirm Initialization", MessageBoxButton.YesNo, MessageBoxImage.Question);
//            if (result != MessageBoxResult.Yes)
//                return;

//            Mouse.OverrideCursor = Cursors.Wait;
//            try
//            {
//                foreach (Window win in Application.Current.Windows)
//                {
//                    if (win != this)
//                        win.Close();
//                }
//                s_bl.Admin.InitializeDatabase();
//                MessageBox.Show("Database initialized successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
//                UpdateCallStatusCounts(); // עדכון הספירות לאחר אתחול
//            }
//            finally
//            {
//                Mouse.OverrideCursor = null;
//            }
//        }

//        /// <summary>
//        /// מאפס את מסד הנתונים לאחר אישור המשתמש.
//        /// סוגר את כל החלונות האחרים למעט החלון הראשי.
//        /// </summary>
//        private void BtnResetDb_Click(object sender, RoutedEventArgs e)
//        {
//            var result = MessageBox.Show("Are you sure you want to reset the database?", "Confirm Reset", MessageBoxButton.YesNo, MessageBoxImage.Warning);
//            if (result != MessageBoxResult.Yes)
//                return;

//            Mouse.OverrideCursor = Cursors.Wait;
//            try
//            {
//                foreach (Window win in Application.Current.Windows)
//                {
//                    if (win != this)
//                        win.Close();
//                }
//                s_bl.Admin.ResetDatabase();
//                MessageBox.Show("Database reset successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
//                UpdateCallStatusCounts(); // עדכון הספירות לאחר איפוס
//            }
//            finally
//            {
//                Mouse.OverrideCursor = null;
//            }
//        }

//        /// <summary>
//        /// בנאי החלון הראשי.
//        /// אתחול ערך RiskTimeSpan לפי ערך מהמערכת.
//        /// </summary>
//        public MainWindow()
//        {
//            InitializeComponent();
//            RiskTimeSpan = (int)s_bl.Admin.GetRiskTimeSpan().TotalMinutes;
//            UpdateCallStatusCounts(); // אתחול ספירת הקריאות עם עליית החלון
//            DataContext = this;
//        }

//        /// <summary>
//        /// טיפול באירוע טעינת החלון.
//        /// אתחול הזמן הנוכחי, פרמטרי סיכון והוספת צופים (Observers).
//        /// </summary>
//        private void Window_Loaded(object sender, RoutedEventArgs e)
//        {
//            CurrentTime1 = s_bl.Admin.GetCurrentTime();
//            RiskTimeSpan = (int)s_bl.Admin.GetRiskTimeSpan().TotalMinutes;
//            s_bl.Admin.AddClockObserver(ClockObserver);
//            s_bl.Admin.AddConfigObserver(ConfigObserver);
//            s_bl.Call.AddObserver(CallListObserver); // הוספת צופה לשינויים ברשימת הקריאות
//        }

//        /// <summary>
//        /// טיפול באירוע סגירת החלון.
//        /// הסרת הצופים (Observers).
//        /// </summary>
//        private void Window_Closed(object sender, EventArgs e)
//        {
//            s_bl.Admin.RemoveClockObserver(ClockObserver);
//            s_bl.Admin.RemoveConfigObserver(ConfigObserver);
//            s_bl.Call.RemoveObserver(CallListObserver); // הסרת הצופה
//        }

//        /// <summary>
//        /// צופה לשינויי זמן.
//        /// מעדכן את CurrentTime.
//        /// </summary>
//        /// 
//        private volatile DispatcherOperation? _observerOperation2 = null; //stage 7
//        private void ClockObserver()
//        {

//            if (_observerOperation2 is null || _observerOperation2.Status == DispatcherOperationStatus.Completed)
//                _observerOperation2 = Dispatcher.BeginInvoke(() =>
//                {

//                    CurrentTime1 = s_bl.Admin.GetCurrentTime();
//                }
//                );
//        }
//        /// <summary>
//        /// צופה לשינויי תצורה.
//        /// מעדכן את RiskTimeSpan ויכול לעדכן פרמטרים נוספים.
//        /// </summary>
//        /// 
//        private volatile DispatcherOperation? _observerOperation = null; //stage 7
//        private void ConfigObserver()
//        {
//            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
//                _observerOperation = Dispatcher.BeginInvoke(() =>
//                {

//                    RiskTimeSpan = (int)s_bl.Admin.GetRiskTimeSpan().TotalMinutes;
//                    // כאן ניתן להוסיף עדכוני משתנים נוספים במידת הצורך
//                });
//        }

//        /// <summary>
//        /// צופה לשינויים ברשימת הקריאות (לדוגמה, הוספה/מחיקה/עדכון קריאה).
//        /// מעדכן את ספירות הסטטוסים.
//        /// </summary>
//        private volatile DispatcherOperation? _observerOperation1 = null; //stage 7
//        private void CallListObserver()
//        {

//            if (_observerOperation1 is null || _observerOperation1.Status == DispatcherOperationStatus.Completed)
//                _observerOperation1 = Dispatcher.BeginInvoke(() =>
//                {

//                    UpdateCallStatusCounts();
//                });
//        }

//        /// <summary>
//        /// מעדכן את ספירות הקריאות לפי סטטוסים.
//        /// </summary>
//        private void UpdateCallStatusCounts()
//        {
//            int[] statusCounts = s_bl.Call.GetCallStatusCounts(); // קבלת מערך הספירות

//            // יש למפות את המערך statusCounts לסטטוסים המתאימים לפי סדר ההגדרה ב-Enum BO.CallStatus.
//            // public enum CallStatus { Open, InProgress, Closed, Expired, OpenAtRisk, InProgressAtRisk }
//            OpenCallsCount = statusCounts[(int)BO.CallStatus.Open];
//            InProgressCallsCount = statusCounts[(int)BO.CallStatus.InProgress];
//            ClosedCallsCount = statusCounts[(int)BO.CallStatus.Closed];
//            ExpiredCallsCount = statusCounts[(int)BO.CallStatus.Expired];
//            OpenAtRiskCallsCount = statusCounts[(int)BO.CallStatus.OpenAtRisk];
//            InProgressAtRiskCallsCount = statusCounts[(int)BO.CallStatus.InProgressAtRisk];
//        }

//        /// <summary>
//        /// פותח את חלון רשימת הקריאות עם סינון לפי סטטוס.
//        /// </summary>
//        /// <param name="status">סטטוס הקריאות לסינון, null אם אין סינון.</param>
//        private void OpenCallListWindow(BO.CallStatus? status)
//        {
//            if (_callListWindow == null || !_callListWindow.IsLoaded)
//            {
//                // נניח ש-CallInListWindow יכול לקבל פרמטר לסינון.
//                _callListWindow = new CallInListWindow(status);
//                _callListWindow.Closed += (s, _) => _callListWindow = null;
//                _callListWindow.Show();
//            }
//            else
//            {
//                // אם החלון כבר פתוח, נפעיל אותו ונעדכן את הסינון שלו.
//                _callListWindow.Activate();
//                _callListWindow.ApplyFilter(status); // יש להוסיף מתודה כזו ל-CallInListWindow
//            }
//        }

//        // --- מטפלי אירועים עבור כפתורי הסטטוסים (צריך להוסיף אותם ב-XAML) ---

//        private void BtnOpenCalls_Click(object sender, RoutedEventArgs e)
//        {
//            OpenCallListWindow(BO.CallStatus.Open);
//        }

//        private void BtnInProgressCalls_Click(object sender, RoutedEventArgs e)
//        {
//            OpenCallListWindow(BO.CallStatus.InProgress);
//        }

//        private void BtnClosedCalls_Click(object sender, RoutedEventArgs e)
//        {
//            OpenCallListWindow(BO.CallStatus.Closed);
//        }

//        private void BtnExpiredCalls_Click(object sender, RoutedEventArgs e)
//        {
//            OpenCallListWindow(BO.CallStatus.Expired);
//        }

//        private void BtnOpenAtRiskCalls_Click(object sender, RoutedEventArgs e)
//        {
//            OpenCallListWindow(BO.CallStatus.OpenAtRisk);
//        }

//        private void BtnInProgressAtRiskCalls_Click(object sender, RoutedEventArgs e)
//        {
//            OpenCallListWindow(BO.CallStatus.InProgressAtRisk);
//        }

//        /// <summary>
//        /// תכונת תלות לשמירת זמן הסיכון במערכת (בדקות).
//        /// </summary>
//        public int RiskTimeSpan
//        {
//            get { return (int)GetValue(RiskTimeSpanProperty); }
//            set { SetValue(RiskTimeSpanProperty, value); }
//        }

//        /// <summary>
//        /// תכונת תלות של RiskTimeSpan.
//        /// </summary>
//        public static readonly DependencyProperty RiskTimeSpanProperty =
//            DependencyProperty.Register("RiskTimeSpan", typeof(int), typeof(MainWindow), new PropertyMetadata(default(int)));

//        /// <summary>
//        /// תכונת תלות לשמירת הזמן הנוכחי במערכת.
//        /// </summary>
//        public DateTime CurrentTime1
//        {
//            get { return (DateTime)GetValue(CurrentTimeProperty); }
//            set { SetValue(CurrentTimeProperty, value); }
//        }

//        /// <summary>
//        /// תכונת תלות של CurrentTime.
//        /// </summary>
//        public static readonly DependencyProperty CurrentTimeProperty =
//            DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow), new PropertyMetadata(s_bl.Admin.GetCurrentTime()));

//        // --- Dependency Properties חדשים עבור ספירות הסטטוסים, מותאמים ל-enum החדש ---

//        public int OpenCallsCount
//        {
//            get { return (int)GetValue(OpenCallsCountProperty); }
//            set { SetValue(OpenCallsCountProperty, value); }
//        }

//        public static readonly DependencyProperty OpenCallsCountProperty =
//            DependencyProperty.Register("OpenCallsCount", typeof(int), typeof(MainWindow), new PropertyMetadata(0));

//        public int InProgressCallsCount
//        {
//            get { return (int)GetValue(InProgressCallsCountProperty); }
//            set { SetValue(InProgressCallsCountProperty, value); }
//        }

//        public static readonly DependencyProperty InProgressCallsCountProperty =
//            DependencyProperty.Register("InProgressCallsCount", typeof(int), typeof(MainWindow), new PropertyMetadata(0));

//        public int ClosedCallsCount
//        {
//            get { return (int)GetValue(ClosedCallsCountProperty); }
//            set { SetValue(ClosedCallsCountProperty, value); }
//        }

//        public static readonly DependencyProperty ClosedCallsCountProperty =
//            DependencyProperty.Register("ClosedCallsCount", typeof(int), typeof(MainWindow), new PropertyMetadata(0));

//        public int ExpiredCallsCount
//        {
//            get { return (int)GetValue(ExpiredCallsCountProperty); }
//            set { SetValue(ExpiredCallsCountProperty, value); }
//        }

//        public static readonly DependencyProperty ExpiredCallsCountProperty =
//            DependencyProperty.Register("ExpiredCallsCount", typeof(int), typeof(MainWindow), new PropertyMetadata(0));

//        public int OpenAtRiskCallsCount
//        {
//            get { return (int)GetValue(OpenAtRiskCallsCountProperty); }
//            set { SetValue(OpenAtRiskCallsCountProperty, value); }
//        }

//        public static readonly DependencyProperty OpenAtRiskCallsCountProperty =
//            DependencyProperty.Register("OpenAtRiskCallsCount", typeof(int), typeof(MainWindow), new PropertyMetadata(0));

//        public int InProgressAtRiskCallsCount
//        {
//            get { return (int)GetValue(InProgressAtRiskCallsCountProperty); }
//            set { SetValue(InProgressAtRiskCallsCountProperty, value); }
//        }

//        public static readonly DependencyProperty InProgressAtRiskCallsCountProperty =
//            DependencyProperty.Register("InProgressAtRiskCallsCount", typeof(int), typeof(MainWindow), new PropertyMetadata(0));
//        public static readonly DependencyProperty IntervalProperty =
//       DependencyProperty.Register(nameof(Interval), typeof(int), typeof(MainWindow), new PropertyMetadata(1));


//        public int Interval
//        {
//            get => (int)GetValue(IntervalProperty);
//            set => SetValue(IntervalProperty, value);
//        }

//        public static readonly DependencyProperty IsSimulatorRunningProperty =
//            DependencyProperty.Register(nameof(IsSimulatorRunning), typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

//        public bool IsSimulatorRunning
//        {
//            get => (bool)GetValue(IsSimulatorRunningProperty);
//            set => SetValue(IsSimulatorRunningProperty, value);
//        }

//        // דוגמה: תכונה לשעון המערכת (אם יש)
//        public DateTime SystemClock
//        {
//            get => s_bl.Admin.GetCurrentTime();
//        }

//        private void SimulatorButton_Click(object sender, RoutedEventArgs e)
//        {
//            if (!IsSimulatorRunning)
//            {
//                s_bl.Admin.StartSimulator(Interval);
//                IsSimulatorRunning = true;
//            }
//            else
//            {
//                s_bl.Admin.StopSimulator();
//                IsSimulatorRunning = false;
//            }
//        }

//        // טיפול בסגירת חלון
//        protected override void OnClosing(CancelEventArgs e)
//        {
//            if (IsSimulatorRunning)
//            {
//                s_bl.Admin.StopSimulator();
//                IsSimulatorRunning = false;
//            }
//            base.OnClosing(e);
//        }
//    }
//}
using BlApi;
using BO;
using PL.Call;
using PL.Volunteer;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

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
        private void ClockObserver()
        {
            Dispatcher.Invoke(() => SetValue(CurrentTimeProperty, s_bl.Admin.GetCurrentTime()));
        }

        private void ConfigObserver()
        {
            Dispatcher.Invoke(() => SetValue(RiskTimeSpanProperty, (int)s_bl.Admin.GetRiskTimeSpan().TotalMinutes));
        }

        private void CallListObserver()
        {
            Dispatcher.Invoke(UpdateCallStatusCounts);
        }

        private void OnSimulationStoppedHandler(object? sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                IsSimulatorRunning = false;
                MessageBox.Show("הסימולציה הופסקה.", "הודעת מערכת", MessageBoxButton.OK, MessageBoxImage.Information);
            });
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