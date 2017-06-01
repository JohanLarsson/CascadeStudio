namespace CascadeStudio
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Input;
    using Gu.Reactive;
    using Gu.Wpf.Reactive;

    public sealed class RectangleViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly IDisposable disposable;
        private bool disposed;

        public RectangleViewModel(PositiveViewModel positive, RectangleInfo info)
        {
            this.Info = info;
            this.Positive = positive;
            this.IncreaseSizeCommand = new RelayCommand(this.Info.IncreaseSize);
            this.DecreaseSizeCommand = new RelayCommand(this.Info.DecreaseSize, () => this.Info.Width > 3 && this.Info.Height > 3);
            this.DecreaseXCommand = new RelayCommand(() => this.Info.X--, () => this.Info.X > 0);
            this.IncreaseXCommand = new RelayCommand(() => this.Info.X++);

            this.DecreaseYCommand = new RelayCommand(() => this.Info.Y--, () => this.Info.Y > 0);
            this.IncreaseYCommand = new RelayCommand(() => this.Info.Y++);
            this.disposable = this.Info.ObservePropertyChangedSlim()
                .Subscribe(_ => this.OnPropertyChanged(nameof(this.SourceRect)));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public RectangleInfo Info { get; }

        public PositiveViewModel Positive { get; }

        public ICommand IncreaseSizeCommand { get; }

        public ICommand DecreaseSizeCommand { get; }

        public ICommand DecreaseXCommand { get; }

        public ICommand IncreaseXCommand { get; }

        public ICommand DecreaseYCommand { get; }

        public ICommand IncreaseYCommand { get; }

        public Int32Rect SourceRect
        {
            get
            {
                this.ThrowIfDisposed();
                return new Int32Rect(this.Info.X, this.Info.Y, this.Info.Width, this.Info.Height);
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