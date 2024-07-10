using AlbumCrawler.Configuration;
using AlbumCrawler.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

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

        public PhotoAlbumCrawler(IOptions<AlbumCrawlerOptions> options, 
            IWebHostEnvironment webHostEnvironment)
        {
            _albumCrawlerOptions = options.Value;
            _webHostEnvironment = webHostEnvironment;
        }
        /// <summary>
        /// Use FolderCrawler to find all folders containing photos and create an Album
        /// for each.  If there are any overrides in the AlbumCrawlerOptions, apply them.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Album> Crawl()
        {
            string startingFolderFullPath = ".";
            string[] extensions = [ "jpg", "jpeg", "png", "gif" ];

            if (_albumCrawlerOptions != null)
            {
                startingFolderFullPath = MapPath(_albumCrawlerOptions.AlbumRoot);
                extensions = _albumCrawlerOptions.PhotoExtensions;
            }
            FolderCrawler folderCrawler = new();

            IEnumerable<Folder> folders = folderCrawler.Crawl(startingFolderFullPath, GetWildcardPatterns(extensions));
            List<Album> albums = new List<Album>();

            foreach (Folder folder in folders)
            {
                albums.Add(CreateAlbumFromFolder(folder));
            }

            return albums;
        }
        /// <summary>
        /// Create a default Album from the folder.  The default Album uses the Name of the 
        /// folder for both the name and path.  The thumbnail is the first file in the folder.
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public Album CreateDefaultAlbumFromFolder(Folder folder)
        {
            string thumbnail = folder.Files.Count > 0 ? folder.Files[0].Name : "";

            return new Album(folder.Name, folder.Name, "", thumbnail);
        }
        /// <summary>
        /// Create an Album from the folder.  If there are any overrides in the AlbumCrawlerOptions,
        /// use them to provide more meaningful values for the Album.
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public Album CreateAlbumFromFolder(Folder folder)
        {
            Album defaultAlbum = CreateDefaultAlbumFromFolder(folder);

            return MergeWithOverrides(defaultAlbum);
        }
        /// <summary>
        /// Given the album automatically created from the folder, merge any overrides
        /// specified in the AlbumCrawlerOptions.
        /// </summary>
        /// <param name="defaultAlbum"></param>
        /// <returns></returns>
        public Album MergeWithOverrides(Album defaultAlbum)
        {
            Album album = defaultAlbum;
            if (_albumCrawlerOptions.Albums != null && _albumCrawlerOptions.Albums.Length > 0)
            {
                album = new Album(defaultAlbum);

                AlbumOptions? albumOptions = _albumCrawlerOptions.Albums.FirstOrDefault(a => a.Path == defaultAlbum.Path);
                if (albumOptions != null)
                {
                    if (!String.IsNullOrEmpty(albumOptions.Name))
                    {
                        album.Name = albumOptions.Name;
                    }
                    if (!String.IsNullOrEmpty(albumOptions.Description))
                    {
                        album.Description = albumOptions.Description;
                    }
                    if (!String.IsNullOrEmpty(albumOptions.Thumbnail))
                    {
                        album.Thumbnail = albumOptions.Thumbnail;
                    }
                }
            }
            return album;
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
        /// <summary>
        /// maps the path relative to the web root to a full path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected string MapPath(string path)
        {
            if (path.EndsWith("/") || path.EndsWith("\\"))
            {
                path = path.Remove(path.Length - 1);
            }
            return _webHostEnvironment.WebRootPath + path;
        }
    }
}
