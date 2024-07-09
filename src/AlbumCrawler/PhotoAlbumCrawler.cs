using AlbumCrawler.Configuration;
using AlbumCrawler.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace AlbumCrawler
{
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
        public Album CreateDefaultAlbumFromFolder(Folder folder)
        {
            string thumbnail = folder.Files.Count > 0 ? folder.Files[0].Name : "";

            return new Album(folder.Name, folder.Name, "", thumbnail);
        }
        public Album CreateAlbumFromFolder(Folder folder)
        {
            Album defaultAlbum = CreateDefaultAlbumFromFolder(folder);

            return MergeWithOverrides(defaultAlbum);
        }
        public Album MergeWithOverrides(Album defaultAlbum)
        {
            if (_albumCrawlerOptions.Albums != null && _albumCrawlerOptions.Albums.Length > 0)
            {
                AlbumOptions? albumOptions = _albumCrawlerOptions.Albums.FirstOrDefault(a => a.Path == defaultAlbum.Path);
                if (albumOptions != null)
                {
                    if (!String.IsNullOrEmpty(albumOptions.Name))
                    {
                        defaultAlbum.Name = albumOptions.Name;
                    }
                    if (!String.IsNullOrEmpty(albumOptions.Description))
                    {
                        defaultAlbum.Description = albumOptions.Description;
                    }
                    if (!String.IsNullOrEmpty(albumOptions.Thumbnail))
                    {
                        defaultAlbum.Thumbnail = albumOptions.Thumbnail;
                    }
                }
            }
            return defaultAlbum;
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
