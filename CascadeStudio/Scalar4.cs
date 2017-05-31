namespace CascadeStudio
{
    using OpenCvSharp;

    public static class Scalar4
    {
        public static Scalar Red => new Scalar(0, 0, 255, 255);

        public static Scalar Green => new Scalar(0, 255, 0, 255);
    }

    public static class MatExt
    {
        public static Mat OverLay(this Mat mat)
        {
            return new Mat(mat.Size(), MatType.CV_8UC4, new Scalar(0, 0, 0, 0));
        }
    }
}