using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace HospitalApp.Converters
{
    public class StringIsNotEmptyToBoolConverter : IValueConverter
    {
        public bool Inverse { get; set; } = false;

        // WARNING: This is a static cache and works only if each binding is independent.
        // For per-instance tracking, a more robust approach is needed (e.g., via ViewModel).
        private static string _cachedValue = "";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = value?.ToString();
            if (!string.IsNullOrEmpty(str))
                _cachedValue = str;

            bool result = !string.IsNullOrEmpty(str);
            return Inverse ? !result : result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
            {
                if (Inverse)
                    b = !b;

                return b ? _cachedValue : string.Empty;
            }
            return string.Empty;
        }
    }
}
