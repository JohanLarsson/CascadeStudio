namespace CascadeStudio
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public class SourceAndRectToCroppedBitmapConverter : IMultiValueConverter
    {
        public static readonly SourceAndRectToCroppedBitmapConverter Default = new SourceAndRectToCroppedBitmapConverter();

        private static readonly ImageSourceConverter Converter = new ImageSourceConverter();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] == DependencyProperty.UnsetValue ||
                values[1] == DependencyProperty.UnsetValue ||
                values[0] == null ||
                values[1] == null)
            {
                return null;
            }

            if (values[0] is string text)
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    return null;
                }

                return new CroppedBitmap((BitmapSource)Converter.ConvertFrom(text), (Int32Rect)values[1]);
            }

            return new CroppedBitmap((BitmapSource)values[0], (Int32Rect)values[1]);
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
