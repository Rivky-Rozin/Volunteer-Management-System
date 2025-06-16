// --- START OF FILE ChooseCallWindow.xaml.cs ---

using BO;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace PL
{
    /// <summary>
    /// Interaction logic for ChooseCallWindow.xaml
    /// A screen for a volunteer to view and select an open call for treatment.
    /// This screen is opened only if the volunteer is not currently handling a call.
    /// </summary>
    public partial class ChooseCallWindow : Window
    {
        private readonly BlApi.IBl _bl=BlApi.Factory.Get();
        private readonly int _volunteerId;

        #region Dependency Properties

        /// <summary>
        /// A collection of open calls available for the volunteer.
        /// </summary>
        public ObservableCollection<OpenCallInList> OpenCallsList
        {
            get => (ObservableCollection<OpenCallInList>)GetValue(OpenCallsListProperty);
            set => SetValue(OpenCallsListProperty, value);
        }
        public static readonly DependencyProperty OpenCallsListProperty =
            DependencyProperty.Register("OpenCallsList", typeof(ObservableCollection<OpenCallInList>), typeof(ChooseCallWindow), new PropertyMetadata(null));

        /// <summary>
        /// The call currently selected by the user in the list.
        /// </summary>
        public OpenCallInList? SelectedCall
        {
            get => (OpenCallInList)GetValue(SelectedCallProperty);
            set => SetValue(SelectedCallProperty, value);
        }
        public static readonly DependencyProperty SelectedCallProperty =
            DependencyProperty.Register("SelectedCall", typeof(OpenCallInList), typeof(ChooseCallWindow), new PropertyMetadata(null));

        /// <summary>
        /// The current volunteer viewing the screen. Used for address updates.
        /// </summary>
        public BO.Volunteer CurrentVolunteer
        {
            get { return (BO.Volunteer)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }
        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(ChooseCallWindow), new PropertyMetadata(null));

        #endregion

        /// <summary>
        /// Constructor for the Choose Call Window.
        /// </summary>
        /// <param name="bl">The business logic layer interface.</param>
        /// <param name="volunteerId">The ID of the currently logged-in volunteer.</param>
        public ChooseCallWindow( int volunteerId)
        {
            
            _volunteerId = volunteerId;
            InitializeComponent();

            // Initialize properties before loading data
            OpenCallsList = new ObservableCollection<OpenCallInList>();

            try
            {
                // Load the current volunteer's details to allow address updates
                CurrentVolunteer = _bl.Volunteer.GetVolunteerDetails(_volunteerId.ToString());
                LoadOpenCalls();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"שגיאה בטעינת נתוני המתנדב: {ex.Message}", "שגיאת טעינה", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close(); // Close the window if initial data cannot be loaded
            }
        }

        /// <summary>
        /// Loads the list of open calls from the business logic layer
        /// that are relevant to the volunteer (based on location, status, etc.).
        /// </summary>
        private void LoadOpenCalls()
        {
            try
            {
                // Provide default values for the required parameters 'callTypeFilter' and 'sortField'
                var calls = _bl.Call.GetOpenCallsForVolunteer(_volunteerId, null, null);
                // Using ObservableCollection to automatically update the ListView
                OpenCallsList = new ObservableCollection<OpenCallInList>(calls);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"אירעה שגיאה בעת טעינת רשימת הקריאות הפתוחות.\nפרטים: {ex.Message}",
                                "שגיאת תקשורת", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Handles the click event for the "Choose Call" button in each row of the list.
        /// </summary>
        private void ChooseCall_Click(object sender, RoutedEventArgs e)
        {
            // The DataContext of the button is the OpenCallInList object for its row
            if ((sender as Button)?.DataContext is OpenCallInList callToTake)
            {
                try
                {
                    _bl.Call.SelectCallForTreatment( _volunteerId, callToTake.Id);

                    MessageBox.Show($"הקריאה מספר {callToTake.Id} נבחרה בהצלחה!\nכעת ניתן לחזור למסך הראשי ולדווח על התקדמות.",
                                    "בחירה הושלמה", MessageBoxButton.OK, MessageBoxImage.Information);

                    // After successfully choosing a call, the volunteer is busy, so this screen should close.
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"לא ניתן היה לבחור בקריאה.\nסיבה: {ex.Message}",
                                    "שגיאת בחירה", MessageBoxButton.OK, MessageBoxImage.Warning);

                    // It's a good practice to refresh the list in case the call was taken by another volunteer
                    LoadOpenCalls();
                }
            }
        }

        /// <summary>
        /// Handles the click event for the "Update Address and Refresh" button.
        /// It updates the volunteer's address in the system and then reloads the call list.
        /// </summary>
        private void UpdateAddress_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // The address is already updated in the CurrentVolunteer object thanks to data binding.
                _bl.Volunteer.UpdateVolunteer(CurrentVolunteer.Id.ToString(), CurrentVolunteer);

                MessageBox.Show("הכתובת עודכנה בהצלחה. רשימת הקריאות תתעדכן כעת.",
                                "עדכון כתובת", MessageBoxButton.OK, MessageBoxImage.Information);

                // Reload the calls based on the new address.
                LoadOpenCalls();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"אירעה שגיאה בעת עדכון הכתובת.\nפרטים: {ex.Message}",
                                "שגיאת עדכון", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
// --- END OF FILE ChooseCallWindow.xaml.cs ---