namespace CascadeStudio
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Windows;
    using System.Windows.Input;
    using System.Xml.Linq;
    using Gu.Reactive;
    using Gu.Units;
    using Gu.Wpf.Reactive;

    public sealed class TrainingViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly ProjectViewModel projectViewModel = ProjectViewModel.Instance;
        private readonly StringBuilder builder = new StringBuilder();
        private readonly IDisposable disposable;

        private string createSamplesAppFileName = @"C:\Program Files\opencv\build\x64\vc14\bin\opencv_createsamples.exe";
        private string trainCascadeAppFileName = @"C:\Program Files\opencv\build\x64\vc14\bin\opencv_traincascade.exe";

        private int width = 24;
        private int height = 24;
        private int? numPos;
        private int? numNeg;
        private int? numStages;
        private Data? precalcValBufSize;
        private Data? precalcIdxBufSize;
        private int? numThreads;
        private double? acceptanceRatioBreakValue;
        private StageType stageType = StageType.BOOST;
        private FeatureType featureType = FeatureType.HAAR;
        private BoostType boostType = BoostType.GAB;
        private double? minHitRate;
        private double? maxFalseAlarmRate;
        private double? weightTrimRate;
        private int? maxDepth;
        private int? maxWeakCount;
        private HaarTypes haarMode = HaarTypes.Basic;
        private string output;
        private bool disposed;

        private TrainingViewModel()
        {
            this.CreateVecFileCommand = new RelayCommand(
                this.CreateVecFile,
                () => File.Exists(this.projectViewModel.InfoFileName) && File.Exists(this.createSamplesAppFileName));

            this.PreviewVecFileCommand = new RelayCommand(
                this.PreviewVecFile,
                () => File.Exists(this.projectViewModel.VecFileName));

            this.StartTrainingCommand = new RelayCommand(
                this.StartTraining,
                () => File.Exists(this.projectViewModel.VecFileName) &&
                      File.Exists(this.projectViewModel.NegativesIndexFileName));

            this.disposable = CascadeFileWatcher.Instance.ObserveValue(x => x.CascadeFile)
                                                .Subscribe(x => this.OnCascadeFileChanged(x.GetValueOrDefault()));
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

        public event PropertyChangedEventHandler PropertyChanged;

        public static TrainingViewModel Instance { get; } = new TrainingViewModel();

        public ICommand CreateVecFileCommand { get; }

        public ICommand PreviewVecFileCommand { get; }

        public ICommand StartTrainingCommand { get; }

        public string CreateSamplesAppFileName
        {
            get => this.createSamplesAppFileName;

            set
            {
                if (value == this.createSamplesAppFileName)
                {
                    return;
                }

                this.createSamplesAppFileName = value;
                this.OnPropertyChanged();
            }
        }

        public string TrainCascadeAppFileName
        {
            get => this.trainCascadeAppFileName;

            set
            {
                if (value == this.trainCascadeAppFileName)
                {
                    return;
                }

                this.trainCascadeAppFileName = value;
                this.OnPropertyChanged();
            }
        }

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

        public int? NumPos
        {
            get => this.numPos;

            set
            {
                if (value == this.numPos)
                {
                    return;
                }

                this.numPos = value;
                this.OnPropertyChanged();
            }
        }

        public int? NumNeg
        {
            get => this.numNeg;

            set
            {
                if (value == this.numNeg)
                {
                    return;
                }

                this.numNeg = value;
                this.OnPropertyChanged();
            }
        }

        public int? NumStages
        {
            get => this.numStages;

            set
            {
                if (value == this.numStages)
                {
                    return;
                }

                this.numStages = value;
                this.OnPropertyChanged();
            }
        }

        public Data? PrecalcValBufSize
        {
            get => this.precalcValBufSize;

            set
            {
                if (value == this.precalcValBufSize)
                {
                    return;
                }

                this.precalcValBufSize = value;
                this.OnPropertyChanged();
            }
        }

        public Data? PrecalcIdxBufSize
        {
            get => this.precalcIdxBufSize;

            set
            {
                if (value == this.precalcIdxBufSize)
                {
                    return;
                }

                this.precalcIdxBufSize = value;
                this.OnPropertyChanged();
            }
        }

        public int? NumThreads
        {
            get => this.numThreads;

            set
            {
                if (value == this.numThreads)
                {
                    return;
                }

                this.numThreads = value;
                this.OnPropertyChanged();
            }
        }

        public double? AcceptanceRatioBreakValue
        {
            get => this.acceptanceRatioBreakValue;

            set
            {
                if (value == this.acceptanceRatioBreakValue)
                {
                    return;
                }

                this.acceptanceRatioBreakValue = value;
                this.OnPropertyChanged();
            }
        }

        public StageType StageType
        {
            get => this.stageType;

            set
            {
                if (value == this.stageType)
                {
                    return;
                }

                this.stageType = value;
                this.OnPropertyChanged();
            }
        }

        public FeatureType FeatureType
        {
            get => this.featureType;

            set
            {
                if (value == this.featureType)
                {
                    return;
                }

                this.featureType = value;
                this.OnPropertyChanged();
            }
        }

        public BoostType BoostType
        {
            get => this.boostType;

            set
            {
                if (value == this.boostType)
                {
                    return;
                }

                this.boostType = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Minimal desired hit rate for each stage of the classifier. Overall hit rate may be estimated as (min_hit_rate ^ number_of_stages), [174] §4.1.
        /// http://docs.opencv.org/master/dc/d88/tutorial_traincascade.html
        /// </summary>
        public double? MinHitRate
        {
            get => this.minHitRate;

            set
            {
                if (value == this.minHitRate)
                {
                    return;
                }

                this.minHitRate = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Maximal desired false alarm rate for each stage of the classifier. Overall false alarm rate may be estimated as (max_false_alarm_rate ^ number_of_stages), [174] §4.1.
        /// http://docs.opencv.org/master/dc/d88/tutorial_traincascade.html
        /// </summary>
        public double? MaxFalseAlarmRate
        {
            get => this.maxFalseAlarmRate;

            set
            {
                if (value == this.maxFalseAlarmRate)
                {
                    return;
                }

                this.maxFalseAlarmRate = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Specifies whether trimming should be used and its weight. A decent choice is 0.95.
        /// http://docs.opencv.org/master/dc/d88/tutorial_traincascade.html
        /// </summary>
        public double? WeightTrimRate
        {
            get => this.weightTrimRate;

            set
            {
                if (value == this.weightTrimRate)
                {
                    return;
                }

                this.weightTrimRate = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Maximal depth of a weak tree. A decent choice is 1, that is case of stumps.
        /// http://docs.opencv.org/master/dc/d88/tutorial_traincascade.html
        /// </summary>
        public int? MaxDepth
        {
            get => this.maxDepth;

            set
            {
                if (value == this.maxDepth)
                {
                    return;
                }

                this.maxDepth = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        ///  Maximal count of weak trees for every cascade stage. The boosted classifier (stage) will have so many weak trees (<=maxWeakCount), as needed to achieve the given -maxFalseAlarmRate.
        /// http://docs.opencv.org/master/dc/d88/tutorial_traincascade.html
        /// </summary>
        public int? MaxWeakCount
        {
            get => this.maxWeakCount;

            set
            {
                if (value == this.maxWeakCount)
                {
                    return;
                }

                this.maxWeakCount = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        ///  Maximal count of weak trees for every cascade stage. The boosted classifier (stage) will have so many weak trees (<=maxWeakCount), as needed to achieve the given -maxFalseAlarmRate.
        /// http://docs.opencv.org/master/dc/d88/tutorial_traincascade.html
        /// </summary>
        public HaarTypes HaarMode
        {
            get => this.haarMode;

            set
            {
                if (value == this.haarMode)
                {
                    return;
                }

                this.haarMode = value;
                this.OnPropertyChanged();
            }
        }

        public string Output
        {
            get => this.output;

            set
            {
                if (value == this.output)
                {
                    return;
                }

                this.output = value;
                this.OnPropertyChanged();
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void CreateVecFile()
        {
            var vecFileName = this.projectViewModel.VecFileName;
            File.Delete(vecFileName);
            var infoFile = InfoFile.Load(this.projectViewModel.InfoFileName);
            using (var process = Process.Start(
                new ProcessStartInfo
                {
                    FileName = this.CreateSamplesAppFileName,
                    Arguments = $"-info {Path.GetFileName(this.projectViewModel.InfoFileName)} -vec {Path.GetFileName(vecFileName)} -w {this.Width} -h {this.Height} -num {infoFile.AllRectangles.Length}",
                    WorkingDirectory = this.projectViewModel.RootDirectory,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                }))
            {
                this.builder.Clear();
                while (!process.StandardOutput.EndOfStream)
                {
                    this.builder.AppendLine(process.StandardOutput.ReadLine());
                }

                this.Output = this.builder.ToString();
            }
        }

        private void PreviewVecFile()
        {
            using (var process = Process.Start(
                new ProcessStartInfo
                {
                    FileName = this.CreateSamplesAppFileName,
                    Arguments = $"-vec {Path.GetFileName(this.projectViewModel.VecFileName)}",
                    WorkingDirectory = this.projectViewModel.RootDirectory,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                }))
            {
                var builder = new StringBuilder();
                while (!process.StandardOutput.EndOfStream)
                {
                    builder.AppendLine(process.StandardOutput.ReadLine());
                }

                var text = builder.ToString();
                process.WaitForExit();
            }
        }

        private void StartTraining()
        {
            var numNeg = File.ReadAllText(this.projectViewModel.NegativesIndexFileName)
                             .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                             .Length;

            var infoFile = InfoFile.Load(this.projectViewModel.InfoFileName);
            var numPos = infoFile.AllRectangles.Length;
            var dataDirectory = this.projectViewModel.DataDirectory;
            if (Directory.Exists(dataDirectory) && Directory.EnumerateFiles(dataDirectory).Any())
            {
                if (MessageBox.Show(Application.Current.MainWindow, "The contents in the data directory will be deleted.\r\nDo you want to continue?", "Data directory is not empty.", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    return;
                }

                Directory.Delete(dataDirectory, recursive: true);
            }

            this.builder.Clear();
            this.builder.Append($"-data data")
                .Append($" -vec {Path.GetFileName(this.projectViewModel.VecFileName)}")
                .Append($" -bg {Path.GetFileName(this.projectViewModel.NegativesIndexFileName)}")
                .Append($" -numPos {numPos}")
                .Append($" -numNeg {numNeg}")
                .Append($" -w {this.Width}")
                .Append($" -h {this.Height}")
                .AppendIfNotNull(this.precalcValBufSize?.Megabyte, " -precalcValBufSize {0}")
                .AppendIfNotNull(this.precalcIdxBufSize?.Megabyte, " -precalcIdxBufSize {0}")
                .AppendIfNotNull(this.numThreads, " -numThreads {0}")
                .AppendIfNotNull(this.acceptanceRatioBreakValue, " -acceptanceRatioBreakValue {0}")
                .Append(" -stageType BOOST")
                .Append($" -featureType {this.featureType}")
                .AppendIfNotNull(this.minHitRate, " -minHitRate {0}")
                .AppendIfNotNull(this.maxFalseAlarmRate, " -maxFalseAlarmRate {0}")
                .AppendIfNotNull(this.weightTrimRate, " -weightTrimRate {0}")
                .AppendIfNotNull(this.maxDepth, " -maxDepth {0}")
                .AppendIfNotNull(this.maxWeakCount, " -maxWeakCount {0}");

            if (this.featureType == FeatureType.HAAR)
            {
                this.builder.Append($" -mode {this.haarMode.ToString().ToUpperInvariant()}");
            }

            // http://docs.opencv.org/master/dc/d88/tutorial_traincascade.html
            Directory.CreateDirectory(dataDirectory);
            using (Process.Start(
                new ProcessStartInfo
                {
                    FileName = this.TrainCascadeAppFileName,
                    WorkingDirectory = this.projectViewModel.RootDirectory,
                    Arguments = this.builder.ToString(),
                    //// Arguments = $"-data data -vec {Path.GetFileName(this.projectViewModel.VecFileName)} -bg bg.txt -numPos {numPos} -numNeg {numNeg} -w {this.Width} -h {this.Height} -featureType HAAR",
                }))
            {
            }
        }

        private void OnCascadeFileChanged(string fileName)
        {
            if (File.Exists(fileName))
            {
                try
                {
                    var document = XDocument.Load(fileName);
                    var cascade = document.Root.Element(XName.Get("cascade"));
                    if (cascade == null)
                    {
                        return;
                    }

                    this.FeatureType = (FeatureType)Enum.Parse(typeof(FeatureType), cascade.Element(XName.Get("featureType")).Value, ignoreCase: true);
                    this.Width = int.Parse(cascade.Element(XName.Get("width")).Value, CultureInfo.InvariantCulture);
                    this.Height = int.Parse(cascade.Element(XName.Get("height")).Value, CultureInfo.InvariantCulture);
                    var stageParams = cascade.Element(XName.Get("stageParams"));
                    if (stageParams != null)
                    {
                        this.BoostType = (BoostType)Enum.Parse(typeof(BoostType), stageParams.Element(XName.Get("boostType")).Value, ignoreCase: true);
                        this.MinHitRate = double.Parse(stageParams.Element(XName.Get("minHitRate")).Value, CultureInfo.InvariantCulture);
                        this.MaxFalseAlarmRate = double.Parse(stageParams.Element(XName.Get("maxFalseAlarm")).Value, CultureInfo.InvariantCulture);
                        this.WeightTrimRate = double.Parse(stageParams.Element(XName.Get("weightTrimRate")).Value, CultureInfo.InvariantCulture);
                        this.MaxDepth = int.Parse(stageParams.Element(XName.Get("maxDepth")).Value, CultureInfo.InvariantCulture);
                        this.MaxWeakCount = int.Parse(stageParams.Element(XName.Get("maxWeakCount")).Value, CultureInfo.InvariantCulture);
                    }

                    var featureParams = cascade.Element(XName.Get("featureParams"));
                    if (featureParams != null)
                    {
                        this.HaarMode = (HaarTypes)Enum.Parse(typeof(HaarTypes), featureParams.Element(XName.Get("mode"))?.Value ?? "BASIC", ignoreCase: true);
                    }
                }
                catch
                {
                    // just swallowing here, might add to view later
                }
            }
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
