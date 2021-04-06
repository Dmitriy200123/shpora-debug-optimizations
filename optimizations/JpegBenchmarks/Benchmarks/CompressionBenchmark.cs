using System.Drawing;
using System.IO;
using BenchmarkDotNet.Attributes;
using JPEG.ImageConversion;
using JPEG.ImageLoadAndSave;
using JPEG.ImageMatrix;
using JpegBenchmarks.PreviousVersionAlgorithms.CompressAndUncompressImage;

namespace JpegBenchmarks.Benchmarks
{
    [DisassemblyDiagnoser()]
    public class CompressionBenchmark
    {
        private Matrix _matrixForNewVersion;
        private PreviousVersionAlgorithms.MatrixAndPixels.Matrix _matrixForOldVersion;
        private Bitmap _bmp;
        private CompressedImage _compressedImageForNewVersion;
        private PreviousVersionAlgorithms.LoadAndSave.CompressedImage _compressedImageForOldVersion;

        [GlobalSetup]
        public void SetUp()
        {
            using (var fileStream = File.OpenRead(Path.Combine(
                @"C:\Users\79321\Desktop\optimization-shpora\optimizations\JpegBenchmarks\bin\Release\netcoreapp2.2",
                "sample.bmp")))
            {
                _bmp = (Bitmap) Image.FromStream(fileStream, false, false);
                _matrixForNewVersion = (Matrix) _bmp;
                _matrixForOldVersion = (PreviousVersionAlgorithms.MatrixAndPixels.Matrix) _bmp;
                _compressedImageForNewVersion = Compressor.Compress(_matrixForNewVersion);
                _compressedImageForOldVersion = Compression.Compress(_matrixForOldVersion);
            }
        }

        [GlobalCleanup]
        public void CleanUp()
        {
            _bmp.Dispose();
        }

        [Benchmark]
        public void CompressNewVersion()
        {
            Compressor.Compress(_matrixForNewVersion);
        }

        [Benchmark]
        public void CompressOldVersion()
        {
            Compression.Compress(_matrixForOldVersion);
        }

        [Benchmark]
        public void UncompressNewVersion()
        {
            UnCompressor.Uncompress(_compressedImageForNewVersion);
        }

        [Benchmark]
        public void UncompressOldVersion()
        {
            Compression.Uncompress(_compressedImageForOldVersion);
        }
    }
}