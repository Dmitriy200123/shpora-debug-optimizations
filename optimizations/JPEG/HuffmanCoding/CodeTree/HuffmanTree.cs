using System.Collections.Generic;
using System.Linq;
using JPEG.Extensions;

namespace JPEG.HuffmanCoding.CodeTree
{
    public static class HuffmanTree
    {
        private static readonly int[] ByteEnumerable = Enumerable.Range(0, byte.MaxValue + 1).ToArray();

        public static HuffmanNode BuildHuffmanTree(int[] frequencies)
        {
            var nodes = GetNodes(frequencies);
            while (nodes.Count > 1)
            {
                var (posFirstMin, posSecondMin) = nodes.FindTwoPositionsOfMinimums();
                var firstMin = nodes[posFirstMin];
                var secondMin = nodes[posSecondMin];
                nodes.RemoveAt(posFirstMin);
                nodes.RemoveAt(posFirstMin < posSecondMin ? posSecondMin - 1 : posSecondMin);
                nodes.Add(new HuffmanNode
                    {Frequency = firstMin.Frequency + secondMin.Frequency, Left = secondMin, Right = firstMin});
            }

            return nodes.First();
        }

        private static List<HuffmanNode> GetNodes(int[] frequencies)
        {
            return ByteEnumerable
                .Select(num => new HuffmanNode {Frequency = frequencies[num], LeafLabel = (byte) num})
                .Where(node => node.Frequency > 0)
                .ToList();
        }
    }
}