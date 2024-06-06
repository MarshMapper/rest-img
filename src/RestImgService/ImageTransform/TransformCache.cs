using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using SkiaSharp;
using RestImgService.ImageFile;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Options;

namespace RestImgService.ImageTransform
{
    /// <summary>
    /// cache of images that have been transformed.
    /// the cache contains the original images before transformation.  the transformed images are not cached.
    /// .NET OutputCaching will be used to cache the resized images.
    /// </summary>
    public class TransformCache
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<TransformCache> _logger;
        private readonly ImageCacheOptions _options;

        public TransformCache(IMemoryCache memoryCache,
            ILogger<TransformCache> logger,
            IOptions<ImageCacheOptions> options)
        {
            _memoryCache = memoryCache;
            _logger = logger;
            _options = options.Value;
        }
        protected string GetCacheKey(ImagePath imagePath, TransformRequest transformRequest)
        {
            return imagePath.GetImagePath();
        }
        /// <summary>
        /// return the cached PixelMap for the given image path and transform request, if one exists
        /// </summary>
        /// <param name="imagePath"></param>
        /// <param name="transformRequest"></param>
        /// <returns></returns>
        public PixelMap? Get(ImagePath imagePath, TransformRequest transformRequest)
        {
            string cacheKey = GetCacheKey(imagePath, transformRequest);
            PixelMap? pixelMap;
            bool isCached = _memoryCache.TryGetValue(cacheKey, out pixelMap);
            if (isCached)
            {
                return pixelMap;
            }
            return null;
        }
        /// <summary>
        /// cache the PixelMap for the given image path and transform request
        /// </summary>
        /// <param name="imagePath"></param>
        /// <param name="transformRequest"></param>
        /// <param name="pixelMap"></param>
        public void Set(ImagePath imagePath, TransformRequest transformRequest, PixelMap pixelMap)
        {
            TimeSpan expirationSeconds = TimeSpan.FromSeconds(_options.CacheDuration);

            var expirationTime = DateTime.Now.Add(expirationSeconds);

            // need to use options to set custom callback for disposing the pixelmap
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(expirationSeconds)
                .RegisterPostEvictionCallback(callback: RemovedCallback, state: this);

            string cacheKey = GetCacheKey(imagePath, transformRequest);
            _memoryCache.Set(cacheKey, pixelMap, cacheEntryOptions);
        }
        /// <summary>
        /// Calls IDisposable.Dispose on objects when they are removed from the cache
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="reason"></param>
        /// <param name="state"></param>
        private static void RemovedCallback(object key, object? value, EvictionReason reason, object? state)
        {
            if (reason != EvictionReason.Removed)
            {
                var item = value as IDisposable;
                if (item != null)
                {
                    item.Dispose();
                }
            }
        }
    }
}
