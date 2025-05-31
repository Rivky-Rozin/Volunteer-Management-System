using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Windows;

namespace MyApp
{
    public partial class LoginWindow : Window, INotifyPropertyChanged
    {
        private readonly BlApi.IBl _bl;
        // Static variable to track if the manager is logged in
        // רק מנהל אחד יכול להיות מחובר בכל רגע נתון
        private static bool _managerLoggedIn = false;

        public LoginWindow()
        {
            InitializeComponent();
            DataContext = this;
            _bl = BlApi.Factory.Get();
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

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BO.VolunteerRole UserRole = _bl.Volunteer.Login(Id, Password);
                if (UserRole == BO.VolunteerRole.Manager)
                {
                    // אם מנהל כבר מחובר, הצג הודעת שגיאה
                    if (_managerLoggedIn)
                    {
                        MessageBox.Show("A manager is already logged in. Please log out first.", "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    var choice = MessageBox.Show("Enter as Manager? (yes/no)",
                                 "Choose Mode",
                                 MessageBoxButton.YesNo,
                                 MessageBoxImage.Question);

                    if (choice == MessageBoxResult.Yes)
                    {
                        //var managerWindow = new AdminMainWindow(); 
                        _managerLoggedIn = true; // עדכון הסטטוס של המנהל המחובר
                        //כשיקרה אירוע סגירה של חלון המנהל, תירא פונקציה שמעדכנת את המשתנה הסטטי לפולס
                        //managerWindow.Closed += (_, _) => _managerLoggedIn = false;
                        //managerWindow.Show();
                    }
                }
                else
                {
                    //var volunteerWindow = new VolunteerMainWindow();
                    //volunteerWindow.Show();
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
