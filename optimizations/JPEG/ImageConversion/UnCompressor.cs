using System.IO;
using System.Threading.Tasks;
using JPEG.DiscreteCosineTransform;
using JPEG.Extensions;
using JPEG.HuffmanCoding;
using JPEG.ImageLoadAndSave;
using JPEG.ImageMatrix;
using JPEG.Quantization;
using JPEG.ZigZagScan;

namespace JPEG.ImageConversion
{
    public static class UnCompressor
    {
        private const int DctSize = DCT.Size;
        private const int QuantizedSize = DctSize * DctSize;

        public static Matrix Uncompress(CompressedImage image)
        {
            var width = image.Width / DctSize;
            var height = image.Height / DctSize;
            var result = new Matrix(image.Height, image.Width);
            using (var allQuantizedBytes =
                new MemoryStream(HuffmanCodec.Decode(image.CompressedBytes, image.DecodeTable, image.BitsCount)))
            {
                Parallel.For(0, height, y =>
                {
                    Parallel.For(0, width, x =>
                    {
                        var fregChannels = new float[3][,];
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

                            var quantizedFreqs = ZigZagScanning.UnScan(quantizedBytes);
                            var channelFreqs = Quantizer.DeQuantize(quantizedFreqs);
                            fregChannels[i] = channelFreqs;
                            var idctResult = DCT.IDCT2D(fregChannels[i], 128);
                            fregChannels[i] = idctResult;
                        }

                        result.SetPixels(fregChannels[0], fregChannels[1], fregChannels[2], y * DctSize, x * DctSize);
                    });
                });
            }

            return result;
        }
    }
}