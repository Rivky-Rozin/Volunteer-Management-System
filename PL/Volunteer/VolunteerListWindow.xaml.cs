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
    public VolunteerSelectMenu VolunteerSelectMenus { get; set; } = BO.VolunteerSelectMenu.None;

    private List<VolunteerInList> allVolunteers = new();


    public VolunteerListWindow()
    {
        InitializeComponent();
        LoadVolunteers();
    }

    private void LoadVolunteers()
    {
        // Load all volunteers from BL
        allVolunteers = s_bl.Volunteer.GetVolunteersList().ToList();
        VolunteerList = allVolunteers;
    }

    private void VolunteerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Fixing the issue by using the correct method and removing invalid references
        VolunteerList = (VolunteerSelectMenus == BO.VolunteerSelectMenu.None)
            ? s_bl.Volunteer.GetVolunteersList()
            : s_bl.Volunteer.GetVolunteersList().Where(v => FilterVolunteers(v)).ToList();
    }

    private bool FilterVolunteers(VolunteerInList volunteer)
    {
        // Implement filtering logic based on VolunteerSelectMenus
        return VolunteerSelectMenus switch
        {
            VolunteerSelectMenu.FullName => !string.IsNullOrEmpty(volunteer.Name),
            VolunteerSelectMenu.HandledCallsCount => volunteer.HandledCallsCount > 0,
            VolunteerSelectMenu.IsActive => volunteer.IsActive,
            VolunteerSelectMenu.CallInProgressId => volunteer.CallInProgressId.HasValue,
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
