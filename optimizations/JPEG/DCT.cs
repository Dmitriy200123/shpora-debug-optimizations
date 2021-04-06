using System;

namespace JPEG
{
	public class DCT
	{
		private static readonly double AlphaByZero = 1 / Math.Sqrt(2);

		public static double[,] DCT2D(double[,] input,  double[] coefficientsForDCT)
		{
			var height = input.GetLength(0);
			var width = input.GetLength(1);
			var coeffs = new double[width, height];
			var beta = 1d / width + 1d / height;
			var iteration = 0;
			for (var i = 0; i < width; i++)
			{
				for (var j = 0; j < height; j++)
				{
					var sum = 0d;
					for (var x = 0; x < width; x++)
					for (var y = 0; y < height; y++)
						sum += input[x, y] * coefficientsForDCT[iteration++];
					coeffs[i, j] = sum * beta * (i == 0 ? AlphaByZero : 1) * (j == 0 ? AlphaByZero : 1);
				}
			}

			return coeffs;
		}
		
		public static double[,] IDCT2D(double[,] coeffs, double[] coefficientsForDCT)
		{
			var width = coeffs.GetLength(1);
			var height = coeffs.GetLength(0);
			var beta = 1d / width + 1d / height;
			var i = 0;
			var output = new double[width, height];
			for(var x = 0; x < width; x++)
			{
				for(var y = 0; y < height; y++)
				{
					var sum = 0d;
					for (var u = 0; u < width; u++)
					{
						for (var v = 0; v < height; v++)
						{
							sum += coeffs[u, v] * coefficientsForDCT[i++] *
							       (u == 0 ? AlphaByZero : 1) * (v == 0 ? AlphaByZero : 1);
						}
					}
					output[x, y] = sum * beta;
				}
			}

			return output;
		}

		internal static double[] GetDCTCoefficients(int height, int width)
		{
			var output = new double[width * width * height * height];
			var i = 0;
			for(var u = 0; u < width; u++)
			{
				for(var v = 0; v < height; v++)
				{
					for (var x = 0; x < width; x++)
					{
						for (var y = 0; y < height; y++)
						{
							var b = Math.Cos((2d * x + 1d) * u * Math.PI / (2 * width));
							var c = Math.Cos((2d * y + 1d) * v * Math.PI / (2 * height));
							output[i++] = b * c;
						}
					}
				}
			}
			

			return output;
		}
		
		internal static double[] GetIDCTCoefficients(int height, int width)
		{
			var output = new double[width * width * height * height];
			var i = 0;
			for(var x = 0; x < width; x++)
			{
				for(var y = 0; y < height; y++)
				{
					for (var u = 0; u < width; u++)
					{
						for (var v = 0; v < height; v++)
						{
							var b = Math.Cos((2d * x + 1d) * u * Math.PI / (2 * width));
							var c = Math.Cos((2d * y + 1d) * v * Math.PI / (2 * height));
							output[i++] = b * c;
						}
					}
				}
			}
			

			return output;
		}
	}
}