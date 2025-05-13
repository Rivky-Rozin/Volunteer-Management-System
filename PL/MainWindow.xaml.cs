using BlApi;
using System;
using System.Windows;
using System.Windows.Threading;

namespace PL
{
    public partial class MainWindow : Window
    {
        private void btnAddOneMinute_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceTime(BO.TimeUnit.Minute); // Fixed method name and enum value casing
        }

        static readonly IBl s_bl = Factory.Get();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            // אפשרות: להפעיל טיימר כדי לעדכן את השעה כל שנייה
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) => CurrentTime = DateTime.Now;
            timer.Start();
        }


        // תכונת תלות - CurrentTime
        public DateTime CurrentTime
        {
            get { return (DateTime)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }

        // רישום התכונה כמאפיין תלות
        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register(
                "CurrentTime",           // שם המאפיין
                typeof(DateTime),        // טיפוס
                typeof(MainWindow),      // הבעלים של התכונה
                new PropertyMetadata(DateTime.Now)); // ערך ברירת מחדל
    }

}
