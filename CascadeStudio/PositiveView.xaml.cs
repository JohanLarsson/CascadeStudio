namespace CascadeStudio
{
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public partial class PositiveView : UserControl
    {
        public PositiveView()
        {
            this.InitializeComponent();
        }

        private TrainingViewModel ViewModel => (TrainingViewModel)this.DataContext;

        private void OnCanSave(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !string.IsNullOrEmpty(this.ViewModel.Project.InfoFileName);
            e.Handled = true;
        }

        private void OnSave(object sender, ExecutedRoutedEventArgs e)
        {
            this.ViewModel.SaveInfo(new FileInfo(this.ViewModel.Project.InfoFileName));
        }

        private void OnCanSaveAs(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ViewModel.Positives.Count > 0;
            e.Handled = true;
        }

        private void OnSaveAs(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaSaveFileDialog();
            if (dialog.ShowDialog(Window.GetWindow(this)) == true)
            {
                if (dialog.FileName.EndsWith(".info"))
                {
                    this.ViewModel.SaveInfo(new FileInfo(dialog.FileName));
                    this.ViewModel.Project.InfoFileName = dialog.FileName;
                }
                else
                {
                    ////this.ViewModel.ImageFileName = openFileDialog.FileName;
                }
            }
        }

        private void OnOpen(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter != null)
            {
                this.ViewModel.Project.ImageFileName = Path.Combine(Path.GetDirectoryName(this.ViewModel.Project.InfoFileName), (string)e.Parameter);
            }
            else
            {
                var dialog = new Ookii.Dialogs.Wpf.VistaOpenFileDialog();
                if (dialog.ShowDialog(Window.GetWindow(this)) == true)
                {
                    if (dialog.FileName.EndsWith(".info"))
                    {
                        this.ViewModel.Project.InfoFileName = dialog.FileName;
                    }
                    else
                    {
                        this.ViewModel.Project.ImageFileName = dialog.FileName;
                    }
                }
            }

            e.Handled = true;
        }

        private void OnCanAdd(object sender, CanExecuteRoutedEventArgs e)
        {
            ////e.CanExecute = !string.IsNullOrWhiteSpace(this.ViewModel.ImageFileName) &&
            ////               !string.IsNullOrWhiteSpace(this.ViewModel.InfoFileName);
            e.CanExecute = true;
            e.Handled = true;
        }

        private void OnAdd(object sender, ExecutedRoutedEventArgs e)
        {
            var image = (Image)e.OriginalSource;
            var p = Mouse.GetPosition(image);
            var w = this.ViewModel.Width;
            var h = this.ViewModel.Height;
            this.ViewModel.Positives.Add(new RectangleInfo((int)p.X - (w / 2), (int)(p.Y - (h / 2)), w, h));
            e.Handled = true;
        }
    }
}
