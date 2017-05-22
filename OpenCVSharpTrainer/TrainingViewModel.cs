namespace OpenCVSharpTrainer
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;

    public class TrainingViewModel : INotifyPropertyChanged
    {
        private string infoFileName;
        private int width = 24;
        private int height = 24;
        private string imageFileName;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<RectangleInfo> Positives { get; } = new ObservableCollection<RectangleInfo>();

        public int Width
        {
            get
            {
                return this.width;
            }

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
            get
            {
                return this.height;
            }

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

        public string InfoFileName
        {
            get
            {
                return this.infoFileName;
            }

            set
            {
                if (value == this.infoFileName)
                {
                    return;
                }

                this.infoFileName = value;
                this.OnPropertyChanged();
                this.ReadInfoFile();
            }
        }

        public string ImageFileName
        {
            get
            {
                return this.imageFileName;
            }

            set
            {
                if (value == this.imageFileName)
                {
                    return;
                }

                this.imageFileName = value;
                this.OnPropertyChanged();
                this.ReadInfoFile();
            }
        }

        public void SavePositives(string fileName)
        {
            var line = File.ReadAllLines(fileName).SingleOrDefault(l => l.StartsWith(this.imageFileName));
            File.WriteAllText(
                this.infoFileName,
                File.ReadAllText(fileName)
                    .Replace(line, $"{this.imageFileName} {this.Positives.Count} {string.Join(" ", this.Positives.Select(p => $"{p.X} {p.Y} {p.Width} {p.Height}"))}"));
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ReadInfoFile()
        {
            this.Positives.Clear();
            try
            {
                if (!File.Exists(this.infoFileName) ||
                    !File.Exists(this.imageFileName))
                {
                    return;
                }

                var line = File.ReadAllLines(this.infoFileName).SingleOrDefault(l => l.StartsWith(this.ImageFileName))
                               ?.Replace(this.imageFileName, string.Empty);
                if (line == null)
                {
                    return;
                }

                var matches = Regex.Matches(line, @"(?<rect>\d+ \d+ \d+ \d+)", RegexOptions.RightToLeft);
                foreach (Match match in matches)
                {
                    var rect = match.Groups["rect"].Value;
                    var coords = Regex.Match(rect, @"(?<x>\d+) (?<y>\d+) (?<w>\d+) (?<h>\d+)");
                    this.Positives.Insert(
                        0,
                        new RectangleInfo(
                            int.Parse(coords.Groups["x"].Value),
                            int.Parse(coords.Groups["y"].Value),
                            int.Parse(coords.Groups["w"].Value),
                            int.Parse(coords.Groups["h"].Value)));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
