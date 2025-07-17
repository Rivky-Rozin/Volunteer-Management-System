//using PL;
//using PL.Volunteer;
//using System.ComponentModel;
//using System.Runtime.CompilerServices;
//using System.Windows;

//namespace MyApp
//{
//    public partial class LoginWindow : Window, INotifyPropertyChanged
//    {
//        private readonly BlApi.IBl _bl;
//        // Static variable to track if the manager is logged in
//        // רק מנהל אחד יכול להיות מחובר בכל רגע נתון
//        private static bool _managerLoggedIn = false;
//        public static string? LoggedInManagerId { get; private set; }


//        public LoginWindow()
//        {
//            InitializeComponent();
//            DataContext = this;

//            //למה לא לאתחל מחוץ לפונקציה?
//            _bl = BlApi.Factory.Get();

//#if DEBUG
//Id= "361587797"; // ת"ז של המנהל    
//password = "1234"; // סיסמה של המנהל    
//#endif
//        }

//        private string id;
//        public string Id
//        {
//            get => id;
//            set { id = value; OnPropertyChanged(); }
//        }

//        private string password;
//        public string Password
//        {
//            get => password;
//            set { password = value; OnPropertyChanged(); }
//        }

//        private void LoginButton_Click(object sender, RoutedEventArgs e)
//        {
//            try
//            {
//                BO.VolunteerRole UserRole = _bl.Volunteer.Login(Id, Password);
//                if (UserRole == BO.VolunteerRole.Manager)
//                {

//                    var choice = MessageBox.Show("Enter as Manager? (yes/no)",
//                                 "Choose Mode",
//                                 MessageBoxButton.YesNo,
//                                 MessageBoxImage.Question);

//                    if (choice == MessageBoxResult.Yes)
//                    {

//                        // אם מנהל כבר מחובר, הצג הודעת שגיאה
//                        if (_managerLoggedIn)
//                        {
//                            MessageBox.Show("A manager is already logged in. Please log out first.", "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
//                            return;
//                        }

//                        var managerWindow = new MainWindow();
//                        _managerLoggedIn = true;
//                        LoggedInManagerId = Id; // שמור את ת"ז המנהל
//                        managerWindow.Closed += (_, _) =>
//                        {
//                            _managerLoggedIn = false;
//                            LoggedInManagerId = null;
//                        };
//                        managerWindow.Show();
//                    }
//                    else if (choice == MessageBoxResult.No)// אם המנהל רוצה להיות מתנדב רגיל
//                    {
//                        // יוצרים את חלון המתנדב ומעבירים לו את אובייקט הלוגיקה ואת ת"ז המתנדב
//                        VolunteerWindow volunteerWindow = new VolunteerWindow(Id); volunteerWindow.Show();

//                    }

//                }

//                else 
//                {

//                    VolunteerWindow volunteerWindow = new VolunteerWindow(Id);

//                    volunteerWindow.Show();

//                }

//            }
//            catch (Exception ex)
//            {

//                MessageBox.Show($"Login failed. Please check your credentials. \n{ex}", "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);

//            }
//        }

//        public event PropertyChangedEventHandler PropertyChanged;
//        private void OnPropertyChanged([CallerMemberName] string name = null) =>
//            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
//    }
//}
using PL;
using PL.Volunteer;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input; //  <-- הוספנו את זה כדי לזהות לחיצות מקשים

namespace MyApp
{
    public partial class LoginWindow : Window, INotifyPropertyChanged
    {
        private readonly BlApi.IBl _bl;
        // Static variable to track if the manager is logged in
        // רק מנהל אחד יכול להיות מחובר בכל רגע נתון
        private static bool _managerLoggedIn = false;
        public static string? LoggedInManagerId { get; private set; }


        public LoginWindow()
        {
            InitializeComponent();
            DataContext = this;

            // הרשמה לאירוע לחיצת מקש בחלון
            this.KeyDown += Window_KeyDown; // <-- הוספנו את השורה הזאת

            //למה לא לאתחל מחוץ לפונקציה?
            _bl = BlApi.Factory.Get();

#if DEBUG
            Id = "200000000"; // ת"ז של המנהל    
            password = "pw200000000"; // סיסמה של המנהל    
#endif
        }

        private string id;
        public string Id
        {
            get => id;
            set { id = value; OnPropertyChanged(); }
        }

        private string password;
        public string Password
        {
            get => password;
            set { password = value; OnPropertyChanged(); }
        }

        // <-- הוספנו את כל הפונקציה הזאת
        /// <summary>
        /// Handles the KeyDown event for the window to trigger login on Enter key press.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An object that contains the event data.</param>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // אם המקש שנלחץ הוא Enter
            if (e.Key == Key.Enter)
            {
                // קורא לאותה הפונקציה שלחיצת כפתור ההתחברות קוראת לה
                LoginButton_Click(sender, e);
            }
        }
        // סוף התוספת -->


        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BO.VolunteerRole UserRole = _bl.Volunteer.Login(Id, Password);
                if (UserRole == BO.VolunteerRole.Manager)
                {

                    var choice = MessageBox.Show("Enter as Manager? (yes/no)",
                                 "Choose Mode",
                                 MessageBoxButton.YesNo,
                                 MessageBoxImage.Question);

                    if (choice == MessageBoxResult.Yes)
                    {

                        // אם מנהל כבר מחובר, הצג הודעת שגיאה
                        if (_managerLoggedIn)
                        {
                            MessageBox.Show("A manager is already logged in. Please log out first.", "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        var managerWindow = new MainWindow();
                        _managerLoggedIn = true;
                        LoggedInManagerId = Id; // שמור את ת"ז המנהל
                        managerWindow.Closed += (_, _) =>
                        {
                            _managerLoggedIn = false;
                            LoggedInManagerId = null;
                        };
                        managerWindow.Show();
                    }
                    else if (choice == MessageBoxResult.No)// אם המנהל רוצה להיות מתנדב רגיל
                    {
                        // יוצרים את חלון המתנדב ומעבירים לו את אובייקט הלוגיקה ואת ת"ז המתנדב
                        VolunteerWindow volunteerWindow = new VolunteerWindow(Id); volunteerWindow.Show();

                    }

                }

                else
                {

                    VolunteerWindow volunteerWindow = new VolunteerWindow(Id);

                    volunteerWindow.Show();

                }

            }
            catch (Exception ex)
            {

                MessageBox.Show($"Login failed. Please check your credentials. \n{ex}", "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
