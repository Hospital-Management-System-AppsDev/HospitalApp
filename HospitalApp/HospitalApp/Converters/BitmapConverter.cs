using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Globalization;
using System.IO;

namespace HospitalApp.Converters
{
    public class BitmapConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string path)
            {
                try
                {
                    path = path.Trim().TrimEnd('}');

                    if (!path.StartsWith("avares://"))
                        path = $"avares://HospitalApp/{path.TrimStart('/')}";

                    return new Bitmap(AssetLoader.Open(new Uri(path)));
                }
                catch
                {
                    return null;
                }
            }


            // If value is a byte array
            if (value is byte[] imageData && imageData.Length > 0)
            {
                try
                {
                    using var ms = new MemoryStream(imageData);
                    return new Bitmap(ms);
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
