namespace OpenCVSharpTrainer.Tests
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using NUnit.Framework;

    public class SquaresAndCircles
    {
        private static readonly string WorkingDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "SquaresAndCircles");

        [Test]
        public void DumpSquares()
        {
            var dir = Path.Combine(WorkingDirectory, "Positives");
            Directory.CreateDirectory(dir);
            for (var i = 0; i < 100; i++)
            {
                using (var image = new Bitmap(24, 24))
                {
                    using (var graphics = Graphics.FromImage(image))
                    {
                        var rnd = new Random();
                        graphics.Clear(Color.FromArgb(255, rnd.Next(0, 20), rnd.Next(0, 20), rnd.Next(0, 20)));
                        //graphics.ScaleTransform((float)(rnd.Next(6, 10) / 10.0), (float)(rnd.Next(6, 10) / 10.0));
                        //graphics.RotateTransform(rnd.Next(0, 90));
                        graphics.FillRectangle(
                            new SolidBrush(
                                Color.FromArgb(
                                    255,
                                    rnd.Next(25, 255),
                                    rnd.Next(25, 255),
                                    rnd.Next(25, 255))),
                            new Rectangle(1, 1, 22, 22));
                        var fileName = Path.Combine(dir, $"{i}.bmp");
                        image.Save(fileName, ImageFormat.Bmp);
                    }
                }
            }

            File.WriteAllText(Path.Combine(WorkingDirectory, "positives.info"), string.Join(Environment.NewLine, Directory.EnumerateFiles(dir).Select(f => $"Positives\\{Path.GetFileName(f)} 1 0 0 24 24")));
        }

        [Test]
        public void DumpCircles()
        {
            var dir = Path.Combine(WorkingDirectory, "Negatives");
            Directory.CreateDirectory(dir);
            for (var i = 0; i < 100; i++)
            {
                using (var image = new Bitmap(24, 24))
                {
                    using (var graphics = Graphics.FromImage(image))
                    {
                        var rnd = new Random();
                        graphics.Clear(Color.FromArgb(255, rnd.Next(0, 20), rnd.Next(0, 20), rnd.Next(0, 20)));
                        //graphics.ScaleTransform((float)(rnd.Next(6, 10) / 10.0), (float)(rnd.Next(6, 10) / 10.0));
                        //graphics.RotateTransform(rnd.Next(0, 90));
                        graphics.FillEllipse(
                            new SolidBrush(
                                Color.FromArgb(
                                    255,
                                    rnd.Next(25, 255),
                                    rnd.Next(25, 255),
                                    rnd.Next(25, 255))),
                            new Rectangle(1, 1, 22, 22));
                        var fileName = Path.Combine(dir, $"{i}.bmp");
                        image.Save(fileName, ImageFormat.Bmp);
                    }
                }
            }

            File.WriteAllText(Path.Combine(WorkingDirectory, "bg.txt"), string.Join(Environment.NewLine, Directory.EnumerateFiles(dir).Select(f => $"Negatives\\{Path.GetFileName(f)}")));
        }
    }
}
