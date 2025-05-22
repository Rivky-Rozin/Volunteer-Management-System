using BlApi;
using PL.Volunteer;
using PL.Call;
using System;
using System.Configuration;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Input;

namespace PL
{
    /// <summary>
    /// חלון ראשי של האפליקציה, המכיל לוגיקה לניהול הזמן, עדכון הגדרות, וניהול מסכים נוספים.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// מופע BL (Business Logic) עבור גישה לפונקציונליות הליבה של האפליקציה.
        /// </summary>
        static readonly IBl s_bl = Factory.Get();

        #region כפתורים של השעון

        /// <summary>
        /// מוסיף דקה אחת לשעון המערכת.
        /// </summary>
        private void btnAddOneMinute_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceTime(BO.TimeUnit.Minute);
        }

        /// <summary>
        /// מוסיף שעה אחת לשעון המערכת.
        /// </summary>
        private void btnAddOneHour_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceTime(BO.TimeUnit.Hour);
        }

        /// <summary>
        /// מוסיף יום אחד לשעון המערכת.
        /// </summary>
        private void btnAddOneDay_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceTime(BO.TimeUnit.Day);
        }

        /// <summary>
        /// מוסיף שנה אחת לשעון המערכת.
        /// </summary>
        private void btnAddOneYear_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceTime(BO.TimeUnit.Year);
        }

        #endregion

        /// <summary>
        /// מאפיין כללי לדוגמה (לא בשימוש כאן).
        /// </summary>
        public int MyProperty { get; set; }

        /// <summary>
        /// מעדכן את משך זמן הסיכון במערכת, לפי הערך שהוזן.
        /// </summary>
        private void btnUpdateRiskTimeSpan_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.SetRiskTimeSpan(TimeSpan.FromMinutes(RiskTimeSpan));
            MessageBox.Show("הערך עודכן בהצלחה ✅", "עדכון", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// מעדכן את משך זמן הטיפול במערכת, לפי הערך שהוזן.
        /// </summary>
        private void btnUpdateTreatmentTime_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.SetTreatmentTime(TimeSpan.FromMinutes(RiskTimeSpan));
            MessageBox.Show("הערך עודכן בהצלחה ✅", "עדכון", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// פותח את חלון רשימת המתנדבים.
        /// </summary>
        private void BtnVolunteers_Click(object sender, RoutedEventArgs e)
        {
            new VolunteerListWindow().Show();
        }

        /// <summary>
        /// פותח את חלון רשימת הקריאות.
        /// </summary>
        private void BtnCalls_Click(object sender, RoutedEventArgs e)
        {
            new CallInListWindow().Show();
        }

        /// <summary>
        /// מאתחל את מסד הנתונים לאחר אישור המשתמש.
        /// סוגר את כל החלונות האחרים למעט החלון הראשי.
        /// </summary>
        private void BtnInitializeDb_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to initialize the database?", "Confirm Initialization", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
                return;

            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                foreach (Window win in Application.Current.Windows)
                {
                    if (win != this)
                        win.Close();
                }
                s_bl.Admin.InitializeDatabase();
                MessageBox.Show("Database initialized successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        /// <summary>
        /// מאפס את מסד הנתונים לאחר אישור המשתמש.
        /// סוגר את כל החלונות האחרים למעט החלון הראשי.
        /// </summary>
        private void BtnResetDb_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to reset the database?", "Confirm Reset", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes)
                return;

            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                foreach (Window win in Application.Current.Windows)
                {
                    if (win != this)
                        win.Close();
                }
                s_bl.Admin.ResetDatabase();
                MessageBox.Show("Database reset successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        /// <summary>
        /// בנאי החלון הראשי.
        /// אתחול ערך RiskTimeSpan לפי ערך מהמערכת.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            RiskTimeSpan = (int)s_bl.Admin.GetRiskTimeSpan().TotalMinutes;
        }

        /// <summary>
        /// טיפול באירוע טעינת החלון.
        /// אתחול הזמן הנוכחי, פרמטרי סיכון והוספת צופים (Observers).
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CurrentTime = s_bl.Admin.GetCurrentTime();
            RiskTimeSpan = (int)s_bl.Admin.GetRiskTimeSpan().TotalMinutes;
            s_bl.Admin.AddClockObserver(ClockObserver);
            s_bl.Admin.AddConfigObserver(ConfigObserver);
        }

        /// <summary>
        /// טיפול באירוע סגירת החלון.
        /// הסרת הצופים (Observers).
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Admin.RemoveClockObserver(ClockObserver);
            s_bl.Admin.RemoveConfigObserver(ConfigObserver);
        }

        /// <summary>
        /// צופה לשינויי זמן.
        /// מעדכן את CurrentTime.
        /// </summary>
        private void ClockObserver()
        {
            CurrentTime = s_bl.Admin.GetCurrentTime();
        }

        /// <summary>
        /// צופה לשינויי תצורה.
        /// מעדכן את RiskTimeSpan ויכול לעדכן פרמטרים נוספים.
        /// </summary>
        private void ConfigObserver()
        {
            Dispatcher.Invoke(() =>
            {
                RiskTimeSpan = (int)s_bl.Admin.GetRiskTimeSpan().TotalMinutes;

                // כאן ניתן להוסיף עדכוני משתנים נוספים במידת הצורך
                // לדוגמה:
                // MinVolunteerAge = s_bl.Admin.GetMinVolunteerAge();
            });
        }

        /// <summary>
        /// תכונת תלות לשמירת זמן הסיכון במערכת (בדקות).
        /// </summary>
        public int RiskTimeSpan
        {
            get { return (int)GetValue(RiskTimeSpanProperty); }
            set { SetValue(RiskTimeSpanProperty, value); }
        }

        /// <summary>
        /// תכונת תלות של RiskTimeSpan.
        /// </summary>
        public static readonly DependencyProperty RiskTimeSpanProperty =
            DependencyProperty.Register("RiskTimeSpan", typeof(int), typeof(MainWindow), new PropertyMetadata(default(int)));

        /// <summary>
        /// תכונת תלות לשמירת הזמן הנוכחי במערכת.
        /// </summary>
        public DateTime CurrentTime
        {
            get { return (DateTime)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }

        /// <summary>
        /// תכונת תלות של CurrentTime.
        /// </summary>
        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow), new PropertyMetadata(s_bl.Admin.GetCurrentTime()));
    }
}
