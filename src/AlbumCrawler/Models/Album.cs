﻿namespace AlbumCrawler.Models
{
    public class Album
    { 
        public string Name { get; set; } = String.Empty;
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
