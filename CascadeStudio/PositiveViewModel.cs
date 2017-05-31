namespace CascadeStudio
{
    using System.Collections.Generic;
    using System.Linq;
    using Gu.Reactive;

    public class PositiveViewModel : ImageViewModel
    {
        public PositiveViewModel(string fileName, IReadOnlyList<RectangleInfo> rectangles)
            : base(fileName)
        {
            this.FileName = fileName;
            this.Rectangles.AddRange(rectangles);
        }

        public ObservableBatchCollection<RectangleInfo> Rectangles { get; } = new ObservableBatchCollection<RectangleInfo>();

        public int Width => this.Rectangles.LastOrDefault()?.Width ?? 64;

        public int Height => this.Rectangles.LastOrDefault()?.Height ?? 64;
    }
}
