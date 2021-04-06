using System.Drawing;
using System.IO;
using BenchmarkDotNet.Attributes;
using JPEG.ImageMatrix;

namespace JpegBenchmarks.Benchmarks
{
    [DisassemblyDiagnoser]
    public class MatrixBenchmark
    {
        private Matrix _matrixForNewVersion;
        private PreviousVersionAlgorithms.MatrixAndPixels.Matrix _matrixForOldVersion;
        private static Bitmap _bmp;
        
        private const string FileName = @"sample.bmp";
        
        [GlobalSetup]
        public void Setup()
        {
            using (var fileStream = File.OpenRead(Path.Combine(@"C:\Users\79321\Desktop\optimization-shpora\optimizations\JpegBenchmarks\bin\Release\netcoreapp2.2", FileName)))
            {
                _bmp = (Bitmap) Image.FromStream(fileStream, false, false);
                _matrixForNewVersion = (Matrix) _bmp;
                _matrixForOldVersion = (PreviousVersionAlgorithms.MatrixAndPixels.Matrix) _bmp;
            }
        }
        
        [GlobalCleanup]
        public void CleanUp()
        {
            _bmp.Dispose();
        }

        [Benchmark]
        public void ConvertMatrixToBmpNewVersion()
        {
            var _ = (Bitmap) _matrixForNewVersion;
        }
        
        [Benchmark]
        public void ConvertMatrixToBmpOldVersion()
        {
            var _ = (Bitmap) _matrixForOldVersion;
        }
        
        [Benchmark]
        public void ConvertBmpToMatrixNewVersion()
        {
            var _ = (Matrix) _bmp;
        }
        
        [Benchmark]
        public void ConvertBmpToMatrixOldVersion()
        {
            var _ = (PreviousVersionAlgorithms.MatrixAndPixels.Matrix) _bmp;
        }
    }
}