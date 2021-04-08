using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace JPEG.ImageMatrix
{
    public class Matrix
    {
        public readonly byte [,] FirstColorChannel;
        public readonly byte [,] SecondColorChannel;
        public readonly byte [,] ThirdColorChannel;
        public int Height { get; }
        public int Width{  get; }

        public Matrix(int bmpHeight, int bmpWidth)
        {
            var remainsHeight = bmpHeight % 8;
            var remainsWidth = bmpWidth % 8;
            Height = remainsHeight != 0 ? bmpHeight + 8 - remainsHeight : bmpHeight;
            Width = remainsWidth != 0 ? bmpWidth + 8 - remainsWidth : bmpWidth;
            FirstColorChannel = new byte[Height, Width];
            SecondColorChannel = new byte[Height, Width];
            ThirdColorChannel = new byte[Height, Width];
        }

        public static unsafe explicit operator Matrix(Bitmap bmp)
        {
            var originalBmpHeight = bmp.Height;
            var originalBmpWidth = bmp.Width;
            var matrix = new Matrix(originalBmpHeight, originalBmpWidth);
            var bmpData = bmp.LockBits(new Rectangle(0, 0, originalBmpWidth, originalBmpHeight), ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb);
            Parallel.For(0, originalBmpHeight, j =>
            {
                var index = (byte*) bmpData.Scan0 + j * bmpData.Stride;
                for (var i = 0; i < originalBmpWidth; i++)
                {
                    var b = *index++;
                    var g = *index++;
                    var r = *index++;
                    matrix.FirstColorChannel[j, i] = r;
                    matrix.SecondColorChannel[j, i] = g;
                    matrix.ThirdColorChannel[j, i] = b;
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
            var bmpData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly,
                PixelFormat.Format24bppRgb);
            Parallel.For(0, height, j =>
            {
                var index = (byte*) bmpData.Scan0 + j * bmpData.Stride;
                for (var i = 0; i < width; i++)
                {
                    var first = matrix.FirstColorChannel[j, i];
                    var second = matrix.SecondColorChannel[j, i];
                    var third = matrix.ThirdColorChannel[j, i];
                    var color = Color.FromArgb(first, second, third);
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