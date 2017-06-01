namespace CascadeStudio
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using OpenCvSharp;

    public class StringToSizeConverter : IValueConverter
    {
        public static readonly StringToSizeConverter Default = new StringToSizeConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Size size)
            {
                return $"{size.Width}, {size.Height}";
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string text)
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    return null;
                }

                var parts = text.Split(',');
                return new Size(
                    double.Parse(parts[0], CultureInfo.InvariantCulture),
                    double.Parse(parts[1], CultureInfo.InvariantCulture));
            }

            return null;
        }
    }
}