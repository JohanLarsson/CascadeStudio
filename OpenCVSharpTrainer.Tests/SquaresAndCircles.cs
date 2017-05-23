namespace OpenCVSharpTrainer.Tests
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using NUnit.Framework;
    using OpenCvSharp;

    [Explicit("Script")]
    public class SquaresAndCircles
    {
        private const int NumPos = 10;
        private const int NumNeg = 10;

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
                    Arguments = $"-info {Path.Combine(WorkingDirectory, "positives.info")} -vec {vec} -w 24 -h 24 -num {NumPos}",
                }))
            {
                process.WaitForExit();
            }

            Directory.CreateDirectory(Path.Combine(WorkingDirectory, "data"));
            using (Process.Start(
                new ProcessStartInfo
                {
                    FileName = TrainCascadeAppFileName,
                    WorkingDirectory = WorkingDirectory,
                    Arguments = $"-data data -vec {vec} -bg bg.txt -numPos {NumPos} -numNeg {NumNeg} -w 24 -h 24 -featureType HAAR",
                }))
            {
            }
        }

        [Test]
        public void DumpSquares()
        {
            var dir = Path.Combine(WorkingDirectory, "Positives");
            Directory.CreateDirectory(dir);
            var rnd = new Random();
            for (var i = 0; i < NumPos; i++)
            {
                using (var mat = new Mat(new Size(24, 24), MatType.CV_8U, new Scalar(255)))
                {
                    Cv2.Rectangle(mat, new Rect(1, 1, 22, 22), new Scalar(rnd.Next(20, 128)));
                    mat.SaveImage(Path.Combine(dir, $"{i}.bmp"));
                }
            }

            File.WriteAllText(
                Path.Combine(WorkingDirectory, "positives.info"),
                string.Join(
                    Environment.NewLine,
                    Directory.EnumerateFiles(dir).Select(f => $"Positives\\{Path.GetFileName(f)} 1 1 1 22 22")));
        }

        [Test]
        public void DumpCircles()
        {
            var dir = Path.Combine(WorkingDirectory, "Negatives");
            Directory.CreateDirectory(dir);
            var rnd = new Random();
            for (var i = 0; i < NumNeg; i++)
            {
                using (var mat = new Mat(new Size(24 * 3, 24 * 3), MatType.CV_8U, new Scalar(255)))
                {
                    Cv2.Circle(mat, 36, 36, 11, new Scalar(rnd.Next(20, 128)));
                    mat.SaveImage(Path.Combine(dir, $"{i}.bmp"));
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
            var rnd = new Random();
            using (var image = new Mat(new Size(24 * 10, 24 * 10), MatType.CV_8U, new Scalar(255)))
            {
                for (var x = 0; x < 10; x++)
                {
                    for (var y = 0; y < 10; y++)
                    {
                        switch (rnd.Next(0, 10))
                        {
                            case 0:
                                Cv2.Circle(image, (24 * x) + 12, (24 * y) + 12, 11, new Scalar(rnd.Next(20, 128)));
                                break;
                            case 1:
                                Cv2.Rectangle(image, new Rect((24 * x) + 1, (24 * y) + 1, 22, 22), new Scalar(rnd.Next(20, 128)));
                                break;
                        }
                    }

                    image.SaveImage(Path.Combine(dir, $"Meh.bmp"));
                }
            }
        }
    }
}
