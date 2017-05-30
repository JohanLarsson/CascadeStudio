namespace OpenCVSharpTrainer
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Windows.Input;

    public class TrainingViewModel : INotifyPropertyChanged
    {
        private int width = 90;
        private int height = 90;

        public TrainingViewModel()
        {
            this.CreateVecFileCommand = new RelayCommand(_ => this.CreateVecFile(), _ => File.Exists(this.Files.InfoFileName) && File.Exists(this.Files.CreateSamplesAppFileName));
            this.SavePositivesAsSeparateFilesCommand = new RelayCommand(_ => this.SavePositivesAsSeparateFiles(), _ => File.Exists(this.Files.InfoFileName));
            this.SaveNegativesCommand = new RelayCommand(_ => this.SaveNegatives(), _ => File.Exists(this.Files.ImageFileName));
            this.CreateNegIndexCommand = new RelayCommand(
                _ => this.CreateNegativesIndex(),
                _ => !string.IsNullOrWhiteSpace(this.Files.NegativesDirectory) &&
                     Directory.EnumerateFiles(this.Files.NegativesDirectory).Any());
            this.PreviewVecFileCommand = new RelayCommand(_ => this.PreviewVecFile(), _ => File.Exists(this.Files.VecFileName));
            this.StartTrainingHaarCommand = new RelayCommand(_ => this.StartTrainingHaar(), _ => File.Exists(this.Files.VecFileName));
            this.StartTrainingLbpCommand = new RelayCommand(_ => this.StartTrainingLbp(), _ => File.Exists(this.Files.VecFileName));
            this.Files.PropertyChanged += (_, args) =>
            {
                switch (args.PropertyName)
                {
                    case nameof(this.Files.InfoFileName):
                    case nameof(this.Files.ImageFileName):
                        this.ReadInfoFile();
                        break;
                }
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public FilesViewModel Files { get; } = new FilesViewModel();

        public ObservableCollection<RectangleInfo> Positives { get; } = new ObservableCollection<RectangleInfo>();

        public ObservableCollection<string> Images { get; } = new ObservableCollection<string>();

        public ICommand CreateVecFileCommand { get; }

        public ICommand SavePositivesAsSeparateFilesCommand { get; }

        public ICommand PreviewVecFileCommand { get; }

        public ICommand SaveNegativesCommand { get; }

        public ICommand CreateNegIndexCommand { get; }

        public ICommand StartTrainingHaarCommand { get; }

        public ICommand StartTrainingLbpCommand { get; }

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

        public void SaveInfo(FileInfo file)
        {
            var relativeFileName = this.Files.GetRelativeFileName(file.FullName, this.Files.ImageFileName);
            var newLIne = $"{relativeFileName} {this.Positives.Count} {string.Join(" ", this.Positives.Select(p => $"{p.X} {p.Y} {p.Width} {p.Height}"))}";
            if (File.Exists(file.FullName))
            {
                var oldLine = File.ReadAllLines(file.FullName).SingleOrDefault(l => l.StartsWith(relativeFileName));
                if (oldLine != null)
                {
                    File.WriteAllText(
                        file.FullName,
                        File.ReadAllText(file.FullName)
                            .Replace(oldLine, newLIne));
                    return;
                }
            }

            File.AppendAllLines(file.FullName, new[] { newLIne });
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ReadInfoFile()
        {
            this.Positives.Clear();
            this.Images.Clear();
            try
            {
                if (!File.Exists(this.Files.InfoFileName))
                {
                    return;
                }

                var lines = InfoFile.Load(this.Files.InfoFileName);
                foreach (var line in lines.Lines)
                {
                    this.Images.Add(line.ImageFileName);
                    if (File.Exists(this.Files.ImageFileName))
                    {
                        if (this.Files.GetFileNameRelativeToInfo(this.Files.ImageFileName) == line.ImageFileName)
                        {
                            foreach (var rect in line.Rectangles)
                            {
                                this.Positives.Add(rect);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void SavePositivesAsSeparateFiles()
        {
            var n = 0;
            var index = new StringBuilder();

            var directoryName = this.Files.PositivesDirectory + "_SeparateFiles";
            Directory.CreateDirectory(directoryName);
            var lines = InfoFile.Load(this.Files.InfoFileName);
            foreach (var line in lines.Lines)
            {
                var sourceFileName = line.ImageFileName;
                using (var image = new Bitmap(Path.Combine(this.Files.RootDirectory, sourceFileName)))
                {
                    foreach (var rectangleInfo in line.Rectangles)
                    {
                        using (var target = new Bitmap(rectangleInfo.Width, rectangleInfo.Height))
                        {
                            using (var graphics = Graphics.FromImage(target))
                            {
                                graphics.DrawImage(
                                    image,
                                    new Rectangle(0, 0, target.Width, target.Width),
                                    new Rectangle(rectangleInfo.X, rectangleInfo.Y, rectangleInfo.Width, rectangleInfo.Height),
                                    GraphicsUnit.Pixel);
                                var fileName = Path.Combine(directoryName, $"{n}.bmp");
                                index.AppendLine($"{this.Files.GetFileNameRelativeToInfo(fileName)} 1 0 0 24 24");
                                target.Save(fileName, ImageFormat.Bmp);
                                n++;
                            }
                        }
                    }
                }
            }

            File.WriteAllText(
                Path.Combine(Path.GetDirectoryName(this.Files.InfoFileName), Path.GetFileNameWithoutExtension(this.Files.InfoFileName) + "_SeparateFiles.info"),
                index.ToString());
        }

        private void SaveNegatives()
        {
            Directory.CreateDirectory(this.Files.NegativesDirectory);
            using (var image = new Bitmap(this.Files.ImageFileName))
            {
                for (var x = 0; x < image.Width - this.Width; x += 10)
                {
                    for (var y = 0; y < image.Height - this.Height; y += 10)
                    {
                        using (var target = new Bitmap(this.Width, this.Height))
                        {
                            using (var graphics = Graphics.FromImage(target))
                            {
                                graphics.DrawImage(
                                    image,
                                    new Rectangle(0, 0, target.Width, target.Width),
                                    new Rectangle(x, y, this.Width, this.Height),
                                    GraphicsUnit.Pixel);
                                var fileName = Path.Combine(this.Files.NegativesDirectory, $"{Path.GetFileNameWithoutExtension(this.Files.ImageFileName)}_{x}_{y}.bmp");
                                target.Save(fileName, ImageFormat.Bmp);
                            }
                        }
                    }
                }
            }
        }

        private void CreateNegativesIndex()
        {
            var index = new StringBuilder();
            foreach (var negative in Directory.EnumerateFiles(this.Files.NegativesDirectory))
            {
                index.AppendLine($"{this.Files.GetFileNameRelativeToNegIndex(negative)}");
            }

            File.WriteAllText(
                this.Files.NegativesIndexFileName,
                index.ToString());
        }

        private void CreateVecFile()
        {
            var infoFile = InfoFile.Load(this.Files.InfoFileName);
            if (infoFile.Width <= 0)
            {
                throw new InvalidOperationException("All samples must have the same width");
            }

            if (infoFile.Height <= 0)
            {
                throw new InvalidOperationException("All samples must have the same height");
            }

            using (var process = Process.Start(
                new ProcessStartInfo
                {
                    FileName = this.Files.CreateSamplesAppFileName,
                    Arguments = $"-info {this.Files.InfoFileName} -vec {this.Files.VecFileName} -w 24 -h 24 -num {infoFile.AllRectangles.Length}",
                }))
            {
                process.WaitForExit();
            }
        }

        private void PreviewVecFile()
        {
            using (var process = Process.Start(
                new ProcessStartInfo
                {
                    FileName = this.Files.CreateSamplesAppFileName,
                    Arguments = $"-vec {this.Files.VecFileName}",
                }))
            {
                process.WaitForExit();
            }
        }

        private void StartTrainingHaar()
        {
            var numNeg = File.ReadAllText(this.Files.NegativesIndexFileName)
                                .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                                .Length;

            var infoFile = InfoFile.Load(this.Files.InfoFileName);
            var numPos = infoFile.AllRectangles.Length;

            Directory.CreateDirectory(this.Files.DataDirectory);
            using (var process = Process.Start(
                new ProcessStartInfo
                {
                    FileName = this.Files.TrainCascadeAppFileName,
                    WorkingDirectory = this.Files.RootDirectory,
                    Arguments = $"-data data -vec {this.Files.VecFileName} -bg bg.txt -numPos {numPos} -numNeg {numNeg} -w 24 -h 24 -featureType HAAR",
                }))
            {
            }
        }

        private void StartTrainingLbp()
        {
            var numNeg = File.ReadAllText(this.Files.NegativesIndexFileName)
                             .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                             .Length;

            var infoFile = InfoFile.Load(this.Files.InfoFileName);
            var numPos = infoFile.AllRectangles.Length;

            Directory.CreateDirectory(this.Files.DataDirectory);
            using (var process = Process.Start(
                new ProcessStartInfo
                {
                    FileName = this.Files.TrainCascadeAppFileName,
                    WorkingDirectory = this.Files.RootDirectory,
                    Arguments = $"-data data -vec {this.Files.VecFileName} -bg bg.txt -numPos {numPos} -numNeg {numNeg} -w 24 -h 24 -featureType LBP",
                }))
            {
            }
        }
    }
}
