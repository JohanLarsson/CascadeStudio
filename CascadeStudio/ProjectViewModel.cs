namespace CascadeStudio
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Windows;
    using System.Windows.Input;
    using Gu.Reactive;
    using Gu.Wpf.Reactive;
    using Ookii.Dialogs.Wpf;

    public class ProjectViewModel : INotifyPropertyChanged
    {
        private string negativesDirectory;
        private string positivesDirectory;
        private string dataDirectory;
        private string infoFileName;
        private string vecFileName;
        private string negativesIndexFileName;
        private string rootDirectory;
        private string runBatFileName;
        private string createSamplesAppFileName = @"C:\Program Files\opencv\build\x64\vc14\bin\opencv_createsamples.exe";
        private string trainCascadeAppFileName = @"C:\Program Files\opencv\build\x64\vc14\bin\opencv_traincascade.exe";
        private string imageFileName;

        public ProjectViewModel()
        {
            this.CreateNewCommand = new RelayCommand(this.CreateNew);

            this.CreateVecFileCommand = new RelayCommand(
                this.CreateVecFile,
                () => File.Exists(this.infoFileName) && File.Exists(this.createSamplesAppFileName));

            this.SavePositivesAsSeparateFilesCommand = new RelayCommand(
                this.SavePositivesAsSeparateFiles,
                () => File.Exists(this.infoFileName));

            this.CreateNegIndexCommand = new RelayCommand(
                this.CreateNegativesIndex,
                () => !string.IsNullOrWhiteSpace(this.negativesDirectory) &&
                      Directory.EnumerateFiles(this.negativesDirectory).Any());

            this.PreviewVecFileCommand = new RelayCommand(
                this.PreviewVecFile,
                () => File.Exists(this.vecFileName));

            this.StartTrainingHaarCommand = new RelayCommand(
                this.StartTrainingHaar,
                () => File.Exists(this.vecFileName));

            this.StartTrainingLbpCommand = new RelayCommand(
                this.StartTrainingLbp,
                () => File.Exists(this.vecFileName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand CreateNewCommand { get; }

        public ICommand OpenCommand { get; }

        public ICommand SaveCommand { get; }

        public ICommand SaveAllCommand { get; }

        public ICommand CreateVecFileCommand { get; }

        public ICommand SavePositivesAsSeparateFilesCommand { get; }

        public ICommand PreviewVecFileCommand { get; }

        public ICommand CreateNegIndexCommand { get; }

        public ICommand StartTrainingHaarCommand { get; }

        public ICommand StartTrainingLbpCommand { get; }

        public ObservableBatchCollection<RectangleInfo> Positives { get; } = new ObservableBatchCollection<RectangleInfo>();

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

        public string RootDirectory
        {
            get => this.rootDirectory;

            set
            {
                if (value == this.rootDirectory)
                {
                    return;
                }

                this.rootDirectory = value;
                this.OnPropertyChanged();
            }
        }

        public string NegativesDirectory
        {
            get => this.negativesDirectory;

            set
            {
                if (value == this.negativesDirectory)
                {
                    return;
                }

                this.negativesDirectory = value;
                this.OnPropertyChanged();
            }
        }

        public string PositivesDirectory
        {
            get => this.positivesDirectory;

            set
            {
                if (value == this.positivesDirectory)
                {
                    return;
                }

                this.positivesDirectory = value;
                this.OnPropertyChanged();
            }
        }

        public string DataDirectory
        {
            get => this.dataDirectory;

            set
            {
                if (value == this.dataDirectory)
                {
                    return;
                }

                this.dataDirectory = value;
                this.OnPropertyChanged();
            }
        }

        public string InfoFileName
        {
            get => this.infoFileName;

            set
            {
                if (value == this.infoFileName)
                {
                    return;
                }

                this.infoFileName = value;
                this.OnPropertyChanged();
            }
        }

        public string VecFileName
        {
            get => this.vecFileName;

            set
            {
                if (value == this.vecFileName)
                {
                    return;
                }

                this.vecFileName = value;
                this.OnPropertyChanged();
            }
        }

        public string NegativesIndexFileName
        {
            get => this.negativesIndexFileName;

            set
            {
                if (value == this.negativesIndexFileName)
                {
                    return;
                }

                this.negativesIndexFileName = value;
                this.OnPropertyChanged();
            }
        }

        public string RunBatFileName
        {
            get => this.runBatFileName;

            set
            {
                if (value == this.runBatFileName)
                {
                    return;
                }

                this.runBatFileName = value;
                this.OnPropertyChanged();
            }
        }

        public string ImageFileName
        {
            get => this.imageFileName;

            set
            {
                if (value == this.imageFileName)
                {
                    return;
                }

                this.imageFileName = value;
                this.OnPropertyChanged();
            }
        }

        public string GetFileNameRelativeToNegIndex(string fileName) => this.GetRelativeFileName(this.negativesIndexFileName, fileName);

        public string GetFileNameRelativeToInfo(string fileName) => this.GetRelativeFileName(this.infoFileName, fileName);

        public string GetRelativeFileName(string fileName, string fileNameToTrim)
        {
            var directoryName = Path.GetDirectoryName(fileName);
            return fileNameToTrim.Replace(directoryName + "\\", string.Empty);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void CreateNew()
        {
            var dialog = new VistaFolderBrowserDialog();
            if (dialog.ShowDialog(Application.Current.MainWindow) == true)
            {
                this.RootDirectory = dialog.SelectedPath;

                this.InfoFileName = Path.Combine(dialog.SelectedPath, "positives.info");
                this.PositivesDirectory = Path.Combine(dialog.SelectedPath, "Positives");
                this.negativesDirectory = Path.Combine(dialog.SelectedPath, "Negatives");
                this.vecFileName = Path.ChangeExtension(this.infoFileName, ".vec");
                this.NegativesIndexFileName = Path.Combine(dialog.SelectedPath, "bg.txt");
                this.RunBatFileName = Path.Combine(dialog.SelectedPath, "run.bat");
                if (!Directory.Exists(this.positivesDirectory))
                {
                    Directory.CreateDirectory(this.positivesDirectory);
                }

                if (!Directory.Exists(this.negativesDirectory))
                {
                    Directory.CreateDirectory(this.negativesDirectory);
                }

                if (!File.Exists(this.infoFileName))
                {
                    File.WriteAllText(this.infoFileName, string.Empty);
                }
            }
        }

        private void Open()
        {
            //if (e.Parameter != null)
            //{
            //    this.ViewModel.Project.ImageFileName = Path.Combine(Path.GetDirectoryName(this.ViewModel.Project.InfoFileName), (string)e.Parameter);
            //}
            //else
            //{
            //    var dialog = new Ookii.Dialogs.Wpf.VistaOpenFileDialog();
            //    if (dialog.ShowDialog(Application.Current.MainWindow) == true)
            //    {
            //        if (dialog.FileName.EndsWith(".info"))
            //        {
            //            this.infoFileName = dialog.FileName;
            //        }
            //        else
            //        {
            //            this.imageFileName = dialog.FileName;
            //        }
            //    }
            //}
        }

        public void SaveInfo(FileInfo file)
        {
            var relativeFileName = this.GetRelativeFileName(file.FullName, this.imageFileName);
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

        private void ReadInfoFile(string fileName)
        {
            this.Positives.Clear();
            try
            {
                if (!File.Exists(fileName))
                {
                    return;
                }

                var lines = InfoFile.Load(fileName);
                foreach (var line in lines.Lines)
                {
                    if (File.Exists(this.imageFileName))
                    {
                        if (this.GetFileNameRelativeToInfo(this.imageFileName) == line.ImageFileName)
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

            var directoryName = this.PositivesDirectory + "_SeparateFiles";
            Directory.CreateDirectory(directoryName);
            var lines = InfoFile.Load(this.infoFileName);
            foreach (var line in lines.Lines)
            {
                var sourceFileName = line.ImageFileName;
                using (var image = new Bitmap(Path.Combine(this.RootDirectory, sourceFileName)))
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
                                index.AppendLine($"{this.GetFileNameRelativeToInfo(fileName)} 1 0 0 24 24");
                                target.Save(fileName, ImageFormat.Bmp);
                                n++;
                            }
                        }
                    }
                }
            }

            File.WriteAllText(
                Path.Combine(Path.GetDirectoryName(this.infoFileName), Path.GetFileNameWithoutExtension(this.infoFileName) + "_SeparateFiles.info"),
                index.ToString());
        }

        private void CreateNegativesIndex()
        {
            var index = new StringBuilder();
            foreach (var negative in Directory.EnumerateFiles(this.negativesDirectory))
            {
                index.AppendLine($"{this.GetFileNameRelativeToNegIndex(negative)}");
            }

            File.WriteAllText(
                this.NegativesIndexFileName,
                index.ToString());
        }

        private void CreateVecFile()
        {
            var infoFile = InfoFile.Load(this.infoFileName);
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
                    FileName = this.CreateSamplesAppFileName,
                    Arguments = $"-info {this.infoFileName} -vec {this.vecFileName} -w 24 -h 24 -num {infoFile.AllRectangles.Length}",
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
                    FileName = this.CreateSamplesAppFileName,
                    Arguments = $"-vec {this.vecFileName}",
                }))
            {
                process.WaitForExit();
            }
        }

        private void StartTrainingHaar()
        {
            var numNeg = File.ReadAllText(this.NegativesIndexFileName)
                .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .Length;

            var infoFile = InfoFile.Load(this.infoFileName);
            var numPos = infoFile.AllRectangles.Length;

            Directory.CreateDirectory(this.DataDirectory);
            using (var process = Process.Start(
                new ProcessStartInfo
                {
                    FileName = this.TrainCascadeAppFileName,
                    WorkingDirectory = this.RootDirectory,
                    Arguments = $"-data data -vec {this.vecFileName} -bg bg.txt -numPos {numPos} -numNeg {numNeg} -w 24 -h 24 -featureType HAAR",
                }))
            {
            }
        }

        private void StartTrainingLbp()
        {
            var numNeg = File.ReadAllText(this.NegativesIndexFileName)
                .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .Length;

            var infoFile = InfoFile.Load(this.infoFileName);
            var numPos = infoFile.AllRectangles.Length;

            Directory.CreateDirectory(this.DataDirectory);
            using (var process = Process.Start(
                new ProcessStartInfo
                {
                    FileName = this.TrainCascadeAppFileName,
                    WorkingDirectory = this.RootDirectory,
                    Arguments = $"-data data -vec {this.vecFileName} -bg bg.txt -numPos {numPos} -numNeg {numNeg} -w 24 -h 24 -featureType LBP",
                }))
            {
            }
        }
    }
}
