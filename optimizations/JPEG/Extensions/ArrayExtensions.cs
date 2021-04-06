using System.Collections.Generic;
using JPEG.HuffmanCoding.CodeTree;

namespace JPEG.Extensions
{
    public static class ArrayExtensions
    {
        public static (int posFirstMin, int posSecondMin) FindTwoPositionsOfMinimums(this List<HuffmanNode> elements)
        {
            var firstMin = int.MaxValue;
            var secondMin = int.MaxValue;
            var posFirstMin = -1;
            var posSecondMin = -1;
            for (var i = 0; i < elements.Count; i++)
            {
                if (elements[i].Frequency < firstMin)
                {
                    secondMin = firstMin;
                    firstMin = elements[i].Frequency;
                    posSecondMin = posFirstMin;
                    posFirstMin = i;
                }
                else if (elements[i].Frequency < secondMin)
                {
                    secondMin = elements[i].Frequency;
                    posSecondMin = i;
                }
            }

            return (posFirstMin, posSecondMin);
        }
    }
}