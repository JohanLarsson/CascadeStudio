namespace CascadeStudio
{
    using System.Globalization;
    using System.Text;
    using OpenCvSharp;

    public static class RectExt
    {
        public static Point Midpoint(this Rect rect) => MidPoint(rect.TopLeft, rect.BottomRight);

        private static Point MidPoint(Point p1, Point p2) => new Point((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
    }

    public static class StringBuilderExt
    {
        public static StringBuilder AppendIfNotNull(this StringBuilder builder, double? value, string format)
        {
            if (value != null)
            {
                builder.AppendFormat(CultureInfo.InvariantCulture, format, value.Value);
            }

            return builder;
        }

        public static StringBuilder AppendIfNotNull(this StringBuilder builder, int? value, string format)
        {
            if (value != null)
            {
                builder.AppendFormat(CultureInfo.InvariantCulture, format, value.Value);
            }

            return builder;
        }
    }
}