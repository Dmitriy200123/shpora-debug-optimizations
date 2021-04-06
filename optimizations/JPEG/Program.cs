using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using JPEG.Images;
using JPEG.Utilities;

namespace JPEG
{
	class Program
	{
		public const int CompressionQuality = 70;
		private const int QuantizedSize = DCTSize * DCTSize;

		static void Main(string[] args)
		{
			try
			{
				Console.WriteLine(IntPtr.Size == 8 ? "64-bit version" : "32-bit version");
				var sw = Stopwatch.StartNew();
				var fileName = @"sample.bmp";
//				var fileName = "Big_Black_River_Railroad_Bridge.bmp";
				var compressedFileName = fileName + ".compressed." + CompressionQuality;
				var uncompressedFileName = fileName + ".uncompressed." + CompressionQuality + ".bmp";
				using (var fileStream = File.OpenRead(fileName))
				using (var bmp = (Bitmap) Image.FromStream(fileStream, false, false))
				{
					var imageMatrix = (Matrix) bmp;

					sw.Stop();
					Console.WriteLine($"{bmp.Width}x{bmp.Height} - {fileStream.Length / (1024.0 * 1024):F2} MB");
					sw.Start();

					var compressionResult = Compress(imageMatrix);
					compressionResult.Save(compressedFileName);
				}

				sw.Stop();
				Console.WriteLine("Compression: " + sw.Elapsed);
				sw.Restart();
				var compressedImage = CompressedImage.Load(compressedFileName);
				var uncompressedImage = Uncompress(compressedImage);
				var resultBmp = (Bitmap) uncompressedImage;
				resultBmp.Save(uncompressedFileName, ImageFormat.Bmp);
				Console.WriteLine("Decompression: " + sw.Elapsed);
				Console.WriteLine($"Peak commit size: {MemoryMeter.PeakPrivateBytes() / (1024.0*1024):F2} MB");
				Console.WriteLine($"Peak working set: {MemoryMeter.PeakWorkingSet() / (1024.0*1024):F2} MB");
			}
			catch(Exception e)
			{
				Console.WriteLine(e);
			}
		}

		public static CompressedImage Compress(Matrix matrix)
		{
			var width = matrix.Width / DCTSize;
			var height = matrix.Height / DCTSize;
			var allQuantizedBytes = new byte[height * width * 3 * DCTSize * DCTSize];
			Parallel.For(0, height, y =>
			{
				Parallel.For(0, width, x =>
				{
					var subMatrices = new double[3][,];
					for (var i = 0; i < 3; i++)
					{
						var subMatrix =
							GetSubMatrix(matrix, y * DCTSize, DCTSize, x * DCTSize, DCTSize, i, -128);
						subMatrices[i] = subMatrix;
						subMatrices[i] = DCT.DCT2D(subMatrices[i]);
						var quantizedFreqs = Quantize(subMatrices[i]);
						var quantizedBytes = ZigZagScan(quantizedFreqs);
						var index = y * width * 3 * QuantizedSize + x * 3 * QuantizedSize + i * QuantizedSize;
						foreach (var e in quantizedBytes)
							allQuantizedBytes[index++] = e;
					}
				});
			});
			var compressedBytes =
				HuffmanCodec.Encode(allQuantizedBytes.ToList(), out var decodeTable, out var bitsCount);

			return new CompressedImage
			{
				Quality = CompressionQuality, CompressedBytes = compressedBytes, BitsCount = bitsCount,
				DecodeTable = decodeTable, Height = matrix.Height, Width = matrix.Width
			};
		}
		
