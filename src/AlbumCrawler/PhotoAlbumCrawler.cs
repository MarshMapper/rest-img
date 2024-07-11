using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using AlbumCrawler.Configuration;
using AlbumCrawler.Models;
using Microsoft.Extensions.Logging;

namespace AlbumCrawler
{
    /// <summary>
    /// crawls the file system for photo albums.  The albums are created from folders
    /// containing files with the specified extensions.
    /// </summary>
    public class PhotoAlbumCrawler
    {
        private readonly AlbumCrawlerOptions _albumCrawlerOptions;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<PhotoAlbumCrawler> _logger;
        private readonly AlbumCache _albumCache;

        public PhotoAlbumCrawler(IOptions<AlbumCrawlerOptions> options, 
            IWebHostEnvironment webHostEnvironment,
            ILogger<PhotoAlbumCrawler> logger,
            AlbumCache albumCache)
        {
            _albumCrawlerOptions = options.Value;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _albumCache = albumCache;
        }
        /// <summary>
        /// Use FolderCrawler to find all folders containing photos and create an Album
        /// for each.  If there are any overrides in the AlbumCrawlerOptions, apply them.
        /// </summary>
        /// <returns></returns>
        public AlbumCollection Crawl()
        {
            string startingFolderPath = GetStartingFolderPath();
            string[] extensions = GetExtensions();

            return GetOrCreateAlbums(startingFolderPath, extensions);
        }
        /// <summary>
        /// Get the album summaries that contain description, etc but not the list of files.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AlbumDto> GetAlbumSummaries()
        {
            return Crawl().Albums.Select(a => new AlbumDto(a));
        }
        /// <summary>
        /// Get defualt starting folder full path.  If the AlbumCrawlerOptions specifies a starting folder, use it.
        /// </summary>
        /// <returns></returns>
        public string GetStartingFolderPath()
        {
            string startingFolderWebPath = ".";

            if (_albumCrawlerOptions != null)
            {
                startingFolderWebPath = _albumCrawlerOptions.AlbumRoot;
            }
            return startingFolderWebPath;
        }
        /// <summary>
        /// Get the extensions to search for.  If the AlbumCrawlerOptions specifies extensions, use them.
        /// </summary>
        /// <returns></returns>
        public string[] GetExtensions()
        {
            string[] extensions = [ "jpg", "jpeg", "png", "gif" ];

            if (_albumCrawlerOptions != null)
            {
                extensions = _albumCrawlerOptions.PhotoExtensions;
            }
            return extensions;
        }
        /// <summary>
        /// Get the albums for the specified folder.  First check the cache, if not found
        /// then crawl the folder and cache the results.
        /// </summary>
        /// <param name="startingFolderWebPath"></param>
        /// <param name="extensions"></param>
        /// <returns></returns>
        public AlbumCollection GetOrCreateAlbums(string startingFolderWebPath, string[] extensions)
        {
            AlbumCollection? albums = _albumCache.Get(startingFolderWebPath);

            if (albums == null)
            {
                // not found in cache, crawl the folder and cache the results
                AlbumCollection foundAlbums = GetAlbums(startingFolderWebPath, extensions);
                _albumCache.Set(foundAlbums, startingFolderWebPath);
                albums = foundAlbums;
            }

            return albums;
        }
        /// <summary>
        /// Get the albums for the specified folder.  Use FolderCrawler to find all folders containing photos
        /// </summary>
        /// <param name="startingFolderWebPath"></param>
        /// <param name="extensions"></param>
        /// <returns></returns>
        public AlbumCollection GetAlbums(string startingFolderWebPath, string[] extensions)
        {
            FolderCrawler folderCrawler = new(_webHostEnvironment.WebRootPath);

            FolderCollection folders = folderCrawler.Crawl(startingFolderWebPath, GetWildcardPatterns(extensions));
            AlbumCollection albumCollection = new(folders, _albumCrawlerOptions);

            return albumCollection;
        }
        /// <summary>
        /// Get the album for the specified path.  Uses the cache to find the album, and if not found
        /// all albums are crawled and the cache is updated.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Album? GetAlbum(string path)
        {
            string startingFolderWebPath = GetStartingFolderPath();
            string[] extensions = GetExtensions();
            return GetOrCreateAlbums(startingFolderWebPath, extensions).Albums.FirstOrDefault(a => a.Path == path);
        }
        /// <summary>
        /// create the wildcard patterns to search for files with the specified extensions.
        /// these are in the format expected by the FileSystemGlobbing package.
        /// </summary>
        /// <param name="extensions"></param>
        /// <returns></returns>
        public string[] GetWildcardPatterns(string[] extensions)
        {
            string[] patterns = new string[extensions.Length];

            for (int i = 0; i < extensions.Length; i++)
            {
                // add wildcard to find any files with this extension in all sub folders
                patterns[i] = $"**/*.{extensions[i]}";
            }

            return patterns;
        }

    }
}
