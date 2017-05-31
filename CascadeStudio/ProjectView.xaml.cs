namespace CascadeStudio
{
    using System.Windows;
    using System.Windows.Controls;

    public partial class ProjectView : UserControl
    {
        public ProjectView()
        {
            this.InitializeComponent();
        }

        private ProjectViewModel Project => (ProjectViewModel)this.DataContext;

        private void OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.Project.SelectedNode = e.NewValue;
        }
    }
}
