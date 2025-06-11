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

        public object? FilterValue { get; set; }

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
            object? filterValue = null;

            switch (VolunteerSelectMenus)
            {
                case VolunteerInListEnum.Id:
                case VolunteerInListEnum.HandledCallsCount:
                case VolunteerInListEnum.CancelledCallsCount:
                case VolunteerInListEnum.ExpiredHandledCallsCount:
                case VolunteerInListEnum.CallInProgressId:
                    if (InputBox("הכנס מספר לסינון:", "סינון", out string numStr) && int.TryParse(numStr, out int numVal))
                        filterValue = numVal;
                    break;

                case VolunteerInListEnum.Name:
                    if (InputBox("הכנס שם או חלק ממנו:", "סינון", out string nameStr))
                        filterValue = nameStr;
                    break;

                case VolunteerInListEnum.IsActive:
                    if (InputBox("הכנס זמינות (true / false):", "סינון", out string boolStr) && bool.TryParse(boolStr, out bool boolVal))
                        filterValue = boolVal;
                    break;

                case VolunteerInListEnum.CallInProgressType:
                    if (InputBox("הכנס סוג שיחה (Medical, Fire, Police...):", "סינון", out string enumStr)
                        && Enum.TryParse(typeof(CallType), enumStr, true, out var enumVal))
                        filterValue = enumVal;
                    break;

                case VolunteerInListEnum.None:
                default:
                    break;
            }

            FilterValue = filterValue;

            var volunteers = s_bl.Volunteer.GetVolunteersList().ToList();

            if (VolunteerSelectMenus == VolunteerInListEnum.None || FilterValue == null)
                VolunteerList = volunteers;
            else
                VolunteerList = volunteers.Where(v => FilterVolunteers(v)).ToList();
        }


        private bool FilterVolunteers(VolunteerInList volunteer)
        {
            switch (VolunteerSelectMenus)
            {
                case VolunteerInListEnum.Id:
                    return FilterValue is int id && volunteer.Id == id;

                case VolunteerInListEnum.Name:
                    return FilterValue is string name && volunteer.Name.Contains(name, StringComparison.OrdinalIgnoreCase);

                case VolunteerInListEnum.IsActive:
                    return FilterValue is bool isActive && volunteer.IsActive == isActive;

                case VolunteerInListEnum.HandledCallsCount:
                    return FilterValue is int hcc && volunteer.HandledCallsCount == hcc;

                case VolunteerInListEnum.CancelledCallsCount:
                    return FilterValue is int ccc && volunteer.CancelledCallsCount == ccc;

                case VolunteerInListEnum.ExpiredHandledCallsCount:
                    return FilterValue is int ehcc && volunteer.ExpiredHandledCallsCount == ehcc;

                case VolunteerInListEnum.CallInProgressId:
                    return FilterValue is int callId && volunteer.CallInProgressId == callId;

                case VolunteerInListEnum.CallInProgressType:
                    return FilterValue is CallType callType && volunteer.CallInProgressType == callType;

                case VolunteerInListEnum.None:
                default:
                    return true;
            }
        }


        private void VolunteerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshVolunteerList();
        }


        /// <summary>
        /// Loads the initial list of volunteers (calls RefreshVolunteerList).
        /// </summary>
        private void LoadVolunteers()
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
        //private bool FilterVolunteers(VolunteerInList volunteer)
        //{
        //    return VolunteerSelectMenus switch
        //    {
        //        VolunteerInListEnum.Id => volunteer.Id != 0,
        //        VolunteerInListEnum.Name => !string.IsNullOrEmpty(volunteer.Name),
        //        VolunteerInListEnum.IsActive => volunteer.IsActive,
        //        VolunteerInListEnum.HandledCallsCount => volunteer.HandledCallsCount > 0,
        //        VolunteerInListEnum.CancelledCallsCount => volunteer.CancelledCallsCount > 0,
        //        VolunteerInListEnum.ExpiredHandledCallsCount => volunteer.ExpiredHandledCallsCount > 0,
        //        VolunteerInListEnum.CallInProgressId => volunteer.CallInProgressId.HasValue,
        //        VolunteerInListEnum.CallInProgressType => volunteer.CallInProgressType != CallType.None,
        //        VolunteerInListEnum.None => true,
        //        _ => true,
        //    };
        //}

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
    

    private static bool InputBox(string prompt, string title, out string value)
        {
            value = Microsoft.VisualBasic.Interaction.InputBox(prompt, title, "");
            return !string.IsNullOrWhiteSpace(value);
        }
    }
}
