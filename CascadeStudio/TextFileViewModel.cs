namespace CascadeStudio
{
    using System.IO;

    public class TextFileViewModel
    {
        public TextFileViewModel(string fileName)
        {
            this.FileName = fileName;
            this.Text = File.ReadAllText(fileName);
        }

        public string FileName { get; }

        public string Text { get; }

        public string Name => System.IO.Path.GetFileName(this.FileName);
    }
}