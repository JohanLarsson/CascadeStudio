namespace CascadeStudio
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
                if (this.MinWidth == -1)
                {
                    this.MinWidth = rect.Width;
                }
                else if (rect.Width < this.MinWidth)
                {
                    this.MinWidth = rect.Width;
                }

                if (this.MaxWidth == -1)
                {
                    this.MaxWidth = rect.Width;
                }
                else if (rect.Width > this.MaxWidth)
                {
                    this.MaxWidth = rect.Width;
                }

                if (this.MinHeight == -1)
                {
                    this.MinHeight = rect.Height;
                }
                else if (rect.Height < this.MinHeight)
                {
                    this.MinHeight = rect.Height;
                }

                if (this.MaxHeight == -1)
                {
                    this.MaxHeight = rect.Height;
                }
                else if (rect.Height > this.MaxHeight)
                {
                    this.MaxHeight = rect.Height;
                }
            }
        }

        public IReadOnlyList<LineInfo> Lines { get; }

        public RectangleInfo[] AllRectangles { get; }

        public int MinWidth { get; } = -1;

        public int MaxWidth { get; } = -1;

        public int MinHeight { get; } = -1;

        public int MaxHeight { get; } = -1;

        public static InfoFile Parse(string text)
        {
            return new InfoFile(
                text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(LineInfo.Parse)
                    .ToArray());
        }

        public static InfoFile Load(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return null;
            }

            return Parse(File.ReadAllText(fileName));
        }
    }
}