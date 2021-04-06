using System.Drawing;
using System.Drawing.Imaging;

namespace JPEG.Images
{
    class Matrix
    {
        public readonly Pixel[,] Pixels;
        public readonly int Height;
        public readonly int Width;

        public Matrix(int height, int width)
        {
            Height = height;
            Width = width;
            Pixels = new Pixel[height, width];
        }

        public static unsafe explicit operator Matrix(Bitmap bmp)
        {
            var height = bmp.Height - bmp.Height % 8;
            var width = bmp.Width - bmp.Width % 8;
            var matrix = new Matrix(height, width);
            var data = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            for (var j = 0; j < height; j++)
            {
                var curpos = ((byte*) data.Scan0) + j * data.Stride;
                for (var i = 0; i < width; i++)
                {
                    var b = *(curpos++);
                    var g = *(curpos++);
                    var r = *(curpos++);
                    matrix.Pixels[j, i] = new Pixel(r, g, b, PixelFormat.RGB);
                }
            }

            bmp.UnlockBits(data);

            return matrix;
        }

        public static unsafe explicit operator Bitmap(Matrix matrix)
        {
            var bmp = new Bitmap(matrix.Width, matrix.Height);
            var data = bmp.LockBits(new Rectangle(0, 0, matrix.Width, matrix.Height), ImageLockMode.WriteOnly,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            for (var j = 0; j < bmp.Height; j++)
            {
                var curpos = ((byte*) data.Scan0) + j * data.Stride;
                for (var i = 0; i < bmp.Width; i++)
                {
                    var pixel = matrix.Pixels[j, i];
                    var color = Color.FromArgb(ToByte(pixel.R), ToByte(pixel.G), ToByte(pixel.B));
                    *(curpos++) = color.B;
                    *(curpos++) = color.G;
                    *(curpos++) = color.R;
                }
            }

            bmp.UnlockBits(data);
            return bmp;
        }

        public static int ToByte(double d)
        {
            var val = (int) d;
            if (val > byte.MaxValue)
                return byte.MaxValue;
            if (val < byte.MinValue)
                return byte.MinValue;
            return val;
        }
    }
}