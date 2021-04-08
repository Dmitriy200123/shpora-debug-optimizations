using System.Collections.Generic;
using FluentAssertions;
using JPEG.Extensions;
using JPEG.HuffmanCoding.CodeTree;
using NUnit.Framework;

namespace JpegTests.ArrayExtensionsTests
{
    public class ArrayExtensions_Should
    {
        private List<HuffmanNode> _huffmanNodes;

        [SetUp]
        public void Setup()
        {
            var eightNumbers = new List<int> {1, 2, 5, 6, 7, 3, 1, 0};
            var numbers = new List<int>();
            for (var i = 0; i < 32; i++)
                numbers.AddRange(eightNumbers);
            _huffmanNodes = HuffmanTree.GetNodes(numbers.ToArray());
        }

        [Test]
        public void FindTwoPositionsOfMinimums_TwoMinPositions_WhenSimpleNodesList()
        {
            var act = _huffmanNodes.FindTwoPositionsOfMinimums();

            act.Should().Be((0, 6));
        }
        
        [Test]
        public void FindTwoPositionsOfMinimums_OneMinPositionAndNegativeOne_WhenNodesListWithOneElementNode()
        {
            var nodes = new List<HuffmanNode> {new HuffmanNode {Frequency = 1}};
            
            var act = nodes.FindTwoPositionsOfMinimums();

            act.Should().Be((0, -1));
        }
        
        [Test]
        public void FindTwoPositionsOfMinimums_TwoNegativeOne_WhenEmptyNodesList()
        {
            var nodes = new List<HuffmanNode>();
            
            var act = nodes.FindTwoPositionsOfMinimums();

            act.Should().Be((-1, -1));
        }

        [Test]
        [Timeout(10)]
        public void FindTwoPositionsOfMinimums_VeryFast_WhenFiveHundredCalls()
        {
            for (var i = 0; i < 500; i++)
                _huffmanNodes.FindTwoPositionsOfMinimums();
        }
    }
}