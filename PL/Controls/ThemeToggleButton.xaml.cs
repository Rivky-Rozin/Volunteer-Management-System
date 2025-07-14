// Controls/ThemeToggleButton.xaml.cs
using System.Windows;
using System.Windows.Controls;

namespace PL.Controls
{
    public partial class ThemeToggleButton : UserControl
    {
        public ThemeToggleButton()
        {
            InitializeComponent();
        }

        private void ChangeTheme_Click(object sender, RoutedEventArgs e)
        {
            App.ChangeTheme();
        }
    }
}