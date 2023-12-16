using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;
using JPEG.ImageConversion;
using JPEG.ImageLoadAndSave;
using JPEG.ImageMatrix;

namespace JPEG
{
    static class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine(IntPtr.Size == 8 ? "64-bit version" : "32-bit version");
                var sw = Stopwatch.StartNew();
                var fileName = @"sample.bmp";
//				var fileName = "Big_Black_River_Railroad_Bridge.bmp";
                var compressedFileName = fileName + ".compressed." + Compressor.CompressionQuality;
                var uncompressedFileName = fileName + ".uncompressed." + Compressor.CompressionQuality + ".bmp";
                using (var fileStream = File.OpenRead(fileName))
                using (var bmp = (Bitmap) Image.FromStream(fileStream, false, false))
                {
                    var imageMatrix = (Matrix) bmp;

                    sw.Stop();
                    Console.WriteLine(
                        $"{imageMatrix.Width}x{imageMatrix.Height} - {fileStream.Length / (1024.0 * 1024):F2} MB");
                    sw.Start();

                    var compressionResult = Compressor.Compress(imageMatrix);
                    compressionResult.Save(compressedFileName);
                }

                sw.Stop();
                Console.WriteLine("Compression: " + sw.Elapsed);
                sw.Restart();
                var compressedImage = CompressedImage.Load(compressedFileName);
                var uncompressedImage = UnCompressor.Uncompress(compressedImage);
                var resultBmp = (Bitmap) uncompressedImage;
                resultBmp.Save(uncompressedFileName, ImageFormat.Bmp);
                Console.WriteLine("Decompression: " + sw.Elapsed);
                Console.WriteLine($"Peak commit size: {MemoryMeter.PeakPrivateBytes() / (1024.0 * 1024):F2} MB");
                Console.WriteLine($"Peak working set: {MemoryMeter.PeakWorkingSet() / (1024.0 * 1024):F2} MB");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}