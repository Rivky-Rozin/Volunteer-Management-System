using System;
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
                Status = CallStatus.Open, // נניח שזה ערך ברירת מחדל
                CallType = CallType.Other, // גם נניח ברירת מחדל
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
            try
            {
                s_bl.Call.AddCall(CurrentCall);
                MessageBox.Show("Call added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding call: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
