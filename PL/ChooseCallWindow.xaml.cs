

// Updated ChooseCallWindow.xaml.cs
using BO;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.IO;

namespace PL
{
    public partial class ChooseCallWindow : Window
    {
        private readonly BlApi.IBl _bl = BlApi.Factory.Get();
        private readonly int _volunteerId;
        public class Location
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }

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
            MapBrowser.ObjectForScripting = new ScriptInterop(this);

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
            string mapPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "map.html");
            MapBrowser.Navigate(new Uri(mapPath));

        }
        [System.Runtime.InteropServices.ComVisible(true)]
        public class ScriptInterop
        {
            private readonly ChooseCallWindow _window;
            public ScriptInterop(ChooseCallWindow window) => _window = window;

            public void ready()
            {
                _window.Dispatcher.Invoke(() => _window.ShowVolunteerAndCallsOnMap());
            }
        }
        public void ShowVolunteerAndCallsOnMap()
        {
            var v = GetVolunteerLocation();
            MapBrowser.InvokeScript("setVolunteerLocation", v.Latitude, v.Longitude);

            foreach (var call in OpenCallsList)
            {
                var loc = GetCallLocation(call);
                MapBrowser.InvokeScript("addCallMarker", loc.Latitude, loc.Longitude, call.Id.ToString());
                MapBrowser.InvokeScript("drawLine", v.Latitude, v.Longitude, loc.Latitude, loc.Longitude);
            }
        }

        private void ShowRoute_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCall == null) return;
            var mode = (TravelModeComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "DRIVING";
            var v = GetVolunteerLocation();
            var c = GetCallLocation(SelectedCall);
            MapBrowser.InvokeScript("showRoute", v.Latitude, v.Longitude, c.Latitude, c.Longitude, mode);
        }

        private Location GetVolunteerLocation()
        {
            if (CurrentVolunteer.Latitude.HasValue && CurrentVolunteer.Longitude.HasValue)
            {
                return new Location
                {
                    Latitude = CurrentVolunteer.Latitude.Value,
                    Longitude = CurrentVolunteer.Longitude.Value
                };
            }
            return new Location { Latitude = 0, Longitude = 0 }; // ברירת מחדל
        }

        private Location GetCallLocation(OpenCallInList call)
        {
            // נניח של-OpenCallInList יש שדות Latitude ו-Longitude (אם לא יש צורך להוסיף אותם או לאפשר דרך אחרת)
            // אחרת תחזירי ברירת מחדל
            var latitudeProp = call.GetType().GetProperty("Latitude");
            var longitudeProp = call.GetType().GetProperty("Longitude");

            if (latitudeProp != null && longitudeProp != null)
            {
                var latVal = latitudeProp.GetValue(call) as double?;
                var lngVal = longitudeProp.GetValue(call) as double?;
                if (latVal.HasValue && lngVal.HasValue)
                {
                    return new Location
                    {
                        Latitude = latVal.Value,
                        Longitude = lngVal.Value
                    };
                }
            }
            return new Location { Latitude = 0, Longitude = 0 };
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
