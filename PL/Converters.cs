namespace PL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

/// <summary>
/// קונברטר שמחזיר true אם ButtonText הוא "Update"
/// </summary>
public class UpdateToTrueConverter : IValueConverter
{
    /// <summary>
    /// ממיר את הערך למצב בוליאני: true אם הערך הוא מחרוזת "Update"
    /// </summary>
    /// <param name="value">הערך לבדיקה (צפוי להיות מחרוזת)</param>
    /// <param name="targetType">לא בשימוש</param>
    /// <param name="parameter">לא בשימוש</param>
    /// <param name="culture">תרבות המרה</param>
    /// <returns>מחזיר true אם value הוא "Update", אחרת false</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is string s && s == "Update";

    /// <summary>
    /// פעולה לא נתמכת להמרה חזרה
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

/// <summary>
/// קונברטר שמחזיר Visible אם ButtonText הוא "Update", אחרת Collapsed
/// </summary>
public class UpdateToVisibleConverter : IValueConverter
{
    /// <summary>
    /// ממיר את המחרוזת למצב תצוגה: Visible אם "Update", אחרת Collapsed
    /// </summary>
    /// <param name="value">הערך לבדיקה (צפוי להיות מחרוזת)</param>
    /// <param name="targetType">לא בשימוש</param>
    /// <param name="parameter">לא בשימוש</param>
    /// <param name="culture">תרבות המרה</param>
    /// <returns>מחזיר Visibility.Visible אם value הוא "Update", אחרת Visibility.Collapsed</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is string s && s == "Update" ? Visibility.Visible : Visibility.Collapsed;

    /// <summary>
    /// פעולה לא נתמכת להמרה חזרה
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
