namespace CascadeStudio
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Windows.Media.Imaging;
    using Gu.Reactive;
    using OpenCvSharp;
    using OpenCvSharp.Extensions;

    public sealed class DetectorViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly IDisposable disposable;
        private string imageFile;
        private BitmapSource resultsOverlay;
        private TimeSpan elapsed;
        private Exception exception;
        private RenderMatches renderMatches = RenderMatches.Circles;
        private double scaleFactor = 1.1;
        private Size? minSize;
        private Size? maxSize;
        private int minNeighbors = 3;
        private CascadeClassifier classifier;
        private bool disposed;

        private DetectorViewModel()
        {
            this.disposable = RootDirectoryWatcher.Instance.ObserveValue(x => x.CascadeFile)
                                                .Subscribe(x => this.UpdateClassifier(x.GetValueOrDefault()));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static DetectorViewModel Instance { get; } = new DetectorViewModel();

        public string ImageFile
        {
            get => this.imageFile;

            set
            {
                if (value == this.imageFile)
                {
                    return;
                }

                this.imageFile = value;
                this.OnPropertyChanged();
                this.UpdateResults();
            }
        }

        public BitmapSource ResultsOverlay
        {
            get => this.resultsOverlay;

            set
            {
                if (ReferenceEquals(value, this.resultsOverlay))
                {
                    return;
                }

                this.resultsOverlay = value;
                this.OnPropertyChanged();
            }
        }

        public ObservableBatchCollection<Rect> Matches { get; } = new ObservableBatchCollection<Rect>();

        public TimeSpan Elapsed
        {
            get => this.elapsed;

            private set
            {
                if (value == this.elapsed)
                {
                    return;
                }

                this.elapsed = value;
                this.OnPropertyChanged();
            }
        }

        public Exception Exception
        {
            get => this.exception;

            private set
            {
                if (ReferenceEquals(value, this.exception))
                {
                    return;
                }

                this.exception = value;
                this.OnPropertyChanged();
            }
        }

        public RenderMatches RenderMatches
        {
            get => this.renderMatches;

            set
            {
                if (value == this.renderMatches)
                {
                    return;
                }

                this.renderMatches = value;
                this.OnPropertyChanged();
                this.UpdateResults();
            }
        }

        public double ScaleFactor
        {
            get => this.scaleFactor;

            set
            {
                if (value == this.scaleFactor)
                {
                    return;
                }

                this.scaleFactor = value;
                this.OnPropertyChanged();
                this.UpdateResults();
            }
        }

        public Size? MinSize
        {
            get => this.minSize;

            set
            {
                if (value == this.minSize)
                {
                    return;
                }

                this.minSize = value;
                this.OnPropertyChanged();
                this.UpdateResults();
            }
        }

        public Size? MaxSize
        {
            get => this.maxSize;

            set
            {
                if (value == this.maxSize)
                {
                    return;
                }

                this.maxSize = value;
                this.OnPropertyChanged();
                this.UpdateResults();
            }
        }

        public int MinNeighbors
        {
            get => this.minNeighbors;

            set
            {
                if (value == this.minNeighbors)
                {
                    return;
                }

                this.minNeighbors = value;
                this.OnPropertyChanged();
                this.UpdateResults();
            }
        }

        public void UpdateClassifier(string fileName)
        {
            try
            {
                this.classifier?.Dispose();

                if (fileName == null ||
                    !File.Exists(fileName))
                {
                    this.classifier = null;
                }
                else
                {
                    this.classifier = new CascadeClassifier(fileName);
                }

                this.UpdateResults();
            }
            catch
            {
                // Maybe show exception in view later
            }
        }

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            this.classifier?.Dispose();
            this.disposable.Dispose();
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

        private async void UpdateResults()
        {
            this.Exception = null;
            this.Matches.Clear();
            if (this.renderMatches == RenderMatches.None ||
                !File.Exists(this.imageFile) ||
                this.classifier == null)
            {
                this.ResultsOverlay = null;
                this.Elapsed = TimeSpan.Zero;
                return;
            }

            try
            {
                IReadOnlyList<Rect> matches = null;
                using (var overlay = await Task.Run(
                    () =>
                    {
                        using (var image = new Mat(this.imageFile, ImreadModes.GrayScale))
                        {
                            var sw = Stopwatch.StartNew();
                            {
                                // http://docs.opencv.org/master/db/d28/tutorial_cascade_classifier.html
                                matches = this.classifier.DetectMultiScale(
                                    image,
                                    scaleFactor: this.scaleFactor,
                                    minSize: this.minSize,
                                    maxSize: this.maxSize,
                                    minNeighbors: this.minNeighbors,
                                    flags: HaarDetectionType.DoCannyPruning);
                                this.Elapsed = sw.Elapsed;
                                using (var overLay = image.OverLay())
                                {
                                    foreach (var match in matches)
                                    {
                                        switch (this.renderMatches)
                                        {
                                            case RenderMatches.None:
                                                break;
                                            case RenderMatches.Circles:
                                                Cv2.Circle(overLay, match.Midpoint(), Math.Min(match.Width, match.Height) / 2, Scalar4.Green);

                                                break;
                                            case RenderMatches.Rectangles:
                                                Cv2.Rectangle(overLay, match, Scalar4.Green);
                                                break;
                                            default:
                                                throw new ArgumentOutOfRangeException();
                                        }
                                    }

                                    return overLay.ToBitmap();
                                }
                            }
                        }
                    }).ConfigureAwait(true))
                {
                    this.Matches.AddRange(matches);
                    this.ResultsOverlay = overlay.ToBitmapSource();
                }
            }
            catch (Exception e)
            {
                this.Exception = e;
                this.ResultsOverlay = null;
                this.Elapsed = TimeSpan.Zero;
            }
        }
    }
}