		public static Matrix Uncompress(CompressedImage image)
		{
			var width = image.Width / DCTSize;
			var height = image.Height / DCTSize;
			var result = new Matrix(image.Height, image.Width);
			using (var allQuantizedBytes =
				new MemoryStream(HuffmanCodec.Decode(image.CompressedBytes, image.DecodeTable, image.BitsCount)))
			{
				Parallel.For(0, height, y =>
				{
					Parallel.For(0, width, x =>
					{
						var fregChannels = new double[3][,];
						var quantizedBytes = new byte[QuantizedSize];
						for (var i = 0; i < 3; i++)
						{
							var offset = y * width * 3 * QuantizedSize +
							             x * 3 * QuantizedSize +
							             i * QuantizedSize;
							lock (allQuantizedBytes)
							{
								allQuantizedBytes.Seek(offset, SeekOrigin.Begin);
								allQuantizedBytes.Read(quantizedBytes, 0, QuantizedSize);
							}

							var quantizedFreqs = ZigZagUnScan(quantizedBytes);
							var channelFreqs = DeQuantize(quantizedFreqs);
							fregChannels[i] = channelFreqs;
							var idctResult = DCT.IDCT2D(fregChannels[i], 128);
							fregChannels[i] = idctResult;
						}

						result.SetPixels(fregChannels[0], fregChannels[1], fregChannels[2], y * DCTSize, x * DCTSize);
					});
				});
			}

			return result;
		}

		private static void ShiftMatrixValues(double[,] subMatrix, int shiftValue)
		{
			var height = subMatrix.GetLength(0);
			var width = subMatrix.GetLength(1);
			
			for(var y = 0; y < height; y++)
				for(var x = 0; x < width; x++)
					subMatrix[y, x] = subMatrix[y, x] + shiftValue;
		}

		private static double[,] GetSubMatrix(Matrix matrix, int yOffset, int yLength, int xOffset, int xLength,
			int channelNumber, int shift)
		{
			var result = new double[yLength, xLength];
			var isBrightness = channelNumber == 0;
			var width = isBrightness ? xLength : xLength - 1;
			var height = isBrightness ? yLength : yLength - 1;
			var offset = isBrightness ? 1 : 2;
			for (var j = 0; j < height; j += offset)
			for (var i = 0; i < width; i += offset)
			{
				var (first, second, third) = matrix.ColorChannels[yOffset + j, xOffset + i];
				result[j, i] = (channelNumber == 0 ? first : channelNumber == 1 ? second : third) + shift;
				if (isBrightness) continue;
				result[j + 1, i] = result[j, i];
				result[j, i + 1] = result[j, i];
				result[j + 1, i + 1] = result[j, i];
			}

			return result;
		}

		private static IEnumerable<byte> ZigZagScan(byte[,] channelFreqs)
		{
			return new[]
			{
				channelFreqs[0, 0], channelFreqs[0, 1], channelFreqs[1, 0], channelFreqs[2, 0], channelFreqs[1, 1], channelFreqs[0, 2], channelFreqs[0, 3], channelFreqs[1, 2],
				channelFreqs[2, 1], channelFreqs[3, 0], channelFreqs[4, 0], channelFreqs[3, 1], channelFreqs[2, 2], channelFreqs[1, 3],  channelFreqs[0, 4], channelFreqs[0, 5],
				channelFreqs[1, 4], channelFreqs[2, 3], channelFreqs[3, 2], channelFreqs[4, 1], channelFreqs[5, 0], channelFreqs[6, 0], channelFreqs[5, 1], channelFreqs[4, 2],
				channelFreqs[3, 3], channelFreqs[2, 4], channelFreqs[1, 5],  channelFreqs[0, 6], channelFreqs[0, 7], channelFreqs[1, 6], channelFreqs[2, 5], channelFreqs[3, 4],
				channelFreqs[4, 3], channelFreqs[5, 2], channelFreqs[6, 1], channelFreqs[7, 0], channelFreqs[7, 1], channelFreqs[6, 2], channelFreqs[5, 3], channelFreqs[4, 4],
				channelFreqs[3, 5], channelFreqs[2, 6], channelFreqs[1, 7], channelFreqs[2, 7], channelFreqs[3, 6], channelFreqs[4, 5], channelFreqs[5, 4], channelFreqs[6, 3],
				channelFreqs[7, 2], channelFreqs[7, 3], channelFreqs[6, 4], channelFreqs[5, 5], channelFreqs[4, 6], channelFreqs[3, 7], channelFreqs[4, 7], channelFreqs[5, 6],
				channelFreqs[6, 5], channelFreqs[7, 4], channelFreqs[7, 5], channelFreqs[6, 6], channelFreqs[5, 7], channelFreqs[6, 7], channelFreqs[7, 6], channelFreqs[7, 7]
			};
		}

