namespace OpenCVSharpTrainer
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;

    public class RectangleInfo : INotifyPropertyChanged
    {
        private int x;
        private int y;
        private int width;
        private int height;

        public RectangleInfo()
        {
        }

        public RectangleInfo(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public int X
        {
            get
            {
                return this.x;
            }

            set
            {
                if (value == this.x)
                {
                    return;
                }

                this.x = value;
                this.OnPropertyChanged();
            }
        }

        public int Y
        {
            get
            {
                return this.y;
            }

            set
            {
                if (value == this.y)
                {
                    return;
                }

                this.y = value;
                this.OnPropertyChanged();
            }
        }

        public int Width
        {
            get
            {
                return this.width;
            }

            set
            {
                if (value == this.width)
                {
                    return;
                }

                this.width = value;
                this.OnPropertyChanged();
            }
        }

        public int Height
        {
            get
            {
                return this.height;
            }

            set
            {
                if (value == this.height)
                {
                    return;
                }

                this.height = value;
                this.OnPropertyChanged();
            }
        }

        public static RectangleInfo Parse(string rect)
        {
            var coords = Regex.Match(rect, @"(?<x>\d+) (?<y>\d+) (?<w>\d+) (?<h>\d+)");
            return new RectangleInfo(
                int.Parse(coords.Groups["x"].Value),
                int.Parse(coords.Groups["y"].Value),
                int.Parse(coords.Groups["w"].Value),
                int.Parse(coords.Groups["h"].Value));
        }

        public override string ToString()
        {
            return $"{nameof(this.X)}: {this.X}, {nameof(this.Y)}: {this.Y}, {nameof(this.Width)}: {this.Width}, {nameof(this.Height)}: {this.Height}";
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}