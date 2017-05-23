namespace OpenCVSharpTrainer.Tests
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using NUnit.Framework;

    [Explicit("Script")]
    public class SquaresAndCircles
    {
        private static readonly string WorkingDirectory =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "SquaresAndCircles");

        private static readonly string CreateSamplesAppFileName =
            @"C:\Program Files\opencv\build\x64\vc14\bin\opencv_createsamples.exe";

        private static readonly string TrainCascadeAppFileName =
            @"C:\Program Files\opencv\build\x64\vc14\bin\opencv_traincascade.exe";

        [Test]
        public void Train()
        {
            var vec = Path.Combine(WorkingDirectory, "positives.vec");
            using (var process = Process.Start(
                new ProcessStartInfo
                {
                    FileName = CreateSamplesAppFileName,
                    Arguments =
                        $"-info {Path.Combine(WorkingDirectory, "positives.info")} -vec {vec} -w 24 -h 24 -num 100",
                }))
            {
                process.WaitForExit();
            }

            Directory.CreateDirectory(Path.Combine(WorkingDirectory, "data"));
            using (var process = Process.Start(
                new ProcessStartInfo
                {
                    FileName = TrainCascadeAppFileName,
                    WorkingDirectory = WorkingDirectory,
                    Arguments =
                        $"-data data -vec {vec} -bg bg.txt -numPos 100 -numNeg 100 -w 24 -h 24 -featureType HAAR",
                }))
            {
            }
        }

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
                        ////graphics.ScaleTransform((float)(rnd.Next(6, 10) / 10.0), (float)(rnd.Next(6, 10) / 10.0));
                        ////graphics.RotateTransform(rnd.Next(0, 90));
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

            File.WriteAllText(
                Path.Combine(WorkingDirectory, "positives.info"),
                string.Join(
                    Environment.NewLine,
                    Directory.EnumerateFiles(dir).Select(f => $"Positives\\{Path.GetFileName(f)} 1 0 0 24 24")));
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
                        ////graphics.ScaleTransform((float)(rnd.Next(6, 10) / 10.0), (float)(rnd.Next(6, 10) / 10.0));
                        ////graphics.RotateTransform(rnd.Next(0, 90));
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

            File.WriteAllText(
                Path.Combine(WorkingDirectory, "bg.txt"),
                string.Join(
                    Environment.NewLine,
                    Directory.EnumerateFiles(dir).Select(f => $"Negatives\\{Path.GetFileName(f)}")));
        }

        [Test]
        public void DumpValidationImage()
        {
            var dir = Path.Combine(WorkingDirectory, "Validation");
            Directory.CreateDirectory(dir);

            using (var image = new Bitmap(24 * 10, 24 * 10))
            {
                using (var graphics = Graphics.FromImage(image))
                {
                    var rnd = new Random();
                    graphics.Clear(Color.FromArgb(255, rnd.Next(0, 20), rnd.Next(0, 20), rnd.Next(0, 20)));
                    for (var x = 0; x < 10; x++)
                    {
                        for (var y = 0; y < 10; y++)
                        {
                            var rect = new Rectangle((24 * x) + 1, (24 * y) + 1, 22, 22);
                            var brush = new SolidBrush(
                                Color.FromArgb(
                                    255,
                                    rnd.Next(25, 255),
                                    rnd.Next(25, 255),
                                    rnd.Next(25, 255)));
                            switch (rnd.Next(0,10))
                            {
                                case 0:
                                    graphics.FillEllipse(
                                        brush,
                                        rect);
                                    break;
                                case 1:
                                    graphics.FillRectangle(
                                        brush,
                                        rect);
                                    break;
                            }
                        }
                    }

                    var fileName = Path.Combine(dir, $"Meh.bmp");
                    image.Save(fileName, ImageFormat.Bmp);
                }
            }
        }
    }
}
