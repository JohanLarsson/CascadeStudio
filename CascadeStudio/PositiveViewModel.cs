namespace CascadeStudio
{
    using System.Linq;
    using Gu.Reactive;

    public class PositiveViewModel : ImageViewModel
    {
        public PositiveViewModel(string fileName)
            : base(fileName)
        {
            this.FileName = fileName;
        }

        public ObservableBatchCollection<RectangleViewModel> Rectangles { get; } = new ObservableBatchCollection<RectangleViewModel>();

        public int Width => this.Rectangles.LastOrDefault()?.Info.Width ?? 64;

        public int Height => this.Rectangles.LastOrDefault()?.Info.Height ?? 64;
    }
}
