using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace HospitalApp.Converters
{
    public class CommaToLineBreakConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string text)
            {
                // Convert comma-separated string to newline-separated
                return text.Replace(", ", Environment.NewLine);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string text)
            {
                // Convert newline-separated back to comma-separated
                return text.Replace(Environment.NewLine, ", ");
            }
            return value;
        }
    }
}