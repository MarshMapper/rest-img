using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AlbumCrawler.Configuration;
using AlbumCrawler.Models;

namespace AlbumCrawler
{
    public class AlbumCache
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<AlbumCache> _logger;
        private readonly AlbumCrawlerOptions _options;

        public AlbumCache(IMemoryCache memoryCache,
            ILogger<AlbumCache> logger,
            IOptions<AlbumCrawlerOptions> options)
        {
            _memoryCache = memoryCache;
            _logger = logger;
            _options = options.Value;
        }
        protected string GetCacheKey(string startingFolderFullPath)
        {
            return startingFolderFullPath;
        }
        /// <summary>
        /// return the cached list of Albums
        /// </summary>
        /// <returns></returns>
        public AlbumCollection? Get(string startingFolderFullPath)
        {
            string cacheKey = GetCacheKey(startingFolderFullPath);
            AlbumCollection? albums;
            bool isCached = _memoryCache.TryGetValue(cacheKey, out albums);

            if (isCached)
            {
                _logger.LogInformation($"Albums found in cache for {startingFolderFullPath}");
                return albums;
            }
            return null;
        }
        /// <summary>
        /// cache the given list of albums
        /// </summary>
        /// <param name="albums"></param>
        public void Set(AlbumCollection albums, string startingFolderFullPath)
        {
            TimeSpan expirationSeconds = TimeSpan.FromSeconds(_options.CacheDuration);

            var expirationTime = DateTime.Now.Add(expirationSeconds);

            // need to use options to set custom callback for disposing the pixelmap
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(expirationSeconds);

            string cacheKey = GetCacheKey(startingFolderFullPath);
            _memoryCache.Set(cacheKey, albums, cacheEntryOptions);
        }
    }
}
