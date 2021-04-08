using System;
using FluentAssertions;
using JPEG.ColorSubsampling;
using JPEG.Extensions;
using JPEG.ImageMatrix;
using NUnit.Framework;

namespace JpegTests.ColorSubsampling
{
    [TestFixture]
    public class Subsampling_Should
    {
        private Matrix _matrix;
        private Matrix _matrixWithOnlyZero;
        
        private static readonly float[,] ColorChannel = {
            {1, 3, 0, 1, 2, 1, 3, 1}, {1, 1, 0, 1, 1, 1, 1, 1}, {1, 1, 0, 1, 1, 1, 1, 1}, {0, 0, 0, 1, 1, 1, 0, 0},
            {1, 1, 0, 1, 1, 1, 1, 1}, {1, 1, 0, 1, 1, 1, 1, 1}, {1, 1, 0, 1, 1, 1, 1, 1}, {1, 0, 0, 0, 0, 0, 0, 0}
        };
        
        [SetUp]
        public void Setup()
        {
            _matrix = new Matrix(8, 8);
            _matrix.SetPixels(ColorChannel, ColorChannel, ColorChannel, 0, 0);
            _matrixWithOnlyZero = new Matrix(8, 8);
        }

        [Test]
        public void Subsampling_WidthAndHeightEqualEight_WhenSimpleEightToEightMatrix()
        {
            var act = Subsampling.GetSubMatrix(_matrix, 0, 8, 0, 8, 0, 0);

            act.GetLength(0).Should().Be(8);
            act.GetLength(1).Should().Be(8);
        }
        
        [Test]
        public void Subsampling_SubsamplingCoefficients_WhenSimpleEightToEightMatrixAndBrightnessChannel()
        {
            var act = Subsampling.GetSubMatrix(_matrix, 0, 8, 0, 8, 0, 0);

            act.Should().BeEquivalentTo(new float[,]
            {
                {1, 3, 0, 1, 2, 1, 3, 1}, {1, 1, 0, 1, 1, 1, 1, 1}, {1, 1, 0, 1, 1, 1, 1, 1}, {0, 0, 0, 1, 1, 1, 0, 0},
                {1, 1, 0, 1, 1, 1, 1, 1}, {1, 1, 0, 1, 1, 1, 1, 1}, {1, 1, 0, 1, 1, 1, 1, 1}, {1, 0, 0, 0, 0, 0, 0, 0}
            });
        }
        
        [TestCase(1)]
        [TestCase(-1)]
        [TestCase(0)]
        public void Subsampling_SubsamplingCoefficientsWithOnlyShift_WhenSimpleEightToEightMatrix(int shift)
        {
            var act = Subsampling.GetSubMatrix(_matrixWithOnlyZero, 0, 8, 0, 8, 0, shift);

            foreach (var e in act)
                e.Should().Be(shift);
        }

        [Test]
        public void Subsampling_SubsamplingCoefficients_WhenSimpleEightToEightMatrixAndCbChannel()
        {
            var act = Subsampling.GetSubMatrix(_matrix, 0, 8, 0, 8, 1, 0);

            act.Should().BeEquivalentTo(new float[,]
            {
                {1, 1, 0, 0, 2, 2, 3, 3}, {1, 1, 0, 0, 2, 2, 3, 3}, {1, 1, 0, 0, 1, 1, 1, 1}, {1, 1, 0, 0, 1, 1, 1, 1},
                {1, 1, 0, 0, 1, 1, 1, 1}, {1, 1, 0, 0, 1, 1, 1, 1}, {1, 1, 0, 0, 1, 1, 1, 1}, {1, 1, 0, 0, 1, 1, 1, 1}
            });
        }
        
        [Test]
        public void Subsampling_SubsamplingCoefficients_WhenSimpleEightToEightMatrixAndCrChannel()
        {
            var act = Subsampling.GetSubMatrix(_matrix, 0, 8, 0, 8, 2, 0);

            act.Should().BeEquivalentTo(new float[,]
            {
                {1, 1, 0, 0, 2, 2, 3, 3}, {1, 1, 0, 0, 2, 2, 3, 3}, {1, 1, 0, 0, 1, 1, 1, 1}, {1, 1, 0, 0, 1, 1, 1, 1},
                {1, 1, 0, 0, 1, 1, 1, 1}, {1, 1, 0, 0, 1, 1, 1, 1}, {1, 1, 0, 0, 1, 1, 1, 1}, {1, 1, 0, 0, 1, 1, 1, 1}
            });
        }
        
        [Test]
        [Timeout(10)]
        public void Subsampling_VeryFast_WhenSimpleFiveHundredCalls()
        {
            var random = new Random();
            for (var i = 0; i < 500; i++)
                Subsampling.GetSubMatrix(_matrix, 0, 8, 0, 8, random.Next(0, 2), 0);
        }
    }
}