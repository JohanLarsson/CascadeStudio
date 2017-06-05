namespace CascadeStudio.Tests.IO
{
    using NUnit.Framework;

    public class LineInfoTests
    {
        [Test]
        public void ParseWithOneRectangle()
        {
            var line = LineInfo.Parse("Bar.bmp 1 2 3 4 5");
            Assert.AreEqual("Bar.bmp", line.ImageFileName);
            var expected = new[]
            {
                new RectangleInfo { X = 2, Y = 3, Width = 4, Height = 5 },
            };

            CollectionAssert.AreEqual(expected, line.Rectangles, RectangleInfoComparer.Default);
        }

        [Test]
        public void ParseWithTwoRectangles()
        {
            var line = LineInfo.Parse("Bar.bmp 2 3 4 5 6 7 8 9 10");
            Assert.AreEqual("Bar.bmp", line.ImageFileName);
            var expected = new[]
                           {
                               new RectangleInfo { X = 3, Y = 4, Width = 5, Height = 6 },
                               new RectangleInfo { X = 7, Y = 8, Width = 9, Height = 10 },
                           };

            CollectionAssert.AreEqual(expected, line.Rectangles, RectangleInfoComparer.Default);
        }
    }
}
