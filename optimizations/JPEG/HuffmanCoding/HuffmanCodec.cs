using System.Collections.Generic;
using System.Threading.Tasks;
using JPEG.HuffmanCoding.Buffer;
using JPEG.HuffmanCoding.CodeTree;

namespace JPEG.HuffmanCoding
{
    public static class HuffmanCodec
    {
        public static byte[] Encode(byte[] data, out Dictionary<BitsWithLength, byte> decodeTable,
            out long bitsCount)
        {
            var frequencies = CalculateFrequencies(data);

            var root = HuffmanTree.BuildHuffmanTree(frequencies);

            var encodeTable = new BitsWithLength[byte.MaxValue + 1];
            FillEncodeTable(root, encodeTable);

            var bitsBuffer = new BitsBuffer();
            foreach (var b in data)
                bitsBuffer.Add(encodeTable[b]);

            decodeTable = CreateDecodeTable(encodeTable);

            return bitsBuffer.ToArray(out bitsCount);
        }

        public static byte[] Decode(byte[] encodedData, Dictionary<BitsWithLength, byte> decodeTable, long bitsCount)
        {
            var result = new List<byte>();

            var sample = new BitsWithLength {Bits = 0, BitsCount = 0};
            for (var byteNum = 0; byteNum < encodedData.Length; byteNum++)
            {
                var b = encodedData[byteNum];
                for (var bitNum = 0; bitNum < 8 && byteNum * 8 + bitNum < bitsCount; bitNum++)
                {
                    sample.Bits = (sample.Bits << 1) + ((b & (1 << (8 - bitNum - 1))) != 0 ? 1 : 0);
                    sample.BitsCount++;

                    if (!decodeTable.TryGetValue(sample, out var decodedByte)) continue;
                    result.Add(decodedByte);

                    sample.BitsCount = 0;
                    sample.Bits = 0;
                }
            }

            return result.ToArray();
        }

        private static Dictionary<BitsWithLength, byte> CreateDecodeTable(BitsWithLength[] encodeTable)
        {
            var result = new Dictionary<BitsWithLength, byte>(new BitsWithLength.Comparer());
            for (var b = 0; b < encodeTable.Length; b++)
            {
                var bitsWithLength = encodeTable[b];
                if (bitsWithLength != null)
                    result[bitsWithLength] = (byte) b;
            }

            return result;
        }

        private static void FillEncodeTable(HuffmanNode node, BitsWithLength[] encodeSubstitutionTable,
            int bitvector = 0, int depth = 0)
        {
            while (true)
            {
                if (node.LeafLabel != null)
                    encodeSubstitutionTable[node.LeafLabel.Value] =
                        new BitsWithLength {Bits = bitvector, BitsCount = depth};
                else
                {
                    if (node.Left != null)
                    {
                        FillEncodeTable(node.Left, encodeSubstitutionTable, (bitvector << 1) + 1, depth + 1);
                        node = node.Right;
                        bitvector = (bitvector << 1) + 0;
                        depth += 1;
                        continue;
                    }
                }

                break;
            }
        }

        private static int[] CalculateFrequencies(byte[] data)
        {
            var result = new int[byte.MaxValue + 1];
            Parallel.ForEach(data, element => { result[element]++; });
            return result;
        }
    }
}