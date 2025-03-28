using System;
using Avalonia.Data.Converters;
using Avalonia.Media;
using System.Globalization;

namespace HospitalApp.Converters
{
    public class AvailabilityToColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int isAvailable)
            {
                return isAvailable == 1 ? Brushes.Green : Brushes.Red;
            }
            return Brushes.Gray;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
