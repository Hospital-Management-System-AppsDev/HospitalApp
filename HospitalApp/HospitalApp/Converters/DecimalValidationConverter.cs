using System;
using System.Globalization;
using Avalonia.Data.Converters;
using System.Text.RegularExpressions;

namespace HospitalApp.Converters{
public class DecimalValidationConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is decimal decimalValue)
        {
            return decimalValue.ToString("0.00");
        }
        return "0.00";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string stringValue = value as string;
        
        if (string.IsNullOrEmpty(stringValue))
            return 0m;
            
        // Only allow valid decimal format
        if (Regex.IsMatch(stringValue, @"^[0-9]*\.?[0-9]*$") && decimal.TryParse(stringValue, out decimal result))
        {
            return result;
        }
        
        // Return previous valid value
        return parameter is decimal prev ? prev : 0m;
    }
}
}