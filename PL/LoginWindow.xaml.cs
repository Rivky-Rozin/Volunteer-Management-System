using PL;
using PL.Volunteer;
using System.ComponentModel;
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
        public static string? LoggedInManagerId { get; private set; }


        public LoginWindow()
        {
            InitializeComponent();
            DataContext = this;

            //למה לא לאתחל מחוץ לפונקציה?
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
                        VolunteerView volunteerWindow = new VolunteerView(int.Parse(Id)); volunteerWindow.Show();
                        //this.Close(); // סוגרים את חלון ההתחברות לאחר כניסה מוצלחת
                    }

                }

                else // אם המשתמש הוא מתנדב רגיל
                {
                    // יוצרים את חלון המתנדב ומעבירים לו את אובייקט הלוגיקה ואת ת"ז המתנדב
                    VolunteerView volunteerWindow = new VolunteerView(int.Parse(Id));
                    // פה הפונקציה נופלת ומפילה את התוכנה
                    volunteerWindow.Show();

                    //this.Close(); // סוגרים את חלון ההתחברות לאחר כניסה מוצלחת
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
