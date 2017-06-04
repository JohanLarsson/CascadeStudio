namespace CascadeStudio
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Gu.Reactive;
    using Gu.Wpf.Reactive;

    public class ImageDirectory : INotifyPropertyChanged
    {
        private string path;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableBatchCollection<object> Children { get; } = new ObservableBatchCollection<object>();

        public IEnumerable<ImageViewModel> AllImages => this.Children.OfType<ImageViewModel>()
            .Concat(this.Children.OfType<ImageDirectory>().SelectMany(dir => dir.AllImages));

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