using BenchmarkDotNet.Attributes;
using JPEG.DiscreteCosineTransform;
using JpegBenchmarks.PreviousVersionAlgorithms.DiscreteCosineTransform;

namespace JpegBenchmarks.Benchmarks
{
    [DisassemblyDiagnoser]
    public class DCTBenchmark
    {
        private float[,] _inputForNew;
        private double[,] _inputForPrevious;

        [GlobalSetup]
        public void Setup()
        {
            _inputForNew = new float[8, 8];
            for (var i = 0; i < 8; i++)
            {
                for (var j = 0; j < 8; j++)
                {
                    _inputForNew[i, j] = j * i * 5.5f;
                }
            }

            _inputForPrevious = new double[8, 8];
            for (var i = 0; i < 8; i++)
            {
                for (var j = 0; j < 8; j++)
                {
                    _inputForPrevious[i, j] = j * i * 5.5d;
                }
            }
        }

        [Benchmark]
        public void DCTNewVersion()
        {
            for (var i = 0; i < 500; i++)
            {
                DCT.DCT2D(_inputForNew);
            }
        }

        [Benchmark]
        public void DCTOldVersion()
        {
            for (var i = 0; i < 500; i++)
            {
                DCTPreviousVersion.DCT2D(_inputForPrevious);
            }
        }
        
        [Benchmark]
        public void IDCTNewVersion()
        {
            for (var i = 0; i < 500; i++)
            {
                DCT.IDCT2D(_inputForNew, 8);
            }
        }

        [Benchmark]
        public void IDCTOldVersion()
        {
            for (var i = 0; i < 500; i++)
            {
                DCTPreviousVersion.IDCT2D(_inputForPrevious, new double[8,8]);
            }
        }
    }
}