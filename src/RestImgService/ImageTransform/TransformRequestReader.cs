using Microsoft.AspNetCore.Http;

namespace RestImgService.ImageTransform
{
    public class TransformRequestReader
    {
        public TransformRequestReader()
        {
        }
        public TransformRequest ReadRequest(IQueryCollection queryCollection, int defaultQuality)
        {
            string format = "jpeg";
            int quality = defaultQuality;
            int width = 0;
            int height = 0;

            if (queryCollection.TryGetValue("fmt", out var parameterValue))
            {
                format = parameterValue.ToString();
            }
            if (queryCollection.TryGetValue("q", out parameterValue))
            {
                int.TryParse(parameterValue, out quality);
            }
            if (queryCollection.TryGetValue("w", out parameterValue))
            {
                int.TryParse(parameterValue, out width);
            }
            if (queryCollection.TryGetValue("h", out parameterValue))
            {
                int.TryParse(parameterValue, out height);
            }
            return new TransformRequest(width, height, format, quality);
        }
    }
}
