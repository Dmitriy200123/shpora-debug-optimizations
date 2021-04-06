using JPEG.Images;

namespace JPEG.Utilities
{
    public static class MatrixExtensions
    {
        private const int Size = 8;
        public static void SetPixels(this Matrix matrix, double[,] a, double[,] b, double[,] c,
            int yOffset, int xOffset)
        {
            for (var y = 0; y < Size; y++)
            for (var x = 0; x < Size; x++)
            {
                matrix.ColorChannels[yOffset + y, xOffset + x] = ((byte)a[y, x], (byte)b[y, x], (byte)c[y, x]);
            }
        }
    }
}