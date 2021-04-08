using JPEG.ImageMatrix;

namespace JPEG.ColorSubsampling
{
    public static class Subsampling
    {
        public static float[,] GetSubMatrix(Matrix matrix, int yOffset, int yLength, int xOffset, int xLength,
            int channelNumber, int shift)
        {
            var result = new float[yLength, xLength];
            var isBrightness = channelNumber == 0;
            var width = isBrightness ? xLength : xLength - 1;
            var height = isBrightness ? yLength : yLength - 1;
            var offset = isBrightness ? 1 : 2;
            for (var j = 0; j < height; j += offset)
            for (var i = 0; i < width; i += offset)
            {
                result[j, i] = (channelNumber == 0 ? matrix.ColorChannels[yOffset + j, xOffset + i].first :
                    channelNumber == 1 ? matrix.ColorChannels[yOffset + j, xOffset + i].second :
                    matrix.ColorChannels[yOffset + j, xOffset + i].third) + shift;
                if (isBrightness) continue;
                result[j + 1, i] = result[j, i];
                result[j, i + 1] = result[j, i];
                result[j + 1, i + 1] = result[j, i];
            }

            return result;
        }
    }
}