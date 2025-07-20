using BO;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PL
{
    public partial class ChooseCallWindow : Window
    {
        private readonly BlApi.IBl _bl = BlApi.Factory.Get();
        private readonly int _volunteerId;
        public List<OpenCallField> FilterFields { get; } = Enum.GetValues(typeof(OpenCallField)).Cast<OpenCallField>().ToList();
        public OpenCallField? SelectedFilterField { get; set; }
        public string? FilterValue { get; set; }
        public List<OpenCallField> SortFields { get; } = Enum.GetValues(typeof(OpenCallField)).Cast<OpenCallField>().ToList();
        public OpenCallField? SelectedSortField { get; set; }

        public ObservableCollection<OpenCallInList> OpenCallsList
        {
            get => (ObservableCollection<OpenCallInList>)GetValue(OpenCallsListProperty);
            set => SetValue(OpenCallsListProperty, value);
        }
        public static readonly DependencyProperty OpenCallsListProperty =
            DependencyProperty.Register("OpenCallsList", typeof(ObservableCollection<OpenCallInList>), typeof(ChooseCallWindow), new PropertyMetadata(null));

        public OpenCallInList? SelectedCall
        {
            get => (OpenCallInList)GetValue(SelectedCallProperty);
            set => SetValue(SelectedCallProperty, value);
        }
        public static readonly DependencyProperty SelectedCallProperty =
            DependencyProperty.Register("SelectedCall", typeof(OpenCallInList), typeof(ChooseCallWindow), new PropertyMetadata(null));

        public BO.Volunteer CurrentVolunteer
        {
            get { return (BO.Volunteer)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }
        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(ChooseCallWindow), new PropertyMetadata(null));

        public ChooseCallWindow(int volunteerId)
        {
            _volunteerId = volunteerId;
            InitializeComponent();
            OpenCallsList = new ObservableCollection<OpenCallInList>();
            DataContext = this;

            try
            {
                CurrentVolunteer = _bl.Volunteer.GetVolunteerDetails(_volunteerId.ToString());
                LoadOpenCalls();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"שגיאה בטעינת נתוני המתנדב: {ex.Message}", "שגיאת טעינה", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
       

        }
        private void LoadOpenCalls()
        {
            try
            {
                var calls = _bl.Call.GetOpenCallsForVolunteer(_volunteerId, null, null);
                OpenCallsList = new ObservableCollection<OpenCallInList>(calls);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"אירעה שגיאה בעת טעינת רשימת הקריאות הפתוחות.\nפרטים: {ex.Message}",
                                "שגיאת תקשורת", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilterAndSort()
        {
            var filtered = _bl.Call.GetOpenCallsForVolunteer(_volunteerId, null, null).AsEnumerable();

            if (SelectedFilterField is not null && !string.IsNullOrWhiteSpace(FilterValue))
            {
                switch (SelectedFilterField)
                {
                    case OpenCallField.Id:
                        if (int.TryParse(FilterValue, out int idVal))
                            filtered = filtered.Where(c => c.Id == idVal);
                        break;
                    case OpenCallField.CallType:
                        if (Enum.TryParse(typeof(CallType), FilterValue, true, out var ct))
                            filtered = filtered.Where(c => c.CallType.Equals((CallType)ct));
                        break;
                    case OpenCallField.FullAddress:
                        filtered = filtered.Where(c => c.FullAddress?.Contains(FilterValue) == true);
                        break;
                    case OpenCallField.DistanceFromVolunteer:
                        if (double.TryParse(FilterValue, out double dist))
                            filtered = filtered.Where(c => c.DistanceFromVolunteer <= dist);
                        break;
                }
            }

            if (SelectedSortField is not null)
            {
                filtered = SelectedSortField switch
                {
                    OpenCallField.Id => filtered.OrderBy(c => c.Id),
                    OpenCallField.CallType => filtered.OrderBy(c => c.CallType),
                    OpenCallField.FullAddress => filtered.OrderBy(c => c.FullAddress),
                    OpenCallField.DistanceFromVolunteer => filtered.OrderBy(c => c.DistanceFromVolunteer),
                    _ => filtered
                };
            }

            OpenCallsList = new ObservableCollection<OpenCallInList>(filtered.ToList());
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilterAndSort();
        }

        private void ChooseCall_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is OpenCallInList callToTake)
            {
                try
                {
                    _bl.Call.SelectCallForTreatment(_volunteerId, callToTake.Id);


                    // todo לבדוק מה החלק הזה בקוד עושה
                    // Although the call was successfully assigned, other open windows for the same volunteer
                    // (like VolunteerWindow) won't update automatically because SelectCallForTreatment
                    // doesn't trigger the NotifyObservers mechanism.
                    // To refresh all observers without modifying the BL code, we reload the volunteer data
                    // and pass it to UpdateVolunteer—even if no actual change occurred.
                    // This is a workaround ("ugly hack") that triggers observers and refreshes the UI properly,
                    // without changing the interface or BL classes.
                    var updatedVolunteer = _bl.Volunteer.GetVolunteerDetails(_volunteerId.ToString());
                    _bl.Volunteer.UpdateVolunteer(_volunteerId.ToString(), updatedVolunteer);
                    MessageBox.Show($"הקריאה מספר {callToTake.Id} נבחרה בהצלחה!", "בחירה הושלמה", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"לא ניתן היה לבחור בקריאה.\nסיבה: {ex.Message}", "שגיאת בחירה", MessageBoxButton.OK, MessageBoxImage.Warning);
                    LoadOpenCalls();
                }
            }
        }

        private void UpdateAddress_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _bl.Volunteer.UpdateVolunteer(CurrentVolunteer.Id.ToString(), CurrentVolunteer);
                MessageBox.Show("הכתובת עודכנה בהצלחה.", "עדכון כתובת", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadOpenCalls();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"אירעה שגיאה בעת עדכון הכתובת.\nפרטים: {ex.Message}", "שגיאת עדכון", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
   

    }
}
