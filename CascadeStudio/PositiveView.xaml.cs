namespace CascadeStudio
{
    using System.Windows.Controls;
    using System.Windows.Input;

    public partial class PositiveView : UserControl
    {
        public PositiveView()
        {
            this.InitializeComponent();
        }

        private PositiveViewModel ViewModel => (PositiveViewModel)this.DataContext;

        private void OnAdd(object _, ExecutedRoutedEventArgs e)
        {
            var image = (Image)e.OriginalSource;
            var p = Mouse.GetPosition(image);
            var w = this.ViewModel.Width;
            var h = this.ViewModel.Height;
            this.ViewModel.Rectangles.Add(new RectangleInfo((int)p.X - (w / 2), (int)(p.Y - (h / 2)), w, h));
            e.Handled = true;
        }
    }
}
