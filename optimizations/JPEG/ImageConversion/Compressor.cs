using System.Threading.Tasks;
using JPEG.ColorSubsampling;
using JPEG.DiscreteCosineTransform;
using JPEG.HuffmanCoding;
using JPEG.ImageLoadAndSave;
using JPEG.ImageMatrix;
using JPEG.Quantization;
using JPEG.ZigZagScan;

namespace JPEG.ImageConversion
{
    public static class Compressor
    {
        public const int CompressionQuality = 70;
        private const int DctSize = DCT.Size;
        private const int QuantizedSize = DCT.Size * DctSize;

        public static CompressedImage Compress(Matrix matrix)
        {
            var width = matrix.Width / DctSize;
            var height = matrix.Height / DctSize;
            var allQuantizedBytes = new byte[height * width * 3 * DctSize * DctSize];
            Parallel.For(0, height, y =>
            {
                Parallel.For(0, width, x =>
                {
                    var subMatrices = new float[3][,];
                    for (var i = 0; i < 3; i++)
                    {
                        var subMatrix =
                            Subsampling.GetSubMatrix(matrix, y * DctSize, DctSize, x * DctSize, DctSize, i, -128);
                        subMatrices[i] = subMatrix;
                        subMatrices[i] = DCT.DCT2D(subMatrices[i]);
                        var quantizedFreqs = Quantizer.Quantize(subMatrices[i]);
                        var quantizedBytes = ZigZagScanning.Scan(quantizedFreqs);
                        var index = y * width * 3 * QuantizedSize + x * 3 * QuantizedSize + i * QuantizedSize;
                        foreach (var e in quantizedBytes)
                            allQuantizedBytes[index++] = e;
                    }
                });
            });
            var compressedBytes =
                HuffmanCodec.Encode(allQuantizedBytes, out var decodeTable, out var bitsCount);

            return new CompressedImage
            {
                Quality = CompressionQuality, CompressedBytes = compressedBytes, BitsCount = bitsCount,
                DecodeTable = decodeTable, Height = matrix.Height, Width = matrix.Width
            };
        }
    }
}