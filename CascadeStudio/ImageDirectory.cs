namespace CascadeStudio
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows.Data;
    using Gu.Reactive;

    public class ImageDirectory : INotifyPropertyChanged
    {
        private string path;

        public ImageDirectory()
        {
            this.Children = new CompositeCollection
                            {
                                new CollectionContainer { Collection = this.Images },
                                new CollectionContainer { Collection = this.Directories },
                            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public CompositeCollection Children { get; }

        public ObservableBatchCollection<ImageViewModel> Images { get; } = new ObservableBatchCollection<ImageViewModel>();

        public ObservableBatchCollection<ImageDirectory> Directories { get; } = new ObservableBatchCollection<ImageDirectory>();

        public IEnumerable<ImageViewModel> AllImages => this.Children
                                                            .OfType<ImageViewModel>()
                                                            .Concat(this.Children.OfType<ImageDirectory>()
                                                                                 .SelectMany(dir => dir.AllImages));

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
            this.Children.Clear();
            if (Directory.Exists(this.path))
            {
                foreach (var directory in Directory.EnumerateDirectories(this.path))
                {
                    this.Children.Add(new ImageDirectory { Path = directory });
                }

                foreach (var negative in Directory.EnumerateFiles(this.path))
                {
                    this.Children.Add(new ImageViewModel(negative));
                }
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}