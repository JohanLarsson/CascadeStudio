namespace CascadeStudio
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Gu.Reactive;

    public sealed class PositivesDirectory : INotifyPropertyChanged
    {
        private string path;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableBatchCollection<PositiveViewModel> Images { get; } = new ObservableBatchCollection<PositiveViewModel>();

        public ObservableBatchCollection<PositivesDirectory> Directories { get; } = new ObservableBatchCollection<PositivesDirectory>();

        public IEnumerable<PositiveViewModel> AllImages => this.Images
                                                               .Concat(this.Directories.SelectMany(dir => dir.AllImages));

        public IEnumerable<RectangleViewModel> AllRectangles => this.AllImages.SelectMany(x => x.Rectangles);

        public string Name => System.IO.Path.GetFileName(this.path);

        public string Path
        {
            get => this.path;

            set
            {
                if (value == this.path)
                {
                    return;
                }

                this.path = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.Name));
                this.Update(this.path);
            }
        }

        public void Update(string fileName)
        {
            if (Directory.Exists(this.path))
            {
                var directories = Directory.EnumerateDirectories(this.path).ToArray();
                if (DirectoriesEquals(directories, this.Directories))
                {
                    foreach (var directory in this.Directories)
                    {
                        directory.Update(fileName);
                    }
                }
                else
                {
                    this.Directories.Clear();
                    foreach (var directory in directories)
                    {
                        this.Directories.Add(new PositivesDirectory { Path = directory });
                    }
                }

                var files = Directory.EnumerateFiles(this.path).Where(Filters.IsImageFile).ToArray();
                if (!FilesEquals(files, this.Images))
                {
                    foreach (var image in files)
                    {
                        this.Images.Add(new PositiveViewModel(image));
                    }
                }
            }

            this.OnPropertyChanged(nameof(this.AllImages));
            this.OnPropertyChanged(nameof(this.AllRectangles));
        }

        public void UpdateRectangles(InfoFile infoFile)
        {
            foreach (var positive in this.AllImages)
            {
                var line = infoFile?.Lines.SingleOrDefault(l => string.Equals(
                    ProjectViewModel.Instance.GetFileNameRelativeToInfo(positive.FileName),
                    l.ImageFileName,
                    StringComparison.InvariantCultureIgnoreCase));
                if (line == null)
                {
                    positive.Rectangles.Clear();
                }
                else if (!RectanglesEquals(line.Rectangles, positive.Rectangles))
                {
                    positive.Rectangles.Clear();
                    positive.Rectangles.AddRange(line.Rectangles.Select(x => new RectangleViewModel(positive, x)));
                }
            }

            this.OnPropertyChanged(nameof(this.AllRectangles));
        }

        private static bool DirectoriesEquals(IReadOnlyList<string> directories, IReadOnlyList<PositivesDirectory> vms)
        {
            if (directories.Count != vms.Count)
            {
                return false;
            }

            for (var i = 0; i < directories.Count; i++)
            {
                if (!string.Equals(directories[i], vms[i].Path, StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool FilesEquals(IReadOnlyList<string> files, IReadOnlyList<PositiveViewModel> vms)
        {
            if (files.Count != vms.Count)
            {
                return false;
            }

            for (var i = 0; i < files.Count; i++)
            {
                if (!string.Equals(files[i], vms[i].FileName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool RectanglesEquals(IReadOnlyList<RectangleInfo> fromFile, IReadOnlyList<RectangleViewModel> vms)
        {
            if (fromFile.Count != vms.Count)
            {
                return false;
            }

            for (var i = 0; i < fromFile.Count; i++)
            {
                if (!RectangleInfo.Comparer.Equals(fromFile[i], vms[i].Info))
                {
                    return false;
                }
            }

            return true;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}