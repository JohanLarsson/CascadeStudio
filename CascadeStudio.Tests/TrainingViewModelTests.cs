namespace CascadeStudio.Tests
{
    using NUnit.Framework;

    public class TrainingViewModelTests
    {
        [Test]
        public void UpdatesPositivesWhenInfoFileNameChanges()
        {
            Assert.Fail();
            ////var imageFileName = FullFileName("Foo.bmp");
            ////File.WriteAllText(imageFileName, string.Empty);
            ////var vm = new PositiveViewModel();
            ////vm.Project.ImageFileName = imageFileName;
            ////var text = $"Foo.bmp 2 0 0 24 24 100 100 24 24\r\n" +
            ////           $"Bar.bmp 2 1 1 24 24 101 101 24 24\r\n";
            ////var infoFileName = FullFileName("positives.info");
            ////File.WriteAllText(infoFileName, text);
            ////vm.Project.InfoFileName = infoFileName;
            ////var expected = new[]
            ////{
            ////    new RectangleInfo { X = 0, Y = 0, Width = 24, Height = 24 },
            ////    new RectangleInfo { X = 100, Y = 100, Width = 24, Height = 24 },
            ////};

            ////CollectionAssert.AreEqual(expected, vm.Positives, RectangleInfoComparer.Default);
        }

        [Test]
        public void SavesPositives()
        {
            Assert.Fail();
            ////var imageFileName = FullFileName("Foo.bmp");
            ////File.WriteAllText(imageFileName, string.Empty);
            ////var vm = new ProjectViewModel();
            ////vm.ImageFileName = imageFileName;
            ////var text = $"Foo.bmp 2 0 0 24 24 100 100 24 24\r\n" +
            ////           $"Bar.bmp 2 1 1 24 24 101 101 24 24\r\n";
            ////var infoFileName = FullFileName("positives.info");
            ////File.WriteAllText(infoFileName, text);
            ////vm.InfoFileName = infoFileName;
            ////vm.Positives[0].X = 1;
            ////vm.Positives[0].Y = 2;
            ////vm.Positives[0].Width = 3;
            ////vm.Positives[0].Height = 4;
            ////vm.Positives.Add(new RectangleInfo(5, 6, 7, 8));
            ////vm.SaveInfo(new FileInfo(infoFileName));
            ////var actual = File.ReadAllText(infoFileName);
            ////var expected = $"Foo.bmp 3 1 2 3 4 100 100 24 24 5 6 7 8\r\n" +
            ////               $"Bar.bmp 2 1 1 24 24 101 101 24 24\r\n";
            ////Assert.AreEqual(expected, actual);
        }

        private static string FullFileName(string name)
        {
            return System.IO.Path.Combine(TestContext.CurrentContext.TestDirectory, name);
        }
    }
}
