using System;

namespace JPEG
{
    public class DCT
    {
        private static readonly double AlphaByZero = 1 / Math.Sqrt(2);
        private static readonly double[] DCTCoefficients = GetCoefficientsForDCTOrIDCT(Size, Size, true);
        private static readonly double[] IDCTCoefficients = GetCoefficientsForDCTOrIDCT(Size, Size, false);

        public const int Size = 8;

        public static double[,] DCT2D(double[,] input)
        {
            var matrix = new double[Size, Size];
            var beta = 2 * 1f / Size;
            var iteration = 0;
            for (var i = 0; i < Size; i++)
            {
                for (var j = 0; j < Size; j++)
                {
                    var sum = 0d;
                    for (var x = 0; x < Size; x++)
                    for (var y = 0; y < Size; y++)
                        sum += input[x, y] * DCTCoefficients[iteration++];
                    matrix[i, j] = sum * beta * (i == 0 ? AlphaByZero : 1) * (j == 0 ? AlphaByZero : 1);
                }
            }

            return matrix;
        }

        public static double[,] IDCT2D(double[,] frequencyChannel, int shift)
        {
            var beta = 2 * 1f / Size;
            var i = 0;
            var output = new double[Size, Size];
            for (var x = 0; x < Size; x++)
            {
                for (var y = 0; y < Size; y++)
                {
                    var sum = 0d;
                    for (var u = 0; u < Size; u++)
                    {
                        for (var v = 0; v < Size; v++)
                        {
                            sum += frequencyChannel[u, v] * IDCTCoefficients[i++] *
                                   (u == 0 ? AlphaByZero : 1) * (v == 0 ? AlphaByZero : 1);
                        }
                    }

                    output[x, y] = sum * beta + shift;
                }
            }

            return output;
        }

        private static double[] GetCoefficientsForDCTOrIDCT(int height, int width, bool isDCT)
        {
            var output = new double[width * width * height * height];
            var i = 0;
            for (var u = 0; u < width; u++)
            {
                for (var v = 0; v < height; v++)
                {
                    for (var x = 0; x < width; x++)
                    {
                        for (var y = 0; y < height; y++)
                        {
                            var b = Convert.ToSingle(Math.Cos((2d * (isDCT ? x : u) + 1d) * (isDCT ? u : x) * Math.PI /
                                                              (2 * width)));
                            var c = Convert.ToSingle(Math.Cos((2d * (isDCT ? y : v) + 1d) * (isDCT ? v : y) * Math.PI /
                                                              (2 * height)));
                            output[i++] = b * c;
                        }
                    }
                }
            }


            return output;
        }
    }
}