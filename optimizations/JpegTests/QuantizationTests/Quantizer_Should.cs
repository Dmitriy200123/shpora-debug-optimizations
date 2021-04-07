using FluentAssertions;
using JPEG.Quantization;
using NUnit.Framework;

namespace JpegTests.QuantizationTests
{
    [TestFixture]
    public class Quantizer_Should
    {
        private float[,] _firstInputForQuantize;
        private float[,] _secondInputForQuantize;
        
        private byte[,] _inputForDeQuantize;

        [SetUp]
        public void Setup()
        {
            _firstInputForQuantize = new float[,]
            {
                {2, 1, 1, 1, 2, 2, 2, 2}, {2, 1, 1, 1, 2, 2, 2, 2},
                {2, 1, 1, 1, 2, 2, 2, 2}, {2, 1, 1, 1, 2, 2, 2, 2},
                {2, 1, 1, 1, 2, 2, 2, 2}, {2, 1, 1, 1, 2, 2, 2, 2},
                {2, 1, 1, 1, 2, 2, 2, 2}, {2, 1, 1, 1, 2, 2, 2, 2}
            };

            _secondInputForQuantize = new float[,]
            {
                {252, 250, 175, 300, 258, 301, 210, 55}, {22, 241, 175, 30, 28, 11, 20, 35},
                {252, 250, 175, 300, 258, 301, 210, 55}, {252, 250, 175, 300, 258, 301, 210, 55},
                {252, 250, 175, 300, 258, 301, 210, 55}, {252, 250, 175, 300, 258, 301, 210, 55},
                {252, 250, 175, 300, 258, 301, 210, 55}, {252, 250, 175, 300, 258, 301, 210, 55}
            };

            _inputForDeQuantize = new byte[,]
            {
                {1, 3, 2, 1, 2, 1, 1, 1}, {1, 1, 1, 1, 2, 1, 1, 2},
                {1, 2, 1, 1, 1, 1, 2, 1}, {1, 3, 1, 1, 1, 1, 1, 1},
                {1, 1, 2, 3, 1, 1, 2, 1}, {1, 1, 1, 1, 1, 3, 1, 1},
                {1, 5, 1, 1, 2, 1, 1, 2}, {1, 1, 1, 3, 1, 1, 2, 1}
            };
        }

        [Test]
        public void Quantize_WidthAndHeightEqualEight_WhenSimpleEightToEightArray()
        {
            var act = Quantizer.Quantize(_firstInputForQuantize);

            act.GetLength(0).Should().Be(8);
            act.GetLength(1).Should().Be(8);
        }

        [Test]
        public void Quantize_EightToEightArrayWithOnlyZero_WhenEightToEightArrayWithSmallNumbers()
        {
            var act = Quantizer.Quantize(_firstInputForQuantize);
            foreach (var e in act)
                e.Should().Be(0);
        }

        [Test]
        public void Quantize_QuantizedCoefficients_WhenEightToEightArray()
        {
            var act = Quantizer.Quantize(_secondInputForQuantize);

            act.Should().BeEquivalentTo(new byte[,]
            {
                {25, 35, 29, 30, 18, 12, 6, 1},
                {3, 34, 21, 2, 1, 0, 0, 1},
                {31, 31, 17, 21, 10, 8, 5, 1},
                {31, 25, 13, 17, 8, 5, 4, 1},
                {22, 19, 7, 8, 6, 4, 3, 1},
                {18, 11, 5, 7, 5, 4, 3, 1},
                {8, 6, 3, 5, 4, 4, 2, 0},
                {5, 4, 3, 5, 3, 5, 3, 0}
            });
        }
        
        [Test]
        [Timeout(10)]
        public void Quantize_VeryFast_WhenFiveHundredArrays()
        {
            for (var i = 0; i < 500; i++)
            {
                Quantizer.Quantize(_secondInputForQuantize);
            }
        }
        
        [Test]
        public void DeQuantize_WidthAndHeightEqualEight_WhenSimpleEightToEightArray()
        {
            var act = Quantizer.DeQuantize(_inputForDeQuantize);

            act.GetLength(0).Should().Be(8);
            act.GetLength(1).Should().Be(8);
        }
        
        [Test]
        public void DeQuantize_DeQuantizedCoefficients_WhenEightToEightArray()
        {
            var act = Quantizer.DeQuantize(_inputForDeQuantize);

            act.Should().BeEquivalentTo(new float[,]
            {
                {10, 21, 12, 10, 28, 24, 31, 37},
                {7, 7, 8, 11, 32, 35, 36, 66},
                {8, 16, 10, 14, 24, 34, 82, 34},
                {8, 30, 13, 17, 31, 52, 48, 37},
                {11, 13, 44, 102, 41, 65, 124, 46},
                {14, 21, 33, 38, 49, 186, 68, 55},
                {29, 190, 47, 52, 124, 73, 72, 122},
                {43, 55, 57, 177, 67, 60, 124, 59}
            });
        }
        
        [Test]
        [Timeout(10)]
        public void DeQuantize_VeryFast_WhenFiveHundredArrays()
        {
            for (var i = 0; i < 500; i++)
            {
                Quantizer.DeQuantize(_inputForDeQuantize);
            }
        }
        
        
    }
}