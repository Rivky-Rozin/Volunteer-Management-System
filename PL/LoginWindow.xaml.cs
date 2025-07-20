
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
        private readonly BlApi.IBl _bl = BlApi.Factory.Get();

        private static bool _managerLoggedIn = false;
        public static string? LoggedInManagerId { get; private set; }


        public LoginWindow()
        {
            InitializeComponent();
            DataContext = this;
            this.KeyDown += Window_KeyDown;

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

        
        /// <summary>
        /// Handles the KeyDown event for the window to trigger login on Enter key press.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An object that contains the event data.</param>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
        
            if (e.Key == Key.Enter)
            {
              
                LoginButton_Click(sender, e);
            }
        }
       
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Id) || string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("Please enter both ID and password.", "Missing Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                BO.VolunteerRole UserRole = _bl.Volunteer.Login(Id, Password);
                if (UserRole == BO.VolunteerRole.Manager)
                {

                    var choice = MessageBox.Show("Enter as Manager? (yes/no)", "Choose Mode", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (choice == MessageBoxResult.Yes)
                    {
                        if (_managerLoggedIn)
                        {
                            MessageBox.Show("A manager is already logged in. Please log out first.", "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        var managerWindow = new MainWindow();
                        _managerLoggedIn = true;
                        LoggedInManagerId = Id; 
                        managerWindow.Closed += (_, _) =>
                        {
                            _managerLoggedIn = false;
                            LoggedInManagerId = null;
                        };
                        managerWindow.Show();
                    }
                    else if (choice == MessageBoxResult.No)
                    {
                        VolunteerWindow volunteerWindow = new VolunteerWindow(Id);
                        volunteerWindow.Show();
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
