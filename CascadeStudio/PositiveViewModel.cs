namespace CascadeStudio
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Gu.Reactive;

    public class PositiveViewModel : INotifyPropertyChanged
    {
        private int width;
        private int height;

        public PositiveViewModel(string fileName, IReadOnlyList<RectangleInfo> rectangles)
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

        public event PropertyChangedEventHandler PropertyChanged;

        public string FileName { get; }

        public string Name => System.IO.Path.GetFileNameWithoutExtension(this.FileName);

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

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
