using JPEG.DiscreteCosineTransform;
using JPEG.ImageMatrix;

namespace JPEG.Extensions
{
    public static class MatrixExtensions
    {
        private const int Size = DCT.Size;

        public static void SetPixels(this Matrix matrix, float[,] a, float[,] b, float[,] c,
            int yOffset, int xOffset)
        {
            for (var y = 0; y < Size; y++)
            for (var x = 0; x < Size; x++)
            {
                matrix.ColorChannels[yOffset + y, xOffset + x] = ((byte) a[y, x], (byte) b[y, x], (byte) c[y, x]);
            }
        }
    }
}