using JPEG.DiscreteCosineTransform;
using JPEG.ImageMatrix;

namespace JPEG.Extensions
{
    public static class MatrixExtensions
    {
        private const int Size = DCT.Size;
        private const byte ByteMin = byte.MinValue;
        private const byte ByteMax = byte.MaxValue;

        public static void SetPixels(this Matrix matrix, float[,] a, float[,] b, float[,] c,
            int yOffset, int xOffset)
        {
            for (var y = 0; y < Size; y++)
            for (var x = 0; x < Size; x++)
            {
                var first = a[y, x];
                var second = b[y, x];
                var third = c[y, x];

                matrix.ColorChannels[yOffset + y, xOffset + x] = (
                    first > ByteMax ? ByteMax : first < ByteMin ? ByteMin : (byte) first,
                    second > ByteMax ? ByteMax : second < ByteMin ? ByteMin : (byte) second,
                    third > ByteMax ? ByteMax : third < ByteMin ? ByteMin : (byte) third
                );
            }
        }
    }
}