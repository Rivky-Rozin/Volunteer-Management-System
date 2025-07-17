using System;
using System.Windows;
using BO; // ודא שזה קיים ומפנה למודל העסקי שלך
using BlApi; // ודא שזה קיים ומפנה לממשק ה-BL שלך

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for UpdatePasswordWindow.xaml
    /// </summary>
    public partial class UpdatePasswordWindow : Window
    {
        private readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private readonly int _volunteerId; // שיניתי ל-int אם זה המזהה של המתנדב

        // הגדרת Dependency Properties עבור הסיסמאות
        // Defining Dependency Properties for passwords
        public static readonly DependencyProperty CurrentPasswordProperty =
            DependencyProperty.Register(nameof(CurrentPassword), typeof(string), typeof(UpdatePasswordWindow), new PropertyMetadata(string.Empty));

        public string CurrentPassword
        {
            get => (string)GetValue(CurrentPasswordProperty);
            set => SetValue(CurrentPasswordProperty, value);
        }

        public static readonly DependencyProperty NewPasswordProperty =
            DependencyProperty.Register(nameof(NewPassword), typeof(string), typeof(UpdatePasswordWindow), new PropertyMetadata(string.Empty));

        public string NewPassword
        {
            get => (string)GetValue(NewPasswordProperty);
            set => SetValue(NewPasswordProperty, value);
        }

        public static readonly DependencyProperty ConfirmPasswordProperty =
            DependencyProperty.Register(nameof(ConfirmPassword), typeof(string), typeof(UpdatePasswordWindow), new PropertyMetadata(string.Empty));

        public string ConfirmPassword
        {
            get => (string)GetValue(ConfirmPasswordProperty);
            set => SetValue(ConfirmPasswordProperty, value);
        }

        /// <summary>
        /// בנאי החלון לעדכון סיסמה.
        /// Constructor for the password update window.
        /// </summary>
        /// <param name="volunteerId">מזהה המתנדב שעבורו יש לעדכן סיסמה.</param>
        public UpdatePasswordWindow(int volunteerId) // שיניתי את סוג הפרמטר ל-int
        {
            InitializeComponent();
            _volunteerId = volunteerId;
            this.DataContext = this; // הגדרת DataContext ל-Self
        }

        /// <summary>
        /// מטפל בלחיצה על כפתור "עדכן סיסמה".
        /// Handles the "Update Password" button click.
        /// </summary>
        private void UpdatePassword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // בדיקות ולידציה בסיסיות
                // Basic validation checks
                if (string.IsNullOrWhiteSpace(CurrentPassword) ||
                    string.IsNullOrWhiteSpace(NewPassword) ||
                    string.IsNullOrWhiteSpace(ConfirmPassword))
                {
                    MessageBox.Show("יש למלא את כל השדות.", "שגיאת קלט", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (NewPassword != ConfirmPassword)
                {
                    MessageBox.Show("הסיסמה החדשה ואישור הסיסמה אינם תואמים.", "שגיאת אימות", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // טעינת פרטי המתנדב מה-BL
                // Fetching volunteer details from BL
                BO.Volunteer volunteerToUpdate = s_bl.Volunteer.GetVolunteerDetails(_volunteerId.ToString());

                // אימות סיסמה נוכחית (השוואה לסיסמה הקיימת במודל העסקי)
                // Current password validation (comparing with existing password in business model)
                if (volunteerToUpdate.Password != CurrentPassword)
                {
                    MessageBox.Show("הסיסמה הנוכחית שהוזנה שגויה.", "שגיאת אימות", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // עדכון הסיסמה במודל המתנדב
                // Updating the password in the volunteer model
                volunteerToUpdate.Password = NewPassword;

                // קריאה למתודת עדכון המתנדף ב-BL
                // Calling the update volunteer method in BL
                s_bl.Volunteer.UpdateVolunteer(_volunteerId.ToString(), volunteerToUpdate);

                MessageBox.Show("הסיסמה עודכנה בהצלחה ✅", "הצלחה", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"אירעה שגיאה בעידכון סיסמא: {ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
