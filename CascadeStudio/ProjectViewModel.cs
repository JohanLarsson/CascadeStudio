namespace CascadeStudio
{
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;
    using Gu.Wpf.Reactive;

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
            CreateNewCommand = new RelayCommand(CreateNew);
        }

        private void CreateNew()
        {
            throw new System.NotImplementedException();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand CreateNewCommand { get; }

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

                this.InfoFileName = string.IsNullOrWhiteSpace(value) ? null : Path.Combine(value, "positives.info");
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
                if (string.IsNullOrWhiteSpace(value))
                {
                    this.PositivesDirectory = null;
                    this.NegativesDirectory = null;
                    this.DataDirectory = null;
                    this.VecFileName = null;
                    this.RunBatFileName = null;
                }
                else
                {
                    var directory = Path.GetDirectoryName(value);
                    this.RootDirectory = directory;
                    this.PositivesDirectory = Path.Combine(directory, "Positives");
                    this.NegativesDirectory = Path.Combine(directory, "Negatives");
                    this.DataDirectory = Path.Combine(directory, "Data");
                    Directory.CreateDirectory(this.PositivesDirectory);
                    Directory.CreateDirectory(this.NegativesDirectory);
                    Directory.CreateDirectory(this.DataDirectory);
                    this.VecFileName = Path.ChangeExtension(value, ".vec");
                    this.NegativesIndexFileName = Path.Combine(directory, "bg.txt");
                    this.RunBatFileName = Path.Combine(directory, "run.bat");
                }

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
    }
}
