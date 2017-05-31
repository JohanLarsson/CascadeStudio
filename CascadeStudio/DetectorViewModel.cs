namespace CascadeStudio
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Windows.Media.Imaging;
    using OpenCvSharp;
    using OpenCvSharp.Extensions;

    public sealed class DetectorViewModel : INotifyPropertyChanged, IDisposable
    {
        private string modelFile;
        private string imageFile;
        private BitmapSource resultsOverlay;
        private int milliseconds;
        private RenderMatch renderMatches = RenderMatch.Circle;
        private CascadeClassifier classifier;
        private bool disposed;
        private DateTime lastWriteTime;

        private DetectorViewModel()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static DetectorViewModel Instance { get; } = new DetectorViewModel();

        public string ModelFile
        {
            get => this.modelFile;

            set
            {
                if (File.Exists(value))
                {
                    var writeTime = File.GetLastWriteTime(value);
                    if (value != this.modelFile ||
                        writeTime != this.lastWriteTime)
                    {
                        this.lastWriteTime = writeTime;
                        this.classifier?.Dispose();
                        this.classifier = new CascadeClassifier(value);
                        this.UpdateResults();
                    }
                }
                else
                {
                    this.classifier?.Dispose();
                }

                if (value == this.modelFile)
                {
                    return;
                }

                this.modelFile = value;
                this.OnPropertyChanged();
            }
        }

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

        public int Milliseconds
        {
            get => this.milliseconds;

            set
            {
                if (value == this.milliseconds)
                {
                    return;
                }

                this.milliseconds = value;
                this.OnPropertyChanged();
            }
        }

        public RenderMatch RenderMatches
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

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            this.classifier?.Dispose();
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
            if (this.renderMatches == RenderMatch.None ||
                !File.Exists(this.imageFile) ||
                this.classifier == null)
            {
                this.ResultsOverlay = null;
                return;
            }

            using (var overlay = await Task.Run(
                () =>
                {
                    using (var image = new Mat(this.imageFile, ImreadModes.GrayScale))
                    {
                        var sw = Stopwatch.StartNew();
                        {
                            var matches = this.classifier.DetectMultiScale(image);
                            this.Milliseconds = (int)sw.ElapsedMilliseconds;
                            using (var overLay = image.OverLay())
                            {
                                foreach (var match in matches)
                                {
                                    switch (this.renderMatches)
                                    {
                                        case RenderMatch.None:
                                            break;
                                        case RenderMatch.Circle:
                                            Cv2.Circle(overLay, match.Midpoint(), Math.Min(match.Width, match.Height) / 2, Scalar4.Green);

                                            break;
                                        case RenderMatch.Rectangle:
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
                }))
            {
                this.ResultsOverlay = overlay.ToBitmapSource();
            }
        }
    }
}