namespace OpenCVSharpTrainer
{
    using System.Net.Mime;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private TrainingViewModel ViewModel => (TrainingViewModel)this.DataContext;

        private void OnCanSave(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !string.IsNullOrEmpty(this.ViewModel.InfoFileName);
            e.Handled = true;
        }

        private void OnSave(object sender, ExecutedRoutedEventArgs e)
        {
            this.ViewModel.SavePositives(this.ViewModel.InfoFileName);
        }

        private void OnCanSaveAs(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ViewModel.Positives.Count > 0;
            e.Handled = true;
        }

        private void OnSaveAs(object sender, ExecutedRoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void OnOpen(object sender, ExecutedRoutedEventArgs e)
        {
            var openFileDialog = new Ookii.Dialogs.Wpf.VistaOpenFileDialog();
            if (openFileDialog.ShowDialog(this) == true)
            {
                if (openFileDialog.FileName.EndsWith(".info"))
                {
                    this.ViewModel.InfoFileName = openFileDialog.FileName;
                }
                else
                {
                    this.ViewModel.ImageFileName = openFileDialog.FileName;
                }
            }
        }

        private void OnAdd(object sender, ExecutedRoutedEventArgs e)
        {
            var image = (Image)e.OriginalSource;
            var p = Mouse.GetPosition(image);
            var w = this.ViewModel.Width;
            var h = this.ViewModel.Height;
            this.ViewModel.Positives.Add(new RectangleInfo((int)p.X - (w / 2), (int)(image.ActualHeight - p.Y - (h / 2)), w, h));
            e.Handled = true;
        }
    }
}
