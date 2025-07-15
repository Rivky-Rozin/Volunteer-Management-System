namespace PL;

using BO;
using System;
using System.Collections;
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

public class StatusToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is CallStatus status)
        {
            return status switch
            {
                CallStatus.Open => Brushes.Green,
                CallStatus.InProgress => Brushes.Orange,
                CallStatus.Closed => Brushes.Gray,
                CallStatus.Expired => Brushes.Red,
                CallStatus.OpenAtRisk => Brushes.DarkRed,
                CallStatus.InProgressAtRisk => Brushes.DarkOrange,
                _ => Brushes.Black
            };
        }
        return Brushes.Black;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class CallStateToGeneralEditabilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is CallStatus status)
        {
            return status == CallStatus.Open || status == CallStatus.OpenAtRisk;
        }
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class CallStateToMaxTimeEditabilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is CallStatus status)
        {
            return status == CallStatus.InProgress || status == CallStatus.InProgressAtRisk;
        }
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}


public class CollectionToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var collection = value as ICollection;
        return collection == null || collection.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class NullToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value == null ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class InvertedNullToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value != null ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// ממיר בוליאני הפוך (לצורך הצגת סימון מתאים).
/// </summary>
public class InvertedBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return !boolValue;
        }
        return DependencyProperty.UnsetValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

   
}

public class SimulatorButtonContentConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => (value is bool b && b) ? "עצור סימולטור" : "הפעל סימולטור";

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
public class InverseBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is bool b ? !b : value;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => value is bool b ? !b : value;
}