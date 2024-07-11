using AlbumCrawler.Configuration;

namespace AlbumCrawler.Models
{
    public class AlbumCollection
    {
        // this is the starting folder path used by the crawler.  it is relative to the
        // web root.  the path to files found in the folder collection will be this path
        // plus the folder name plus the file name.
        public string StartingFolderWebPath { get; set; } = "/";
        public List<Album> Albums { get; set; } = new List<Album>();
        public AlbumCollection() { }
        public AlbumCollection(FolderCollection folderCollection, AlbumCrawlerOptions albumCrawlerOptions)
        {
            StartingFolderWebPath = folderCollection.StartingFolderWebPath;
            if (folderCollection.Folders != null)
            {
                foreach (var folder in folderCollection.Folders)
                {
                    Albums.Add(CreateAlbumFromFolder(folder, albumCrawlerOptions));
                }
            }
        }
        /// <summary>
        /// Create a default Album from the folder.  The default Album uses the Name of the 
        /// folder for both the name and path.  The thumbnail is the first file in the folder.
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public Album CreateDefaultAlbumFromFolder(Folder folder, AlbumCrawlerOptions albumCrawlerOptions)
        {
            string thumbnail = folder.Files.Count > 0 ? folder.Files[0].Name : "";

            return new Album(folder.Name, folder.Name, "", thumbnail, folder.Files);
        }
        /// <summary>
        /// Create an Album from the folder.  If there are any overrides in the AlbumCrawlerOptions,
        /// use them to provide more meaningful values for the Album.
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public Album CreateAlbumFromFolder(Folder folder, AlbumCrawlerOptions albumCrawlerOptions)
        {
            Album defaultAlbum = CreateDefaultAlbumFromFolder(folder, albumCrawlerOptions);

            return MergeWithOverrides(defaultAlbum, albumCrawlerOptions);
        }
        /// <summary>
        /// Given the album automatically created from the folder, merge any overrides
        /// specified in the AlbumCrawlerOptions.
        /// </summary>
        /// <param name="defaultAlbum"></param>
        /// <returns></returns>
        public Album MergeWithOverrides(Album defaultAlbum, AlbumCrawlerOptions albumCrawlerOptions)
        {
            Album album = defaultAlbum;
            if (albumCrawlerOptions.Albums != null && albumCrawlerOptions.Albums.Length > 0)
            {
                album = new Album(defaultAlbum);

                AlbumOptions? albumOptions = albumCrawlerOptions.Albums.FirstOrDefault(a => a.Path == defaultAlbum.Path);
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

    }
}
