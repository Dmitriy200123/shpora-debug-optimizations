using FluentAssertions;
using JPEG.DiscreteCosineTransform;
using NUnit.Framework;

namespace JpegTests.DiscreteCosineTransformTests
{
    [TestFixture]
    public class IDCT_Should
    {
        private float[,] _input;
        
        [SetUp]
        public void Setup()
        {
            _input = new[,]
            {
                {2, 1, 1, 1, 2, 2, 2, 0.25f}, {2, 1.4f, 4.5f, 6.3f, 7, 5, 3, 0.25f},
                {2, 1.4f, 4.5f, 6.3f, 7, 5, 3, 0.25f}, {2, 1.4f, 4.5f, 6.3f, 7, 5, 3, 0.25f},
                {2, 1.4f, 4.5f, 6.3f, 7, 5, 3, 0.25f}, {2, 1.4f, 4.5f, 6.3f, 7, 5, 3, 0.25f},
                {2, 1.4f, 4.5f, 6.3f, 7, 5, 3, 0.25f}, {2, 1.4f, 4.5f, 6.3f, 7, 5, 3, 0.25f},
            };
        }
        
        [Test]
        public void DCT_EightToEightArray_WhenEightToEightArray()
        {
            var act = DCT.IDCT2D(_input, -10);

            act.GetLength(0).Should().Be(8);
            act.GetLength(1).Should().Be(8);
        }
        
        [Test]
        [Timeout(10)]
        public void DCT_VeryFast_WhenFiveHundredArrays()
        {
            for (var i = 0; i < 500; i++)
            {
                DCT.IDCT2D(_input, -10);
            }
        }
    }
}