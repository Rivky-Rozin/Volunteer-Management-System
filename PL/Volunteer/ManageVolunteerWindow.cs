using System;
using System.Collections.ObjectModel;
using System.Windows;
using BO;

namespace PL.Volunteer
{
    public partial class ManageVolunteerWindow : Window
    {
        private readonly bool isAddMode;
        private readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        //void AddObserver(int id, Action observer);
        //void RemoveObserver(int id, Action observer);

        // תכונת תלות עבור המתנדב לעריכה/הוספה
        public BO.Volunteer CurrentVolunteer
        {
            get => (BO.Volunteer)GetValue(CurrentVolunteerProperty);
            set => SetValue(CurrentVolunteerProperty, value);
        }
        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register(nameof(CurrentVolunteer), typeof(BO.Volunteer), typeof(ManageVolunteerWindow), new PropertyMetadata(null));
        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ButtonText == "Add")
                {
                    s_bl.Volunteer.AddVolunteer(CurrentVolunteer);
                    MessageBox.Show("Volunteer added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else // Update
                {
                    s_bl.Volunteer.UpdateVolunteer(CurrentVolunteer.Id.ToString(), CurrentVolunteer);
                    MessageBox.Show("Volunteer updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                //DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public ManageVolunteerWindow(int id = 0)
        {
            InitializeComponent();

            isAddMode = id == 0;
            ButtonText = isAddMode ? "Add" : "Update";

            if (isAddMode)
            {
                // יצירת אובייקט חדש עם ערכי ברירת מחדל
                CurrentVolunteer = new BO.Volunteer();
            }
            else
            {
                // טעינת אובייקט קיים מה-BL עם טיפול בחריגות
                try
                {
                    CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Volunteer not found: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                    return;
                }
            }

            DataContext = this;
        }

        private void RefreshVolunteerObserver()
        {
            if (CurrentVolunteer?.Id > 0)
            {
                int id = CurrentVolunteer.Id;
                // רענון האובייקט מה-BL
                CurrentVolunteer = null;
                CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id.ToString());
            }
        }


        private void Window_Loaded(EventArgs e)
        {
           
                s_bl.Volunteer.AddObserver(CurrentVolunteer.Id, RefreshVolunteerObserver);
        }

        private void Window_Closed(EventArgs e)
        {
            
                s_bl.Volunteer.RemoveObserver(CurrentVolunteer.Id, RefreshVolunteerObserver);
        }


        // תכונת תלות עבור טקסט הכפתור
        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register(nameof(ButtonText), typeof(string), typeof(ManageVolunteerWindow), new PropertyMetadata("Add"));

        public string ButtonText
        {
            get => (string)GetValue(ButtonTextProperty);
            set => SetValue(ButtonTextProperty, value);
        }

    }
}
