namespace PL.Volunteer;

using BO;
using System;
using System.Collections.Generic;
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

    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public VolunteerInListEnum VolunteerSelectMenus { get; set; } = BO.VolunteerInListEnum.None;

    private List<VolunteerInListEnum> allVolunteers = new();

    public BO.VolunteerInList? SelectedVolunteer { get; set; }


    public VolunteerListWindow()
    {
        InitializeComponent();
        LoadVolunteers();
    }

    /// <summary>
    /// מתודת השקפה - תרונן את הרשימה מה־BL
    /// BL יקרא למתודה זו כאשר הרשימה מתעדכנת בבסיס הנתונים
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
    /// מתודת עזר פרטית שמרעננת את הרשימה לפי הסינון הנוכחי
    /// </summary>
    private void RefreshVolunteerList()
    {
        var volunteers = s_bl.Volunteer.GetVolunteersList().ToList();
        if (VolunteerSelectMenus == BO.VolunteerInListEnum.None)
            VolunteerList = volunteers;
        else
            VolunteerList = volunteers.Where(v => FilterVolunteers(v)).ToList();
    }

    // עדכון קריאות פנימיות למתודת העזר
    private void LoadVolunteers()
    {
        RefreshVolunteerList();
    }

    private void VolunteerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        RefreshVolunteerList();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
      => s_bl.Volunteer.AddObserver(RefreshVolunteerListObserver);

    private void Window_Closed(object sender, EventArgs e)
        => s_bl.Volunteer.RemoveObserver(RefreshVolunteerListObserver);


    private void lsvVolunteersList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (SelectedVolunteer != null)
            new VolunteerWindow(SelectedVolunteer.Id).Show();
    }
    private void btnAddVolunteer_Click(object sender, RoutedEventArgs e)
    {
        // פותח את מסך הפריט הבודד במצב הוספה (ללא Id)
        new VolunteerWindow().Show();
    }

    private bool FilterVolunteers(VolunteerInList volunteer)
    {
        // Implement filtering logic based on VolunteerSelectMenus
        return VolunteerSelectMenus switch
        {
            VolunteerInListEnum.Name => !string.IsNullOrEmpty(volunteer.Name),
            VolunteerInListEnum.HandledCallsCount => volunteer.HandledCallsCount > 0,
            VolunteerInListEnum.IsActive => volunteer.IsActive,
            VolunteerInListEnum.CallInProgressId => volunteer.CallInProgressId.HasValue,
            _ => true,
        };
    }

    public IEnumerable<BO.VolunteerInList> VolunteerList
    {
        get { return (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerListProperty); }
        set { SetValue(VolunteerListProperty, value); }
    }

    public static readonly DependencyProperty VolunteerListProperty =
        DependencyProperty.Register("VolunteerList", typeof(IEnumerable<BO.VolunteerInList>), typeof(VolunteerListWindow), new PropertyMetadata(null));
}
