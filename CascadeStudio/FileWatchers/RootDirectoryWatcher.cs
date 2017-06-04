namespace CascadeStudio
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Reactive.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows.Threading;
    using Gu.Reactive;

    public sealed class RootDirectoryWatcher : INotifyPropertyChanged, IDisposable
    {
        private readonly System.Reactive.Disposables.CompositeDisposable disposable;
        private readonly ProjectViewModel project = ProjectViewModel.Instance;

        private string cascadeFile;
        private bool disposed;

        public RootDirectoryWatcher()
        {
            var watcher = new FileSystemWatcher
            {
                IncludeSubdirectories = true,
            };

            this.disposable = new System.Reactive.Disposables.CompositeDisposable
                              {
                                  watcher,
                                  this.project.ObserveValue(x => x.RootDirectory)
                                      .ObserveOnDispatcher(DispatcherPriority.Background)
                                      .Subscribe(
                                          x =>
                                          {
                                              var path = x.GetValueOrDefault();
                                              watcher.Path = path;
                                              watcher.EnableRaisingEvents = path != null;
                                              this.CascadeFile = this.project.CascadeFileName;
                                          }),

                                  Observable.FromEvent<FileSystemEventHandler, FileSystemEventArgs>(
                                                h => (_, e) => h(e),
                                                h => watcher.Changed += h,
                                                h => watcher.Changed -= h)
                                            .Where(args => args.ChangeType == WatcherChangeTypes.Changed)
                                            .Delay(TimeSpan.FromMilliseconds(100))
                                            .ObserveOnDispatcher(DispatcherPriority.Background)
                                            .Subscribe(args => this.OnFileChanged(args.FullPath)),

                                  Observable.FromEvent<FileSystemEventHandler, FileSystemEventArgs>(
                                                h => (_, e) => h(e),
                                                h => watcher.Deleted += h,
                                                h => watcher.Deleted -= h)
                                            .ObserveOnDispatcher(DispatcherPriority.Background)
                                            .Subscribe(args => this.OnFileDeleted(args.FullPath)),

                                  Observable.FromEvent<RenamedEventHandler, RenamedEventArgs>(
                                                h => (_, e) => h(e),
                                                h => watcher.Renamed += h,
                                                h => watcher.Renamed -= h)
                                            .ObserveOnDispatcher(DispatcherPriority.Background)
                                            .Subscribe(args => this.OnFileRenamed(args.OldFullPath, args.FullPath)),
                              };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static RootDirectoryWatcher Instance { get; } = new RootDirectoryWatcher();

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

        private void OnFileChanged(string fileName)
        {
            if (fileName == this.project.CascadeFileName)
            {
                this.CascadeFile = fileName;
            }

            if (fileName == this.project.InfoFileName)
            {
                File.Delete(this.project.VecFileName);
            }

            if (fileName.StartsWith(this.project.Negatives.Path))
            {
                this.project.Negatives.Update(fileName);
                this.project.SaveNegativesIndex();
            }
        }

        private void OnFileDeleted(string fileName)
        {
            if (fileName == this.project.CascadeFileName)
            {
                this.CascadeFile = null;
            }

            if (fileName == this.project.InfoFileName)
            {
                File.Delete(this.project.VecFileName);
            }
        }

        private void OnFileRenamed(string oldName, string newName)
        {
            if (newName == this.project.CascadeFileName)
            {
                this.CascadeFile = newName;
            }

            if (oldName == this.project.CascadeFileName)
            {
                this.CascadeFile = null;
            }

            if (oldName == this.project.InfoFileName)
            {
                File.Delete(this.project.VecFileName);
            }
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