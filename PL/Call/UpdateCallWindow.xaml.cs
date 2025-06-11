using System.Windows;
using BO;

namespace PL.Call
{
    public partial class UpdateCallWindow : Window
    {

        public Array CallTypeCollection { get; } = Enum.GetValues(typeof(BO.CallType));
        private readonly BlApi.IBl _bl = BlApi.Factory.Get();
        public BO.Call EditableCall { get; }

        public UpdateCallWindow(BO.Call call)
        {
            InitializeComponent();
            // יצירת עותק כדי לא לשנות את האובייקט המקורי עד שמאשרים
            EditableCall = new BO.Call
            {
                Id = call.Id,
                CallType = call.CallType,
                Description = call.Description,
                Address = call.Address,
                Latitude = call.Latitude,
                Longitude = call.Longitude,
                CreationTime = call.CreationTime,
                MaxFinishTime = call.MaxFinishTime,
                Status = call.Status
            };

            DataContext = this;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _bl.Call.UpdateCall(EditableCall);
                MessageBox.Show("הקריאה עודכנה בהצלחה");
                DialogResult = true;
                Close();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("שגיאה בעדכון קריאה: " + ex.Message);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
