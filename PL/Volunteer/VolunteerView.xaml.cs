using BO;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace PL
{
    public partial class VolunteerView : Window, INotifyPropertyChanged
    {
        private BlApi.IBl _bl;

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Volunteer Properties
        private BO.Volunteer _currentVolunteer = new();
        public BO.Volunteer CurrentVolunteer
        {
            get => _currentVolunteer;
            set
            {
                _currentVolunteer = value;
                OnPropertyChanged(nameof(CurrentVolunteer));
                OnPropertyChanged(nameof(Id));
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(Address));
                OnPropertyChanged(nameof(PhoneNumber));
                OnPropertyChanged(nameof(IsActive));
                OnPropertyChanged(nameof(CurrentCall)); // עדכון הקריאה תלוי במתנדב
            }
        }

        public int Id => CurrentVolunteer.Id;

        public string Name
        {
            get => CurrentVolunteer.Name;
            set { if (CurrentVolunteer.Name != value) { CurrentVolunteer.Name = value; OnPropertyChanged(); } }
        }

        public string? Address
        {
            get => CurrentVolunteer.Address;
            set { if (CurrentVolunteer.Address != value) { CurrentVolunteer.Address = value; OnPropertyChanged(); } }
        }

        public string PhoneNumber
        {
            get => CurrentVolunteer.Phone;
            set { if (CurrentVolunteer.Phone != value) { CurrentVolunteer.Phone = value; OnPropertyChanged(); } }
        }

        public bool IsActive
        {
            get => CurrentVolunteer.IsActive;
            set
            {
                if (CurrentVolunteer.IsActive == value) return;
                CurrentVolunteer.IsActive = value;
                OnPropertyChanged();
                (UpdateVolunteerCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (ChooseCallCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }
        #endregion

        #region Call Properties
        public BO.Call? CurrentCall { get; private set; }

        public double DistanceToCall { get; private set; }
        #endregion

        #region Commands
        public ICommand UpdateVolunteerCommand { get; private set; }
        public ICommand ChooseCallCommand { get; private set; }
        public ICommand ViewHistoryCommand { get; private set; }
        public ICommand FinishCallCommand { get; private set; }
        public ICommand CancelCallAssignmentCommand { get; private set; }
        #endregion

        // *** תיקון: הבנאי מקבל int, כפי שנשלח מחלון הלוגין ***
        public VolunteerView(int volunteerId)
        {
            this.DataContext = this;
            InitializeComponent();

            // שלב 1: אתחול שכבת הלוגיקה
            _bl = BlApi.Factory.Get();

            // שלב 2: אתחול הפקודות
            UpdateVolunteerCommand = new RelayCommand(UpdateVolunteer, CanUpdateVolunteer);
            ChooseCallCommand = new RelayCommand(ChooseCall, CanChooseCall);
            ViewHistoryCommand = new RelayCommand(ViewHistory);
            FinishCallCommand = new RelayCommand(FinishCall, () => CurrentCall != null);
            CancelCallAssignmentCommand = new RelayCommand(CancelCallAssignment, () => CurrentCall != null);

            // שלב 3: טעינת הנתונים הראשוניים
            LoadVolunteerAndCallData(volunteerId);

            // *** תיקון קריטי: הגדרת ה-DataContext בסוף, אחרי שכל המידע מוכן ***

        }

        #region Data Loading Methods
        private void LoadVolunteerAndCallData(int volunteerId)
        {
            try
            {
                // Correct method name based on the provided type signatures
                CurrentVolunteer = _bl.Volunteer.GetVolunteerDetails(volunteerId.ToString());

                if (CurrentVolunteer.CallInProgress is not null)
                {
                    CurrentCall = _bl.Call.GetCallDetails(CurrentVolunteer.CallInProgress.Id);
                    // DistanceToCall = ... (if needed)
                    OnPropertyChanged(nameof(DistanceToCall));
                }
                else
                {
                    CurrentCall = null;
                }
                OnPropertyChanged(nameof(CurrentCall));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading volunteer data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }
        #endregion

        #region Command Implementations
        private bool CanUpdateVolunteer()
        {
            return !(CurrentCall != null && !IsActive);
        }

        private void UpdateVolunteer()
        {
            try
            {
                _bl.Volunteer.UpdateVolunteer(CurrentVolunteer.Id.ToString(), CurrentVolunteer);
                MessageBox.Show("Details updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Update failed: {ex.Message}", "Update Error", MessageBoxButton.OK, MessageBoxImage.Error);
                LoadVolunteerAndCallData(CurrentVolunteer.Id); // Reload original data
            }
        }
        private bool CanChooseCall()
        {
            return CurrentCall == null && IsActive;
        }

        private void ChooseCall()
        {
            MessageBox.Show("Navigating to 'Choose Call' screen...", "Navigation", MessageBoxButton.OK, MessageBoxImage.Information);
            // new ChooseCallWindow(CurrentVolunteer.Id).Show();
        }

        private void ViewHistory()
        {
            MessageBox.Show("Navigating to 'Call History' screen...", "Navigation", MessageBoxButton.OK, MessageBoxImage.Information);
            // new VolunteerHistoryWindow(CurrentVolunteer.Id).Show();
        }

        private void FinishCall()
        {
            try
            {
                _bl.Call.CompleteCallTreatment(CurrentVolunteer.Id, CurrentCall!.Id);
                MessageBox.Show("Call has been marked as finished.", "Call Finished", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadVolunteerAndCallData(CurrentVolunteer.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error finishing call: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelCallAssignment()
        {
            if (MessageBox.Show("Are you sure you want to cancel your assignment?", "Confirm Cancellation", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    // Corrected method call to use CancelCallTreatment instead of the non-existent CancelCallAssignment
                    _bl.Call.CancelCallTreatment(CurrentVolunteer.Id, CurrentCall!.Id);
                    MessageBox.Show("Assignment cancelled.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadVolunteerAndCallData(CurrentVolunteer.Id);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error cancelling assignment: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        #endregion
    }

    // המחלקה RelayCommand נשארת כפי שהיא
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute == null || _canExecute();
        public void Execute(object? parameter) => _execute();
        public void RaiseCanExecuteChanged() => CommandManager.InvalidateRequerySuggested();
    }
}

