namespace OpenCVSharpTrainer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class InfoFile
    {
        public InfoFile(IReadOnlyList<LineInfo> lines)
        {
            this.Lines = lines;
            this.AllRectangles = lines.SelectMany(l => l.Rectangles)
                                      .ToArray();
            foreach (var rect in this.AllRectangles)
            {
                if (this.Width == 0)
                {
                    this.Width = rect.Width;
                }
                else if (this.Width != rect.Width)
                {
                    this.Width = -1;
                }

                if (this.Height == 0)
                {
                    this.Height = rect.Height;
                }
                else if (this.Height != rect.Height)
                {
                    this.Height = -1;
                }
            }
        }

        public IReadOnlyList<LineInfo> Lines { get; }

        public RectangleInfo[] AllRectangles { get; }

        public int Width { get; }

        public int Height { get; }

        public static InfoFile Parse(string text)
        {
            return new InfoFile(
                text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(LineInfo.Parse)
                    .ToArray());
        }

        public static InfoFile Load(string infoFileName)
        {
            return Parse(File.ReadAllText(infoFileName));
        }
    }
}