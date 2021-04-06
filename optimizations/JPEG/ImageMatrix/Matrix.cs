using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace JPEG.ImageMatrix
{
    public class Matrix
    {
        public readonly (byte first, byte second, byte third)[,] ColorChannels;
        public readonly int Height;
        public readonly int Width;
        public readonly int BmpHeight;
        public readonly int BmpWidth;

        private const byte ByteMax = byte.MaxValue;
        private const byte ByteMin = byte.MinValue;

        public Matrix(int bmpHeight, int bmpWidth)
        {
            BmpHeight = bmpHeight;
            BmpWidth = bmpWidth;
            Height = bmpHeight - bmpHeight % 8;
            Width = bmpWidth - bmpWidth % 8;
            ColorChannels = new (byte first, byte second, byte third)[Height, Width];
        }

        public static unsafe explicit operator Matrix(Bitmap bmp)
        {
            var matrix = new Matrix(bmp.Height, bmp.Width);
            var width = matrix.Width;
            var height = matrix.Height;
            var bmpData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb);
            Parallel.For(0, height, j =>
            {
                var index = (byte*) bmpData.Scan0 + j * bmpData.Stride;
                for (var i = 0; i < width; i++)
                {
                    var b = *index++;
                    var g = *index++;
                    var r = *index++;
                    matrix.ColorChannels[j, i] = (r, g, b);
                }
            });

            bmp.UnlockBits(bmpData);

            return matrix;
        }

        public static unsafe explicit operator Bitmap(Matrix matrix)
        {
            var bmp = new Bitmap(matrix.Width, matrix.Height);
            var width = matrix.Width;
            var height = matrix.Height;
            var bmpWidth = matrix.BmpWidth;
            var bmpHeight = matrix.BmpHeight;
            var bmpData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly,
                PixelFormat.Format24bppRgb);
            Parallel.For(0, bmpHeight, j =>
            {
                var index = (byte*) bmpData.Scan0 + j * bmpData.Stride;
                for (var i = 0; i < bmpWidth; i++)
                {
                    var (first, second, third) = matrix.ColorChannels[j, i];
                    var r = (int) first;
                    var g = (int) second;
                    var b = (int) third;
                    var color = Color.FromArgb(
                        r > ByteMax ? ByteMax : r < ByteMin ? ByteMin : r,
                        g > ByteMax ? ByteMax : g < ByteMin ? ByteMin : g,
                        b > ByteMax ? ByteMax : b < ByteMin ? ByteMin : b
                    );
                    *index++ = color.B;
                    *index++ = color.G;
                    *index++ = color.R;
                }
            });

            bmp.UnlockBits(bmpData);
            return bmp;
        }
    }
}