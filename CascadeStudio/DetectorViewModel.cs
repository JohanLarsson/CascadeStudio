namespace CascadeStudio
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Reactive.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Windows.Media.Imaging;
    using Gu.Reactive;
    using OpenCvSharp;
    using OpenCvSharp.Extensions;

    public sealed class DetectorViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly System.Reactive.Disposables.CompositeDisposable disposable;

        private string imageFile;
        private BitmapSource resultsOverlay;
        private int milliseconds;
        private int count;
        private RenderMatches renderMatches = RenderMatches.Circles;
        private double scaleFactor = 1.1;
        private Size? minSize;
        private Size? maxSize;
        private int minNeighbors = 3;
        private CascadeClassifier classifier;
        private bool disposed;

        private DetectorViewModel()
        {
            var watcher = new FileSystemWatcher
            {
                Filter = "cascade.xml",
                IncludeSubdirectories = true,
            };

            this.disposable = new System.Reactive.Disposables.CompositeDisposable
                              {
                                  watcher,
                                  ProjectViewModel.Instance.ObserveValue(x => x.RootDirectory)
                                                  .Subscribe(
                                                      x =>
                                                      {
                                                          var path = x.GetValueOrDefault();
                                                          watcher.Path = path;
                                                          watcher.EnableRaisingEvents = path != null;
                                                          this.classifier?.Dispose();
                                                          if (path != null &&
                                                              File.Exists(ProjectViewModel.Instance.CascadeFileName))
                                                          {
                                                              this.classifier = new CascadeClassifier(ProjectViewModel.Instance.CascadeFileName);
                                                          }
                                                          else
                                                          {
                                                              this.classifier = null;
                                                          }
                                                          this.UpdateResults();
                                                      }),

                                  Observable.FromEvent<FileSystemEventHandler, FileSystemEventArgs>(
                                                h => (_, e) => h(e),
                                                h => watcher.Changed += h,
                                                h => watcher.Changed -= h)
                                            .Where(args => args.ChangeType == WatcherChangeTypes.Changed)
                                            .Where(args => args.FullPath == ProjectViewModel.Instance.CascadeFileName)
                                            .Subscribe(
                                                args =>
                                                {
                                                    this.classifier?.Dispose();
                                                    this.classifier = new CascadeClassifier(args.FullPath);
                                                    this.UpdateResults();
                                                }),

                                  Observable.FromEvent<FileSystemEventHandler, FileSystemEventArgs>(
                                                h => (_, e) => h(e),
                                                h => watcher.Deleted += h,
                                                h => watcher.Deleted -= h)
                                            .Where(args => args.FullPath == ProjectViewModel.Instance.CascadeFileName)
                                            .Subscribe(
                                                args =>
                                                {
                                                    this.classifier?.Dispose();
                                                    this.classifier = null;
                                                    this.UpdateResults();
                                                }),

                                  Observable.FromEvent<RenamedEventHandler, RenamedEventArgs>(
                                                h => (_, e) => h(e),
                                                h => watcher.Renamed += h,
                                                h => watcher.Renamed -= h)
                                            .Subscribe(
                                                args =>
                                                {
                                                    this.classifier?.Dispose();
                                                    this.classifier = args.FullPath == ProjectViewModel.Instance.CascadeFileName
                                                        ? new CascadeClassifier(args.FullPath)
                                                        : null;
                                                    this.UpdateResults();
                                                }),
                              };
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

        public int Milliseconds
        {
            get => this.milliseconds;

            private set
            {
                if (value == this.milliseconds)
                {
                    return;
                }

                this.milliseconds = value;
                this.OnPropertyChanged();
            }
        }

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

        private string CascadeFile
        {
            get
            {
                var dataDirectory = ProjectViewModel.Instance.DataDirectory;
                return string.IsNullOrEmpty(dataDirectory) ? null : Path.Combine(dataDirectory, "cascade.xml");
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
            if (this.renderMatches == RenderMatches.None ||
                !File.Exists(this.imageFile) ||
                this.classifier == null)
            {
                this.ResultsOverlay = null;
                this.Milliseconds = 0;
                this.Count = 0;
                return;
            }

            using (var overlay = await Task.Run(
                () =>
                {
                    using (var image = new Mat(this.imageFile, ImreadModes.GrayScale))
                    {
                        var sw = Stopwatch.StartNew();
                        {
                            // http://docs.opencv.org/master/db/d28/tutorial_cascade_classifier.html
                            var matches = this.classifier.DetectMultiScale(
                                image,
                                scaleFactor: this.scaleFactor,
                                minSize: this.minSize,
                                maxSize: this.maxSize,
                                minNeighbors: this.minNeighbors,
                                flags: HaarDetectionType.DoCannyPruning);
                            this.Milliseconds = (int)sw.ElapsedMilliseconds;
                            this.Count = matches.Length;
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
                }))
            {
                this.ResultsOverlay = overlay.ToBitmapSource();
            }
        }
    }
}