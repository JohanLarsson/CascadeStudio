namespace OpenCVSharpTrainer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class LineInfo
    {
        public LineInfo(string imageFileName, int count, IReadOnlyList<RectangleInfo> rectangles)
        {
            this.ImageFileName = imageFileName;
            this.Count = count;
            this.Rectangles = rectangles;
        }

        public string ImageFileName { get; }

        public int Count { get; }

        public IReadOnlyList<RectangleInfo> Rectangles { get; }

        public static LineInfo Parse(string text)
        {
            var match = Regex.Match(text, @"^(?<file>.+) (?<count>\d+)(?<rect> \d+ \d+ \d+ \d+)+$", RegexOptions.Singleline | RegexOptions.RightToLeft);
            if (!match.Success)
            {
                throw new FormatException($"Could not parse line from {text}");
            }

            return new LineInfo(
                match.Groups["file"].Value,
                int.Parse(match.Groups["count"].Value),
                match.Groups["rect"].Captures
                                    .OfType<Capture>()
                                    .Select(c => RectangleInfo.Parse(c.Value))
                                    .Reverse()
                                    .ToArray());
        }
    }
}