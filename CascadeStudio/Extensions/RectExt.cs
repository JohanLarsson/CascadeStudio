namespace CascadeStudio
{
    using OpenCvSharp;

    public static class RectExt
    {
        public static Point Midpoint(this Rect rect) => MidPoint(rect.TopLeft, rect.BottomRight);

        private static Point MidPoint(Point p1, Point p2) => new Point((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
    }
}