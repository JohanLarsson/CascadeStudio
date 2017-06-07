namespace CascadeStudio
{
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Gu.Reactive;

    public class DataDirectory : INotifyPropertyChanged
    {
        private string path;

        public DataDirectory(string path)
        {
            this.Path = path;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableBatchCollection<TextFileViewModel> Files { get; } = new ObservableBatchCollection<TextFileViewModel>();

        public ObservableBatchCollection<DataDirectory> Directories { get; } = new ObservableBatchCollection<DataDirectory>();

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
                this.Files.Clear();
                if (Directory.Exists(value))
                {
                    this.Files.AddRange(Directory.EnumerateFiles(this.path).Select(x => new TextFileViewModel(x)));
                }
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
