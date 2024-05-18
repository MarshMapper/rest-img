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
            return HashCode.Combine(imagePath, transformRequest.Width, transformRequest.Height);
        }
        public SKData? Get(string imagePath, TransformRequest transformRequest)
        {
            int cacheKey = GetCacheKey(imagePath, transformRequest);
            byte[]? imageBytes;
            bool isCached = _memoryCache.TryGetValue(cacheKey, out imageBytes);
            if (isCached)
            {
                return SKData.CreateCopy(imageBytes);
            }
            return null;
        }
        public void Set(string imagePath, TransformRequest transformRequest, SKData imageData)
        {
            int cacheKey = GetCacheKey(imagePath, transformRequest);
            _memoryCache.Set(cacheKey, imageData.ToArray());
        }
    }
}
