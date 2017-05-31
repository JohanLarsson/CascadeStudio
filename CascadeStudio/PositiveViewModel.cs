namespace CascadeStudio
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Gu.Reactive;

    public class PositiveViewModel : ImageViewModel
    {
        private int width;
        private int height;

        public PositiveViewModel(string fileName, IReadOnlyList<RectangleInfo> rectangles)
            : base(fileName)
        {
            this.FileName = fileName;
            this.Rectangles.AddRange(rectangles);
            var rectangle = rectangles.FirstOrDefault();
            if (rectangle != null)
            {
                this.width = rectangle.Width;
                this.height = rectangle.Width;
            }
            else
            {
                this.width = 64;
                this.height = 64;
            }
        }

        public ObservableBatchCollection<RectangleInfo> Rectangles { get; } = new ObservableBatchCollection<RectangleInfo>();

        public int Width
        {
            get => this.width;

            set
            {
                if (value == this.width)
                {
                    return;
                }

                this.width = value;
                this.OnPropertyChanged();
            }
        }

        public int Height
        {
            get => this.height;

            set
            {
                if (value == this.height)
                {
                    return;
                }

                this.height = value;
                this.OnPropertyChanged();
            }
        }
    }
}
