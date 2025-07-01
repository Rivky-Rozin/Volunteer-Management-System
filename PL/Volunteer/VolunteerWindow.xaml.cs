using System;
using System.Windows;
using BlApi;
using BO;
using System.ComponentModel;
using System.Windows.Controls;

namespace PL
{
    public partial class VolunteerWindow : Window
    {
        private readonly BlApi.IBl _bl = BlApi.Factory.Get();
        private Action observer;
        private string volunteerId;

        public VolunteerWindow(string volunteerId)
        {
            InitializeComponent();
            DataContext = this;
            this.volunteerId = volunteerId;

            // Initialize dependency properties and load data
            Volunteer = _bl.Volunteer.GetVolunteerDetails(volunteerId);

            // Define enum resources for ComboBox
            Resources["DistanceKindEnum"] = Enum.GetValues(typeof(DistanceKind));

            // Initialize the observer
            observer = () =>
            {
                // Refresh the volunteer details
                Volunteer = _bl.Volunteer.GetVolunteerDetails(volunteerId);
            };

            _bl.Volunteer.AddObserver(int.Parse(volunteerId), observer);

            // Set visibility of call details based on CallInProgress
            SetCallDetailsVisibility();
            EnableSelectCallButton();
        }

        #region Dependency Properties

        // Dependency property for the volunteer
        public BO.Volunteer Volunteer
        {
            get { return (BO.Volunteer)GetValue(VolunteerProperty); }
            set { SetValue(VolunteerProperty, value); }
        }

        public static readonly DependencyProperty VolunteerProperty =
            DependencyProperty.Register("Volunteer", typeof(BO.Volunteer), typeof(VolunteerWindow));

        // Dependency property for call details visibility
        public Visibility CallDetailsVisibility
        {
            get { return (Visibility)GetValue(CallDetailsVisibilityProperty); }
            set { SetValue(CallDetailsVisibilityProperty, value); }
        }

        public static readonly DependencyProperty CallDetailsVisibilityProperty =
            DependencyProperty.Register("CallDetailsVisibility", typeof(Visibility), typeof(VolunteerWindow), new PropertyMetadata(Visibility.Collapsed));

        // Dependency property to enable/disable Select Call button
        public bool IsSelectCallButtonEnabled
        {
            get { return (bool)GetValue(IsSelectCallButtonEnabledProperty); }
            set { SetValue(IsSelectCallButtonEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsSelectCallButtonEnabledProperty =
            DependencyProperty.Register("IsSelectCallButtonEnabled", typeof(bool), typeof(VolunteerWindow), new PropertyMetadata(true));

        private void SetCallDetailsVisibility()
        {
            CallDetailsVisibility = (Volunteer?.CallInProgress != null) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void EnableSelectCallButton()
        {
            IsSelectCallButtonEnabled = Volunteer?.CallInProgress == null;
        }

        private void RefreshVolunteerData()
        {
            try
            {
                Volunteer = _bl.Volunteer.GetVolunteerDetails(volunteerId);
                SetCallDetailsVisibility();
                EnableSelectCallButton();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while refreshing data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Event Handlers

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _bl.Volunteer.UpdateVolunteer(volunteerId, Volunteer);
                MessageBox.Show("Volunteer updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CompleteCallButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Volunteer.CallInProgress != null)
                {
                    _bl.Call.CompleteCallTreatment(Volunteer.Id, Volunteer.CallInProgress.Id);
                    MessageBox.Show("Call treatment completed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    RefreshVolunteerData(); // Refresh to reflect the change
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelCallButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Volunteer.CallInProgress != null)
                {
                    _bl.Call.CancelCallTreatment(Volunteer.Id, Volunteer.CallInProgress.Id);
                    MessageBox.Show("Call treatment cancelled successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    RefreshVolunteerData(); // Refresh to reflect the change
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetInactive_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Volunteer.CallInProgress != null)
                {
                    MessageBox.Show("Cannot set volunteer to inactive while they have a call in progress.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Volunteer.IsActive = false;
                _bl.Volunteer.UpdateVolunteer(volunteerId, Volunteer);
                MessageBox.Show("Volunteer set to inactive successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void HistoryCallButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Open Call History", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SelectCall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ChooseCallWindow window = new ChooseCallWindow(int.Parse(volunteerId));
                window.Show();
                MessageBox.Show("Open Select Call", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // No need to load data here as it's done in the constructor
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            _bl.Volunteer.RemoveObserver(int.Parse(volunteerId), observer);
        }

        #endregion

        private void Window_GotFocus(object sender, RoutedEventArgs e)
        {
           var v= Volunteer.CallInProgress;
        }
    }
}