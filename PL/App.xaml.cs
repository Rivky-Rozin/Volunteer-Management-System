using System.Configuration;
using System.Data;
using System.Windows;

namespace PL
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static bool isDarkTheme = false;

        public static void ThemeSwitchButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeTheme();
        }
        public static void ChangeTheme()
        {
            // נקה את מילון המשאבים הקיים
            Current.Resources.MergedDictionaries.Clear();

            // צור מילון משאבים חדש
            ResourceDictionary resourceDict = new ResourceDictionary();

            if (isDarkTheme)
            {
                // אם כרגע כהה, שנה לבהיר
                resourceDict.Source = new Uri("Themes/LightTheme.xaml", UriKind.Relative);
                isDarkTheme = false;
            }
            else
            {
                // אם כרגע בהיר, שנה לכהה
                resourceDict.Source = new Uri("Themes/DarkTheme.xaml", UriKind.Relative);
                isDarkTheme = true;
            }

            // טען את מילון המשאבים החדש לתוך האפליקציה
            Current.Resources.MergedDictionaries.Add(resourceDict);
        }
    }
}


