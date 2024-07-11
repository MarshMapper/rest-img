namespace AlbumCrawler.Models
{
    public class Album
    { 
        // unique name for this album.  it is the name of the folder that contains the album.
        // these must be unique even across subfolders.
        public string Name { get; set; } = String.Empty;
        // the path to the album.  this is the path to the folder that contains the album, relative
        // to the web root.
        public string Path { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public string Thumbnail { get; set; } = String.Empty;
        public List<MatchingFile> Files { get; set; } = new List<MatchingFile>();

        public Album(string name, string path, string description, string thumbnail, List<MatchingFile> files)
        {
            Name = name;
            Path = path;
            Description = description;
            Thumbnail = thumbnail;
            Files = files;
        }
        public Album(Album album)
        {
            Name = album.Name;
            Path = album.Path;
            Description = album.Description;
            Thumbnail = album.Thumbnail;
            Files = album.Files;
        }
    }
}
