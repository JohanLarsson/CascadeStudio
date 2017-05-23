namespace CascadeDetector
{
    using System.Windows;
    using System.Windows.Input;

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private ViewModel ViewModel => (ViewModel)this.DataContext;

        private void OnOpen(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaOpenFileDialog();
            if (dialog.ShowDialog(Window.GetWindow(this)) == true)
            {
                if (dialog.FileName.EndsWith(".xml"))
                {
                    this.ViewModel.ModelFile = dialog.FileName;
                }
                else
                {
                    this.ViewModel.ImageFile = dialog.FileName;
                }
            }
        }
    }
}
