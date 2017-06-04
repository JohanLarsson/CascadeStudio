namespace CascadeStudio
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using OpenCvSharp;

    public class StringToSizeConverter : IValueConverter
    {
        public static readonly StringToSizeConverter Default = new StringToSizeConverter();
        private static readonly char[] Separators = { ',', ' ' };

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

                var parts = text.Split(Separators, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 1 &&
                    double.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double size))
                {
                    return new Size(size, size);
                }

                if (parts.Length == 2 &&
                    double.TryParse(parts[0], out double width) &&
                    double.TryParse(parts[1], out double height))
                {
                    return new Size(width, height);
                }

                return value;
            }

            return null;
        }
    }
}