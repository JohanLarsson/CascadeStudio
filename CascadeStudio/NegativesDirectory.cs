namespace CascadeStudio
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Gu.Reactive;

    public class NegativesDirectory : INotifyPropertyChanged
    {
        private string path;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableBatchCollection<ImageViewModel> Images { get; } = new ObservableBatchCollection<ImageViewModel>();

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
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}