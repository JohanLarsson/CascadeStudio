namespace CascadeStudio
{
    using System.Collections.Generic;
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

        public static IEqualityComparer<RectangleInfo> Comparer { get; } = new RectangleInfoEqualityComparer();

        public int X
        {
            get => this.x;

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
            get => this.y;

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
            get => this.width;

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
            get => this.height;

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
            var coords = Regex.Match(rect, @"(?<x>\-?\d+) (?<y>\-?\d+) (?<w>\d+) (?<h>\d+)");
            return new RectangleInfo(
                int.Parse(coords.Groups["x"].Value),
                int.Parse(coords.Groups["y"].Value),
                int.Parse(coords.Groups["w"].Value),
                int.Parse(coords.Groups["h"].Value));
        }

        public void IncreaseSize()
        {
            this.x--;
            this.y--;
            this.width += 2;
            this.height += 2;
            this.OnPropertyChanged(nameof(this.X));
            this.OnPropertyChanged(nameof(this.Y));
            this.OnPropertyChanged(nameof(this.Width));
            this.OnPropertyChanged(nameof(this.Height));
        }

        public void DecreaseSize()
        {
            this.x++;
            this.y++;
            this.width -= 2;
            this.height -= 2;
            this.OnPropertyChanged(nameof(this.X));
            this.OnPropertyChanged(nameof(this.Y));
            this.OnPropertyChanged(nameof(this.Width));
            this.OnPropertyChanged(nameof(this.Height));
        }

        public override string ToString()
        {
            return $"{nameof(this.X)}: {this.X}, {nameof(this.Y)}: {this.Y}, {nameof(this.Width)}: {this.Width}, {nameof(this.Height)}: {this.Height}";
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private sealed class RectangleInfoEqualityComparer : IEqualityComparer<RectangleInfo>
        {
            public bool Equals(RectangleInfo x, RectangleInfo y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                if (ReferenceEquals(x, null))
                {
                    return false;
                }

                if (ReferenceEquals(y, null))
                {
                    return false;
                }

                if (x.GetType() != y.GetType())
                {
                    return false;
                }

                return x.x == y.x && x.y == y.y && x.width == y.width && x.height == y.height;
            }

            public int GetHashCode(RectangleInfo obj)
            {
                unchecked
                {
                    var hashCode = obj.x;
                    hashCode = (hashCode * 397) ^ obj.y;
                    hashCode = (hashCode * 397) ^ obj.width;
                    hashCode = (hashCode * 397) ^ obj.height;
                    return hashCode;
                }
            }
        }
    }
}