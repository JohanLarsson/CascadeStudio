namespace OpenCVSharpTrainer
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows.Input;

    public class PositivesViewModel : INotifyPropertyChanged
    {
        private string infoFileName;
        private int width = 64;
        private int height = 64;
        private string imageFileName;
        private string createSamplesAppFileName = @"C:\Program Files\opencv\build\x64\vc14\bin\opencv_createsamples.exe";

        public PositivesViewModel()
        {
            this.CreateVecFileCommand = new RelayCommand(_ => this.CreateVecFile(), _ => File.Exists(this.infoFileName) && File.Exists(this.CreateSamplesAppFileName));
            this.SavePositivesAsSeparateFilesCommand = new RelayCommand(_ => this.SavePositivesAsSeparateFiles(), _ => File.Exists(this.infoFileName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<RectangleInfo> Positives { get; } = new ObservableCollection<RectangleInfo>();

        public ICommand CreateVecFileCommand { get; }

        public ICommand SavePositivesAsSeparateFilesCommand { get; }

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

        public string CreateSamplesAppFileName
        {
            get
            {
                return this.createSamplesAppFileName;
            }

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

        public void SaveInfo(FileInfo file)
        {
            var newLIne = $"{this.imageFileName.Replace(Path.GetDirectoryName(file.FullName), string.Empty)} {this.Positives.Count} {string.Join(" ", this.Positives.Select(p => $"{p.X} {p.Y} {p.Width} {p.Height}"))}";
            if (File.Exists(file.FullName))
            {
                var oldLine = File.ReadAllLines(file.FullName).SingleOrDefault(l => l.StartsWith(this.imageFileName));
                if (oldLine != null)
                {
                    File.WriteAllText(
                        this.infoFileName,
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

        private static IEnumerable<RectangleInfo> ParseRectangleInfos(string line)
        {
            var matches = Regex.Matches(line, @"(?<rect>\d+ \d+ \d+ \d+)", RegexOptions.RightToLeft);
            foreach (Match match in matches)
            {
                var rect = match.Groups["rect"].Value;
                var coords = Regex.Match(rect, @"(?<x>\d+) (?<y>\d+) (?<w>\d+) (?<h>\d+)");
                yield return new RectangleInfo(
                    int.Parse(coords.Groups["x"].Value),
                    int.Parse(coords.Groups["y"].Value),
                    int.Parse(coords.Groups["w"].Value),
                    int.Parse(coords.Groups["h"].Value));
            }
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

                foreach (var rectangleInfo in ParseRectangleInfos(line))
                {
                    this.Positives.Insert(0, rectangleInfo);
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

            var directoryName = Path.Combine(Path.GetDirectoryName(this.infoFileName), "Separate");
            Directory.CreateDirectory(directoryName);
            foreach (var line in File.ReadAllLines(this.infoFileName))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var sourceFileName = line.Substring(0, line.IndexOf(" ", line.LastIndexOf(".")));
                using (var image = new Bitmap(sourceFileName))
                {
                    foreach (var rectangleInfo in ParseRectangleInfos(line))
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
                                index.AppendLine($"{fileName} 1 0 0 {rectangleInfo.Width} {rectangleInfo.Height}");
                                target.Save(fileName.Replace(directoryName, string.Empty), ImageFormat.Bmp);
                                n++;
                            }
                        }
                    }
                }
            }

            File.WriteAllText(
                Path.Combine(directoryName, Path.GetFileName(this.infoFileName)),
                index.ToString());
        }

        private void CreateVecFile()
        {
            var positives = File.ReadAllLines(this.infoFileName)
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .SelectMany(ParseRectangleInfos)
                .ToArray();
            var widths = positives.Select(r => r.Width).Distinct();
            if (widths.Count() != 1)
            {
                throw new InvalidOperationException("All samples must have the same width");
            }

            var heights = positives.Select(r => r.Height).Distinct();
            if (heights.Count() != 1)
            {
                throw new InvalidOperationException("All samples must have the same height");
            }

            using (var process = Process.Start(
                new ProcessStartInfo
                {
                    FileName = this.CreateSamplesAppFileName,
                    Arguments = $"-info {this.infoFileName} -vec {Path.ChangeExtension(this.infoFileName, ".vec")} -w {widths.Single()} -h {heights.Single()} -num {positives.Length}",
                }))
            {
                process.WaitForExit();
            }
        }
    }
}
