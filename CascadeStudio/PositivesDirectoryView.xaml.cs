namespace CascadeStudio
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public partial class PositivesDirectoryView : UserControl
    {
        public PositivesDirectoryView()
        {
            this.InitializeComponent();
        }

        private void ImageOnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var scope = (IInputElement)FocusManager.GetFocusScope((DependencyObject)sender);
            Keyboard.Focus((IInputElement)sender);
            FocusManager.SetFocusedElement((DependencyObject)sender, scope);
        }

        private void NextCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ListBox.Items.Count > 0;
            e.Handled = true;
        }

        private void NextExecute(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.ListBox.SelectedIndex < this.ListBox.Items.Count - 1)
            {
                this.ListBox.SetCurrentValue(System.Windows.Controls.Primitives.Selector.SelectedIndexProperty, this.ListBox.SelectedIndex + 1);
            }
            else
            {
                this.ListBox.SetCurrentValue(System.Windows.Controls.Primitives.Selector.SelectedIndexProperty, 0);
            }
        }

        private void PreviousCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ListBox.Items.Count > 0;
            e.Handled = true;
        }

        private void PreviousExecute(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.ListBox.SelectedIndex > 0)
            {
                this.ListBox.SetCurrentValue(System.Windows.Controls.Primitives.Selector.SelectedIndexProperty, this.ListBox.SelectedIndex - 1);
            }
            else
            {
                this.ListBox.SetCurrentValue(System.Windows.Controls.Primitives.Selector.SelectedIndexProperty, this.ListBox.Items.Count - 1);
            }
        }
    }
}
