namespace CascadeStudio
{
    using System;
    using System.Collections.Generic;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Text;

    public static class Filters
    {
        static Filters()
        {
            // https://www.codeproject.com/Tips/255626/A-FileDialog-Filter-generator-for-all-supported-im
            var openImageBuilder = new StringBuilder();
            var separator = string.Empty;
            var codecs = ImageCodecInfo.GetImageEncoders();
            var images = new Dictionary<string, string>();
            foreach (var codec in codecs)
            {
                openImageBuilder.Append(separator);
                openImageBuilder.Append(codec.FilenameExtension);
                ImageExtensions.UnionWith(codec.FilenameExtension.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.TrimStart('*')));
                separator = ";";
                images.Add($"{codec.FormatDescription} Files: ({codec.FilenameExtension})", codec.FilenameExtension);
            }

            var sb = new StringBuilder();
            if (openImageBuilder.Length > 0)
            {
                sb.AppendFormat("{0}|{1}", "All Images", openImageBuilder);
            }

            images.Add("All Files", "*.*");
            foreach (var image in images)
            {
                sb.AppendFormat("|{0}|{1}", image.Key, image.Value);
            }

            OpenImageFilter = sb.ToString();
        }

        public static HashSet<string> ImageExtensions { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public static string OpenImageFilter { get; }

        public static bool IsImageFile(string fileName)
        {
            return ImageExtensions.Any(ext => fileName.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
        }
    }
}