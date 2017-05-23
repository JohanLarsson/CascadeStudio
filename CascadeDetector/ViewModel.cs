namespace CascadeDetector
{
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Windows.Media.Imaging;
    using OpenCvSharp;
    using OpenCvSharp.Extensions;

    public class ViewModel : INotifyPropertyChanged
    {
        private string modelFile;
        private string imageFile;
        private BitmapSource resultsOverlay;
        private int milliseconds;

        public event PropertyChangedEventHandler PropertyChanged;

        public string ModelFile
        {
            get => this.modelFile;

            set
            {
                if (value == this.modelFile)
                {
                    return;
                }

                this.modelFile = value;
                this.OnPropertyChanged();
                this.UpdateResults();
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

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UpdateResults()
        {
            if (!File.Exists(this.ModelFile) ||
                !File.Exists(this.ImageFile))
            {
                this.ResultsOverlay = null;
                return;
            }

            using (var image = new Mat(this.ImageFile, ImreadModes.Unchanged))
            {
                var sw = Stopwatch.StartNew();
                using (var classifier = new CascadeClassifier(this.ModelFile))
                {
                    var matches = classifier.DetectMultiScale(image);
                    this.Milliseconds = (int)sw.ElapsedMilliseconds;
                    using (var overLay = image.OverLay())
                    {
                        foreach (var match in matches)
                        {
                            Cv2.Rectangle(overLay, match, Scalar4.Red);
                        }

                        this.ResultsOverlay = overLay.ToBitmapSource();
                    }
                }
            }
        }
    }
}
