namespace CascadeStudio
{
    using System;

    public sealed class MainViewModel : IDisposable
    {
        private bool disposed;

        public ProjectViewModel Project { get; } = new ProjectViewModel();

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            this.Project.Dispose();
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
