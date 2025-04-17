using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace HospitalApp.Converters
{
    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is bool b ? !b : AvaloniaProperty.UnsetValue;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is bool b ? !b : AvaloniaProperty.UnsetValue;
    }
}
