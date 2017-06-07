namespace CascadeStudio
{
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [DebuggerDisplay("{this.FileName}")]
    public class ImageViewModel : INotifyPropertyChanged
    {
        private string fileName;

        public ImageViewModel(string fileName)
        {
            this.fileName = fileName;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name => System.IO.Path.GetFileName(this.fileName);

        public string FileName
        {
            get => this.fileName;

            set
            {
                if (value == this.fileName)
                {
                    return;
                }

                this.fileName = value;
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