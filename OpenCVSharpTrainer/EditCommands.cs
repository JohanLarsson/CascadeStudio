namespace OpenCVSharpTrainer
{
    using System.Windows.Input;

    public static class EditCommands
    {
        public static RoutedUICommand Add { get; } = new RoutedUICommand("Add", "EditCommands.Add", typeof(EditCommands));
    }
}