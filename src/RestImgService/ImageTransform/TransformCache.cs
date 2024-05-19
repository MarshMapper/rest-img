using Microsoft.Extensions.Caching.Memory;
using SkiaSharp;

namespace RestImgService.ImageTransform
{
    public class TransformCache
    {
        private IMemoryCache _memoryCache;

        public TransformCache(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public int GetCacheKey(string imagePath, TransformRequest transformRequest)
        {
            return HashCode.Combine(imagePath, transformRequest.Width, transformRequest.Height,
                transformRequest.Quality, transformRequest.Format);
        }
        public ImageData? Get(string imagePath, TransformRequest transformRequest)
        {
            int cacheKey = GetCacheKey(imagePath, transformRequest);
            byte[]? imageBytes;
            bool isCached = _memoryCache.TryGetValue(cacheKey, out imageBytes);
            if (isCached)
            {
                return new ImageData(SKData.CreateCopy(imageBytes));
            }
            return null;
        }
        public void Set(string imagePath, TransformRequest transformRequest, ImageData imageData)
        {
            int cacheKey = GetCacheKey(imagePath, transformRequest);
            _memoryCache.Set(cacheKey, imageData.ToArray());
        }
    }
}
