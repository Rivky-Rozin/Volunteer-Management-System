using BlApi;
using System;
using System.Configuration;
using System.Windows;
using System.Windows.Threading;

namespace PL
{
    public partial class MainWindow : Window
    {
        static readonly IBl s_bl = Factory.Get();

        #region כפתורים של השעון

        private void btnAddOneMinute_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceTime(BO.TimeUnit.Minute); // Fixed method name and enum value casing
        }

        private void btnAddOneHour_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceTime(BO.TimeUnit.Hour); // Fixed method name and enum value casing
        }
        public int MyProperty { get; set; }
        private void btnAddOneDay_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceTime(BO.TimeUnit.Day); // Fixed method name and enum value casing
        }

        private void btnAddOneYear_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceTime(BO.TimeUnit.Year); // Fixed method name and enum value casing
        }

        #endregion


        private void btnUpdateRiskTimeSpan_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.SetRiskTimeSpan(TimeSpan.FromMinutes(RiskTimeSpan)); // Convert int (in minutes) back to TimeSpan
            MessageBox.Show("הערך עודכן בהצלחה ✅", "עדכון", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void btnUpdateTreatmentTime_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.SetTreatmentTime(TimeSpan.FromMinutes(RiskTimeSpan)); // Convert int (in minutes) back to TimeSpan
            MessageBox.Show("הערך עודכן בהצלחה ✅", "עדכון", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public MainWindow()
        {
            InitializeComponent();
            RiskTimeSpan = (int)s_bl.Admin.GetRiskTimeSpan().TotalMinutes; // Convert TimeSpan to int (in minutes)
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // טעינת שעון המערכת
            CurrentTime = s_bl.Admin.GetCurrentTime();
            RiskTimeSpan = (int)s_bl.Admin.GetRiskTimeSpan().TotalMinutes; // Convert TimeSpan to int (in minutes)
            s_bl.Admin.AddClockObserver(ClockObserver);
            s_bl.Admin.AddConfigObserver(ConfigObserver);
        }

        private void ClockObserver()
        {
            //Dispatcher.Invoke(() =>
            //{
                CurrentTime = s_bl.Admin.GetCurrentTime();
            //});
        }
        private void ConfigObserver()
        {
            Dispatcher.Invoke(() =>
            {
                RiskTimeSpan = (int)s_bl.Admin.GetRiskTimeSpan().TotalMinutes; // Convert TimeSpan to int (in minutes)

                // הוסיפי כאן משתנים נוספים אם יש, לדוגמה:
                // MinVolunteerAge = s_bl.Admin.GetMinVolunteerAge();
            });
        }




        public int RiskTimeSpan
        {
            get { return (int)GetValue(RiskTimeSpanProperty); }
            set { SetValue(RiskTimeSpanProperty, value); }
        }

        public static readonly DependencyProperty RiskTimeSpanProperty =
            DependencyProperty.Register("RiskTimeSpan", typeof(int), typeof(MainWindow), new PropertyMetadata(default(int)));


        // תכונת תלות - CurrentTime
        public DateTime CurrentTime
        {
            get { return (DateTime)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }
        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow), new PropertyMetadata(s_bl.Admin.GetCurrentTime()));



    }
}
