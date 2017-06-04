namespace CascadeStudio
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows.Data;
    using Gu.Reactive;

    public sealed class PositivesDirectory : INotifyPropertyChanged
    {
        private string path;

        public PositivesDirectory()
        {
            this.Children = new CompositeCollection
                       {
                           new CollectionContainer { Collection = this.Images },
                           new CollectionContainer { Collection = this.Directories },
                       };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public CompositeCollection Children { get; }

        public ObservableBatchCollection<PositiveViewModel> Images { get; } = new ObservableBatchCollection<PositiveViewModel>();

        public ObservableBatchCollection<PositivesDirectory> Directories { get; } = new ObservableBatchCollection<PositivesDirectory>();

        public IEnumerable<PositiveViewModel> AllImages => this.Images
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
                this.Update(this.path);
            }
        }

        public void Update(string fileName)
        {
            this.Children.Clear();
            if (Directory.Exists(this.path))
            {
                foreach (var directory in Directory.EnumerateDirectories(this.path))
                {
                    this.Children.Add(new PositivesDirectory { Path = directory });
                }

                foreach (var image in Directory.EnumerateFiles(this.path))
                {
                    this.Children.Add(new PositiveViewModel(image));
                }
            }
        }

        public void UpdateRectangles(InfoFile infoFile)
        {
            foreach (var positive in this.AllImages)
            {
                var line = infoFile?.Lines.SingleOrDefault(l => string.Equals(
                    ProjectViewModel.Instance.GetFileNameRelativeToInfo(positive.FileName),
                    l.ImageFileName,
                    StringComparison.InvariantCultureIgnoreCase));
                positive.Rectangles.Clear();
                if (line != null)
                {
                    positive.Rectangles.AddRange(line.Rectangles.Select(x => new RectangleViewModel(positive, x)));
                }
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}