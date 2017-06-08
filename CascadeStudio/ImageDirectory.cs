namespace CascadeStudio
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Gu.Reactive;

    public class ImageDirectory : INotifyPropertyChanged
    {
        private string path;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableBatchCollection<ImageViewModel> Images { get; } = new ObservableBatchCollection<ImageViewModel>();

        public ObservableBatchCollection<ImageDirectory> Directories { get; } = new ObservableBatchCollection<ImageDirectory>();

        public IEnumerable<ImageViewModel> AllImages => this.Images
                                                            .Concat(this.Directories.SelectMany(dir => dir.AllImages));

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
                this.Update(value);
            }
        }

        public void Update(string fileName)
        {
            if (Directory.Exists(this.path))
            {
                var directories = Directory.EnumerateDirectories(this.path).ToArray();
                if (!DirectoriesEquals(directories, this.Directories))
                {
                    this.Directories.Clear();
                    foreach (var directory in directories)
                    {
                        this.Directories.Add(new ImageDirectory { Path = directory });
                    }
                }
                else
                {
                    foreach (var directory in this.Directories)
                    {
                        directory.Update(fileName);
                    }
                }

                var files = Directory.EnumerateFiles(this.path).Where(Filters.IsImageFile).ToArray();
                if (!FilesEquals(files, this.Images))
                {
                    foreach (var negative in files)
                    {
                        this.Images.Add(new ImageViewModel(negative));
                    }
                }
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static bool DirectoriesEquals(IReadOnlyList<string> directories, IReadOnlyList<ImageDirectory> vms)
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

        private static bool FilesEquals(IReadOnlyList<string> files, IReadOnlyList<ImageViewModel> vms)
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
    }
}