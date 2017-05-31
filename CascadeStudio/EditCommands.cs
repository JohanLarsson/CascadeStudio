namespace CascadeStudio
{
    using System.Runtime.CompilerServices;
    using System.Windows.Input;

    public static class EditCommands
    {
        public static RoutedUICommand Add { get; } = Create();

        private static RoutedUICommand Create([CallerMemberName] string name = null) => new RoutedUICommand(name, $"{nameof(EditCommands)}.{name}", typeof(EditCommands));
    }
}