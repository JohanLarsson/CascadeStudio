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
            var rectangle = new RectangleInfo((int)p.X - (w / 2), (int)(p.Y - (h / 2)), w, h);
            this.ViewModel.Rectangles.Add(rectangle);
            e.Handled = true;
        }

        private void Rectangle_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) != 0 &&
                e.Delta != 0)
            {
                var rectangle = (RectangleInfo)((Button)sender).DataContext;
                if (e.Delta > 0)
                {
                    rectangle.IncreaseSizeCommand.Execute(null);
                }
                else
                {
                    rectangle.DecreaseSizeCommand.Execute(null);
                }

                e.Handled = true;
            }
        }
    }
}
