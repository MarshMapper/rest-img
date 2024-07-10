namespace AlbumCrawler.Models
{
    public class Album
    { 
        public string Name { get; set; } = String.Empty;
        public string Path { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public string Thumbnail { get; set; } = String.Empty;
        public Album(string name, string path, string description, string thumbnail)
        {
            Name = name;
            Path = path;
            Description = description;
            Thumbnail = thumbnail;
        }
        public Album(Album album)
        {
            Name = album.Name;
            Path = album.Path;
            Description = album.Description;
            Thumbnail = album.Thumbnail;
        }
    }
}
