namespace CascadeStudio
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Gu.Reactive;

    public class PositiveViewModel : INotifyPropertyChanged
    {
        private int width;
        private int height;

        public PositiveViewModel(string imageFileName, IReadOnlyList<RectangleInfo> rectangles, int width, int height)
        {
            this.width = width;
            this.height = height;
            this.ImageFileName = imageFileName;
            this.Rectangles.AddRange(rectangles);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string ImageFileName { get; }

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
