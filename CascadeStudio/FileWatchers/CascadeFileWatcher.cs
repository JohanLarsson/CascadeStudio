namespace CascadeStudio
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Reactive.Linq;
    using System.Runtime.CompilerServices;
    using Gu.Reactive;

    public sealed class CascadeFileWatcher : INotifyPropertyChanged, IDisposable
    {
        private readonly System.Reactive.Disposables.CompositeDisposable disposable;
        private string cascadeFile;
        private bool disposed;

        public CascadeFileWatcher()
        {
            var watcher = new FileSystemWatcher
                          {
                              Filter = "cascade.xml",
                              IncludeSubdirectories = true,
                          };

            this.disposable = new System.Reactive.Disposables.CompositeDisposable
                              {
                                  watcher,
                                  ProjectViewModel.Instance.ObserveValue(x => x.RootDirectory)
                                                  .Subscribe(
                                                      x =>
                                                      {
                                                          var path = x.GetValueOrDefault();
                                                          watcher.Path = path;
                                                          watcher.EnableRaisingEvents = path != null;
                                                          this.CascadeFile = ProjectViewModel.Instance.CascadeFileName;
                                                      }),

                                  Observable.FromEvent<FileSystemEventHandler, FileSystemEventArgs>(
                                                h => (_, e) => h(e),
                                                h => watcher.Changed += h,
                                                h => watcher.Changed -= h)
                                            .Where(args => args.ChangeType == WatcherChangeTypes.Changed)
                                            .Where(args => args.FullPath == ProjectViewModel.Instance.CascadeFileName)
                                            .Subscribe(args => this.CascadeFile = args.FullPath),

                                  Observable.FromEvent<FileSystemEventHandler, FileSystemEventArgs>(
                                                h => (_, e) => h(e),
                                                h => watcher.Deleted += h,
                                                h => watcher.Deleted -= h)
                                            .Where(args => args.FullPath == ProjectViewModel.Instance.CascadeFileName)
                                            .Subscribe(_ => this.CascadeFile = null),

                                  Observable.FromEvent<RenamedEventHandler, RenamedEventArgs>(
                                                h => (_, e) => h(e),
                                                h => watcher.Renamed += h,
                                                h => watcher.Renamed -= h)
                                            .Subscribe(
                                                args =>
                                                {
                                                    this.CascadeFile = args.FullPath == ProjectViewModel.Instance.CascadeFileName
                                                        ? args.FullPath
                                                        : null;
                                                }),
                              };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static CascadeFileWatcher Instance { get; } = new CascadeFileWatcher();

        public string CascadeFile
        {
            get => this.cascadeFile;

            private set
            {
                if (value == this.cascadeFile)
                {
                    return;
                }

                this.cascadeFile = value;
                this.OnPropertyChanged();
            }
        }

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            this.disposable.Dispose();
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }
    }
}