namespace OpenCVSharpTrainer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class InfoFile
    {
        public static IReadOnlyList<LineInfo> Parse(string text)
        {
            return text.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries)
                       .Select(LineInfo.Parse)
                       .ToArray();
        }
    }
}