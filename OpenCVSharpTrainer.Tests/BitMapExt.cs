namespace OpenCVSharpTrainer.Tests
{
    using System.Drawing;

    public static class BitMapExt
    {
        public static void SetGrayScalePalette(this Bitmap bitmap)
        {
            for (var i = 0; i < 256; i++)
            {
                bitmap.Palette.Entries[i] = Color.FromArgb((byte) i, (byte) i, (byte) i);
            }
        }
    }
}