		private static byte[,] ZigZagUnScan(IReadOnlyList<byte> quantizedBytes)
		{
			return new[,]
			{
				{ quantizedBytes[0], quantizedBytes[1], quantizedBytes[5], quantizedBytes[6], quantizedBytes[14], quantizedBytes[15], quantizedBytes[27], quantizedBytes[28] },
				{ quantizedBytes[2], quantizedBytes[4], quantizedBytes[7], quantizedBytes[13], quantizedBytes[16], quantizedBytes[26], quantizedBytes[29], quantizedBytes[42] },
				{ quantizedBytes[3], quantizedBytes[8], quantizedBytes[12], quantizedBytes[17], quantizedBytes[25], quantizedBytes[30], quantizedBytes[41], quantizedBytes[43] },
				{ quantizedBytes[9], quantizedBytes[11], quantizedBytes[18], quantizedBytes[24], quantizedBytes[31], quantizedBytes[40], quantizedBytes[44], quantizedBytes[53] },
				{ quantizedBytes[10], quantizedBytes[19], quantizedBytes[23], quantizedBytes[32], quantizedBytes[39], quantizedBytes[45], quantizedBytes[52], quantizedBytes[54] },
				{ quantizedBytes[20], quantizedBytes[22], quantizedBytes[33], quantizedBytes[38], quantizedBytes[46], quantizedBytes[51], quantizedBytes[55], quantizedBytes[60] },
				{ quantizedBytes[21], quantizedBytes[34], quantizedBytes[37], quantizedBytes[47], quantizedBytes[50], quantizedBytes[56], quantizedBytes[59], quantizedBytes[61] },
				{ quantizedBytes[35], quantizedBytes[36], quantizedBytes[48], quantizedBytes[49], quantizedBytes[57], quantizedBytes[58], quantizedBytes[62], quantizedBytes[63] }
			};
		}

		private static byte[,] Quantize(double[,] channelFreqs)
		{
			var width = channelFreqs.GetLength(0);
			var height = channelFreqs.GetLength(1);
			//refactor
			var result = new byte[width, height];
			
			for(int y = 0; y < width; y++)
			{
				for(int x = 0; x < height; x++)
				{
					result[y, x] = (byte)(channelFreqs[y, x] / QuantizationMatrix[y, x]);
				}
			}

			return result;
		}

		private static double[,] DeQuantize(byte[,] quantizedBytes)
		{
			var width = quantizedBytes.GetLength(0);
			var height =  quantizedBytes.GetLength(1);
			//refactor
			var result = new double[width, height];
			for(int y = 0; y < width; y++)
			{
				for(int x = 0; x < height; x++)
				{
					result[y, x] = (sbyte)quantizedBytes[y, x] * QuantizationMatrix[y, x];//NOTE cast to sbyte not to loose negative numbers
				}
			}

			return result;
		}

		private static int[,] GetQuantizationMatrix(int quality)
		{
			if(quality < 1 || quality > 99)
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
			//refactor
			var width = result.GetLength(0);
			var height = result.GetLength(1);

			for(var y = 0; y < width; y++)
			{
				for(var x = 0; x < height; x++)
				{
					result[y, x] = (multiplier * result[y, x] + 50) / 100;
				}
			}
			return result;
		}

		private static readonly int[,] QuantizationMatrix = GetQuantizationMatrix(CompressionQuality);
		const int DCTSize = 8;
	}
}
