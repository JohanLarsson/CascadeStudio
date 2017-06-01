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
            this.Rectangles.AddRange(rectangles.Select(x => new RectangleViewModel(this, x)));
        }

        public ObservableBatchCollection<RectangleViewModel> Rectangles { get; } = new ObservableBatchCollection<RectangleViewModel>();

        public int Width => this.Rectangles.LastOrDefault()?.Info.Width ?? 64;

        public int Height => this.Rectangles.LastOrDefault()?.Info.Height ?? 64;
    }
}
