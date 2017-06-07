namespace CascadeStudio
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Runtime.CompilerServices;
    using Gu.Reactive;

    public sealed class PositivesRanges : INotifyPropertyChanged, IDisposable
    {
        private readonly IDisposable disposable;
        private int minWidth;
        private int maxWidth;
        private int minHeight;
        private int count;
        private int maxHeight;
        private bool disposed;

        private PositivesRanges()
        {
            this.disposable = ProjectViewModel.Instance
                            .PositivesTracker
                            .ObservePropertyChangedSlim(x => x.Changes)
                            .Throttle(TimeSpan.FromMilliseconds(100))
                            .Subscribe(_ => this.OnPositivesChanged());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static PositivesRanges Instance { get; } = new PositivesRanges();

        public int Count
        {
            get => this.count;

            private set
            {
                if (value == this.count)
                {
                    return;
                }

                this.count = value;
                this.OnPropertyChanged();
            }
        }

        public int MinWidth
        {
            get => this.minWidth;

            private set
            {
                if (value == this.minWidth)
                {
                    return;
                }

                this.minWidth = value;
                this.OnPropertyChanged();
            }
        }

        public int MaxWidth
        {
            get => this.maxWidth;

            private set
            {
                if (value == this.maxWidth)
                {
                    return;
                }

                this.maxWidth = value;
                this.OnPropertyChanged();
            }
        }

        public int MinHeight
        {
            get => this.minHeight;

            private set
            {
                if (value == this.minHeight)
                {
                    return;
                }

                this.minHeight = value;
                this.OnPropertyChanged();
            }
        }

        public int MaxHeight
        {
            get => this.maxHeight;

            private set
            {
                if (value == this.maxHeight)
                {
                    return;
                }

                this.maxHeight = value;
                this.OnPropertyChanged();
            }
        }

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            this.disposable?.Dispose();
        }

        private void OnPositivesChanged()
        {
            var allRectangles = ProjectViewModel.Instance.Positives.AllImages.SelectMany(x => x.Rectangles).ToArray();
            if (allRectangles.Length == 0)
            {
                this.Count = 0;
                this.MinWidth = 0;
                this.MaxWidth = 0;
                this.MinHeight = 0;
                this.MaxHeight = 0;
            }
            else
            {
                this.Count = allRectangles.Length;
                this.MinWidth = allRectangles.Min(x => x.Info.Width);
                this.MaxWidth = allRectangles.Max(x => x.Info.Width);
                this.MinHeight = allRectangles.Min(x => x.Info.Height);
                this.MaxHeight = allRectangles.Max(x => x.Info.Height);
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }
    }
}