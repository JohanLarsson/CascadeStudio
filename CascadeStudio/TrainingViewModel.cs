namespace CascadeStudio
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Windows;
    using System.Windows.Input;
    using Gu.Units;
    using Gu.Wpf.Reactive;

    public sealed class TrainingViewModel : INotifyPropertyChanged
    {
        private readonly ProjectViewModel projectViewModel = ProjectViewModel.Instance;
        private readonly StringBuilder outputBuilder = new StringBuilder();

        private string createSamplesAppFileName = @"C:\Program Files\opencv\build\x64\vc14\bin\opencv_createsamples.exe";
        private string trainCascadeAppFileName = @"C:\Program Files\opencv\build\x64\vc14\bin\opencv_traincascade.exe";

        private int width = 24;
        private int height = 24;
        private string output;

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
                this.outputBuilder.Clear();
                while (!process.StandardOutput.EndOfStream)
                {
                    this.outputBuilder.AppendLine(process.StandardOutput.ReadLine());
                }

                this.Output = this.outputBuilder.ToString();
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

            // http://docs.opencv.org/master/dc/d88/tutorial_traincascade.html
            Directory.CreateDirectory(dataDirectory);
            using (Process.Start(
                new ProcessStartInfo
                {
                    FileName = this.TrainCascadeAppFileName,
                    WorkingDirectory = this.projectViewModel.RootDirectory,
                    Arguments = $"-data data -vec {Path.GetFileName(this.projectViewModel.VecFileName)} -bg bg.txt -numPos {numPos} -numNeg {numNeg} -w {this.Width} -h {this.Height} -featureType HAAR",
                }))
            {
            }
        }
    }
}
