using System.Collections.Generic;

namespace JPEG.HuffmanCoding.Buffer
{
    public class BitsBuffer
    {
        private readonly List<byte> _buffer = new List<byte>();
        private readonly BitsWithLength _unfinishedBits = new BitsWithLength();

        public void Add(BitsWithLength bitsWithLength)
        {
            var bitsCount = bitsWithLength.BitsCount;
            var bits = bitsWithLength.Bits;

            var neededBits = 8 - _unfinishedBits.BitsCount;
            while (bitsCount >= neededBits)
            {
                bitsCount -= neededBits;
                _buffer.Add((byte) ((_unfinishedBits.Bits << neededBits) + (bits >> bitsCount)));

                bits &= (1 << bitsCount) - 1;

                _unfinishedBits.Bits = 0;
                _unfinishedBits.BitsCount = 0;

                neededBits = 8;
            }

            _unfinishedBits.BitsCount += bitsCount;
            _unfinishedBits.Bits = (_unfinishedBits.Bits << bitsCount) + bits;
        }

        public byte[] ToArray(out long bitsCount)
        {
            bitsCount = _buffer.Count * 8L + _unfinishedBits.BitsCount;
            var result = new byte[bitsCount / 8 + (bitsCount % 8 > 0 ? 1 : 0)];
            _buffer.CopyTo(result);
            if (_unfinishedBits.BitsCount > 0)
                result[_buffer.Count] = (byte) (_unfinishedBits.Bits << (8 - _unfinishedBits.BitsCount));
            return result;
        }
    }
}