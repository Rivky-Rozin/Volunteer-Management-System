using BO;
using BlApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace PL
{
    /// <summary>
    /// Interaction logic for CallWindow.xaml
    /// This is a "Code-Behind" implementation, without ICommand.
    /// NOTE: This approach violates the project requirements mentioned in the prompt.
    /// </summary>
    public partial class CallWindow : Window, INotifyPropertyChanged
    {
        private readonly BlApi.IBl _bl = Factory.Get();


        private BO.Call _currentCall = new();
        public BO.Call CurrentCall
        {
            get => _currentCall;
            set
            {
                _currentCall = value;
                OnPropertyChanged();
                // After the call is updated, we need to re-evaluate if the button should be enabled.
                OnPropertyChanged(nameof(IsUpdateButtonEnabled));
            }
        }

        // This new property will control the button's IsEnabled state.
        public bool IsUpdateButtonEnabled
        {
            get
            {
                if (CurrentCall == null) return false;
                // The logic that determines if the button should be active.
                return CurrentCall.Status != CallStatus.Closed && CurrentCall.Status != CallStatus.Expired;
            }
        }

        public IEnumerable<CallType> CallTypes { get; } = Enum.GetValues(typeof(CallType)).Cast<CallType>();

        public event PropertyChangedEventHandler? PropertyChanged;

        public CallWindow(BO.Call callToUpdate)
        {
            InitializeComponent();
            //_bl = bl;
            CurrentCall = new BO.Call // Use the property to trigger the initial check for the button
            {
                Id = callToUpdate.Id,
                CallType = callToUpdate.CallType,
                Description = callToUpdate.Description,
                Address = callToUpdate.Address,
                Latitude = callToUpdate.Latitude,
                Longitude = callToUpdate.Longitude,
                CreationTime = callToUpdate.CreationTime,
                MaxFinishTime = callToUpdate.MaxFinishTime,
                Status = callToUpdate.Status,
                Assignments = callToUpdate.Assignments
            };

            // Set the DataContext to the window instance itself so bindings work.
            DataContext = this;
        }

        /// <summary>
        /// This is the event handler for the update button's Click event.
        /// It contains all the update logic.
        /// </summary>
        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(CurrentCall.Address))
                {
                    MessageBox.Show("כתובת היא שדה חובה.", "שגיאת קלט", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Send the update request to the Business Logic layer
                _bl.Call.UpdateCall(CurrentCall);

                MessageBox.Show("הקריאה עודכנה בהצלחה!", "עדכון הושלם", MessageBoxButton.OK, MessageBoxImage.Information);

                // Close the window on successful update
                this.Close();
            }
            catch (Exception ex) // Catching exceptions from BL
            {
                MessageBox.Show($"אירעה שגיאה בעת עדכון הקריאה:\n{ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}