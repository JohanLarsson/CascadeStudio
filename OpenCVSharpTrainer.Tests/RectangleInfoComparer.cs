namespace OpenCVSharpTrainer.Tests
{
    using System.Collections;
    using System.Collections.Generic;

    public sealed class RectangleInfoComparer : Comparer<RectangleInfo>
    {
        public static readonly IComparer Default = new RectangleInfoComparer();

        public override int Compare(RectangleInfo x, RectangleInfo y)
        {
            if (x.X == y.X && x.Y == y.Y && x.Width == y.Width && x.Height == y.Height)
            {
                return 0;
            }

            return 1;
        }
    }
}
