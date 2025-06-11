using System;
// ...existing code...
// אין צורך בשום שינוי בקוד מאחורי הקלעים (code-behind) כי לא הייתה לוגיקה שמציגה או משנה את הסטטוס במסך ההוספה.
// ...existing code...
using System.Windows;
using BO;

namespace PL.Call
{
    public partial class AddCallWindow : Window
    {
        private readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        // תכונת תלות עבור הקריאה להוספה
        public BO.Call CurrentCall
        {
            get => (BO.Call)GetValue(CurrentCallProperty);
            set => SetValue(CurrentCallProperty, value);
        }
        public static readonly DependencyProperty CurrentCallProperty =
            DependencyProperty.Register(nameof(CurrentCall), typeof(BO.Call), typeof(AddCallWindow), new PropertyMetadata(null));

        public AddCallWindow()
        {
            InitializeComponent();
            // יצירת אובייקט חדש עם ערכי ברירת מחדל
            CurrentCall = new BO.Call
            {
                CreationTime = DateTime.Now,
                Status = CallStatus.Open, // ערך ברירת מחדל בלבד, לא ניתן לעריכה מה-UI
                CallType = CallType.Other,
                Address = string.Empty,
                Description = string.Empty,
                Latitude = 0,
                Longitude = 0,
                MaxFinishTime = null,
                Assignments = null
            };
            DataContext = this;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            // Format validation
            if (string.IsNullOrWhiteSpace(CurrentCall.Address))
            {
                MessageBox.Show("Address is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(CurrentCall.Description))
            {
                MessageBox.Show("Description is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validate Latitude and Longitude
            if (CurrentCall.Latitude < -90 || CurrentCall.Latitude > 90)
            {
                MessageBox.Show("Latitude must be between -90 and 90.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (CurrentCall.Longitude < -180 || CurrentCall.Longitude > 180)
            {
                MessageBox.Show("Longitude must be between -180 and 180.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validate MaxFinishTime (if entered)
            if (CurrentCall.MaxFinishTime != null)
            {
                if (CurrentCall.MaxFinishTime <= CurrentCall.CreationTime)
                {
                    MessageBox.Show("Max Finish Time must be after the creation time.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            try
            {
                s_bl.Call.AddCall(CurrentCall);
                MessageBox.Show("Call added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Refresh the call list window if it's open
                foreach (Window window in Application.Current.Windows)
                {
                    if (window is CallInListWindow callInListWindow)
                    {
                        callInListWindow.RefreshCalls();
                    }
                }

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding call: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
