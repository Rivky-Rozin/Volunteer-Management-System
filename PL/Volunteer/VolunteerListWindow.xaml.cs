namespace PL.Volunteer
{
    using BO;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;

    /// <summary>
    /// Interaction logic for VolunteerListWindow.xaml
    /// </summary>
    public partial class VolunteerListWindow : Window
    {
        /// <summary>
        /// Access to the business logic layer (BL).
        /// </summary>
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        /// <summary>
        /// Menu selection used to filter the volunteer list.
        /// </summary>
        public VolunteerInListEnum VolunteerSelectMenus { get; set; } = BO.VolunteerInListEnum.None;

        private List<VolunteerInListEnum> allVolunteers = new();

        /// <summary>
        /// Currently selected volunteer in the list (if any).
        /// </summary>
        public BO.VolunteerInList? SelectedVolunteer { get; set; }

        /// <summary>
        /// Constructor. Initializes the window and loads the volunteer list.
        /// </summary>
        public VolunteerListWindow()
        {
            InitializeComponent();
            LoadVolunteers();
        }

        /// <summary>
        /// Observer method that refreshes the volunteer list from the BL.
        /// Called automatically when the data changes in the backend.
        /// </summary>
        private void RefreshVolunteerListObserver()
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(RefreshVolunteerList);
            }
            else
            {
                RefreshVolunteerList();
            }
        }

        /// <summary>
        /// Helper method that refreshes the list of volunteers
        /// according to the selected filtering option.
        /// </summary>
        private void RefreshVolunteerList()
        {
            var volunteers = s_bl.Volunteer.GetVolunteersList().ToList();
            if (VolunteerSelectMenus == BO.VolunteerInListEnum.None)
                VolunteerList = volunteers;
            else
                VolunteerList = volunteers.Where(v => FilterVolunteers(v)).ToList();
        }

        /// <summary>
        /// Loads the initial list of volunteers (calls RefreshVolunteerList).
        /// </summary>
        private void LoadVolunteers()
        {
            RefreshVolunteerList();
        }

        /// <summary>
        /// Event handler for ComboBox selection change. Refreshes the list.
        /// </summary>
        private void VolunteerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshVolunteerList();
        }

        /// <summary>
        /// Event handler called when the window is loaded.
        /// Registers the refresh observer.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
          => s_bl.Volunteer.AddObserver(RefreshVolunteerListObserver);

        /// <summary>
        /// Event handler called when the window is closed.
        /// Unregisters the refresh observer.
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Volunteer.RemoveObserver(RefreshVolunteerListObserver);

        /// <summary>
        /// Opens a volunteer detail window when double-clicking a list item.
        /// </summary>
        private void lsvVolunteersList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedVolunteer != null)
                new ManageVolunteerWindow(SelectedVolunteer.Id).Show();
        }

        /// <summary>
        /// Opens the add volunteer window (no ID provided).
        /// </summary>
        private void btnAddVolunteer_Click(object sender, RoutedEventArgs e)
        {
            new ManageVolunteerWindow().Show();
        }

        /// <summary>
        /// Handles volunteer deletion from the list, with confirmation message.
        /// </summary>
        private void btnDeleteVolunteer_click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is BO.VolunteerInList volunteer)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete volunteer '{volunteer.Name}' (Id: {volunteer.Id})?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        s_bl.Volunteer.DeleteVolunteer(volunteer.Id.ToString());
                        // No need for manual refresh — observer mechanism will auto-update
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Cannot delete volunteer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Filters the volunteer list based on the selected menu filter.
        /// </summary>
        /// <param name="volunteer">The volunteer to test against the filter.</param>
        /// <returns>True if the volunteer matches the filter; otherwise false.</returns>
        private bool FilterVolunteers(VolunteerInList volunteer)
        {
            return VolunteerSelectMenus switch
            {
                VolunteerInListEnum.Id => volunteer.Id != 0,
                VolunteerInListEnum.Name => !string.IsNullOrEmpty(volunteer.Name),
                VolunteerInListEnum.IsActive => volunteer.IsActive,
                VolunteerInListEnum.HandledCallsCount => volunteer.HandledCallsCount > 0,
                VolunteerInListEnum.CancelledCallsCount => volunteer.CancelledCallsCount > 0,
                VolunteerInListEnum.ExpiredHandledCallsCount => volunteer.ExpiredHandledCallsCount > 0,
                VolunteerInListEnum.CallInProgressId => volunteer.CallInProgressId.HasValue,
                VolunteerInListEnum.CallInProgressType => volunteer.CallInProgressType != CallType.None,
                VolunteerInListEnum.None => true,
                _ => true,
            };
        }

        /// <summary>
        /// Gets or sets the list of volunteers displayed in the UI.
        /// Uses a dependency property for binding.
        /// </summary>
        public IEnumerable<BO.VolunteerInList> VolunteerList
        {
            get { return (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerListProperty); }
            set { SetValue(VolunteerListProperty, value); }
        }

        /// <summary>
        /// DependencyProperty registration for VolunteerList.
        /// Enables data binding in the XAML.
        /// </summary>
        public static readonly DependencyProperty VolunteerListProperty =
            DependencyProperty.Register("VolunteerList", typeof(IEnumerable<BO.VolunteerInList>), typeof(VolunteerListWindow), new PropertyMetadata(null));
    }
}
