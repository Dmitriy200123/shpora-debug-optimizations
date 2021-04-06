using System;
using System.Threading.Tasks;
using JPEG.DiscreteCosineTransform;
using JPEG.ImageConversion;

namespace JPEG.Quantization
{
    public static class Quantizer
    {
        private static readonly int[,] QuantizationMatrix = GetQuantizationMatrix(Compressor.CompressionQuality);
        private const int Size = DCT.Size;

        public static byte[,] Quantize(float[,] channelFreqs)
        {
            var result = new byte[Size, Size];
            for (var y = 0; y < Size; y++)
            {
                for (var x = 0; x < Size; x++)
                {
                    result[y, x] = (byte) (channelFreqs[y, x] / QuantizationMatrix[y, x]);
                }
            }

            return result;
        }

        public static float[,] DeQuantize(byte[,] quantizedBytes)
        {
            var result = new float[Size, Size];
            for (var y = 0; y < Size; y++)
            {
                for (var x = 0; x < Size; x++)
                {
                    result[y, x] =
                        (sbyte) quantizedBytes[y, x] *
                        QuantizationMatrix[y, x]; //NOTE cast to sbyte not to loose negative numbers
                }
            }

            return result;
        }

        private static int[,] GetQuantizationMatrix(int quality)
        {
            if (quality < 1 || quality > 99)
                throw new ArgumentException("quality must be in [1,99] interval");

            var multiplier = quality < 50 ? 5000 / quality : 200 - 2 * quality;

            var result = new[,]
            {
                {16, 11, 10, 16, 24, 40, 51, 61},
                {12, 12, 14, 19, 26, 58, 60, 55},
                {14, 13, 16, 24, 40, 57, 69, 56},
                {14, 17, 22, 29, 51, 87, 80, 62},
                {18, 22, 37, 56, 68, 109, 103, 77},
                {24, 35, 55, 64, 81, 104, 113, 92},
                {49, 64, 78, 87, 103, 121, 120, 101},
                {72, 92, 95, 98, 112, 100, 103, 99}
            };

            Parallel.For(0, Size, y =>
            {
                for (var x = 0; x < Size; x++)
                {
                    result[y, x] = (multiplier * result[y, x] + 50) / 100;
                }
            });
            return result;
        }
    }
}