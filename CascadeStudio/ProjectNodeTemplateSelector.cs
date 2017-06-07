namespace CascadeStudio
{
    using System.Windows;
    using System.Windows.Controls;

    public class ProjectNodeTemplateSelector : DataTemplateSelector
    {
        public DataTemplate PositivesDirectoryTemplate { get; set; }

        public DataTemplate PositiveTemplate { get; set; }

        public DataTemplate NegativesDirectoryTemplate { get; set; }

        public DataTemplate ImageTemplate { get; set; }

        public DataTemplate DataDirectoryTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is PositiveViewModel)
            {
                return this.PositiveTemplate;
            }

            if (item is PositivesDirectory)
            {
                return this.PositivesDirectoryTemplate;
            }

            if (item is DataDirectory)
            {
                return this.DataDirectoryTemplate;
            }

            if (item is ImageViewModel)
            {
                return this.ImageTemplate;
            }

            return base.SelectTemplate(item, container);
        }
    }
}
