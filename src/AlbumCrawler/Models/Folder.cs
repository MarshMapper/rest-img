namespace AlbumCrawler.Models
{
    /// <summary>
    /// represents a folder that contains files that match the patterns used
    /// while crawling
    /// </summary>
    public class Folder
    {
        public string Name { get; set; }

        public List<MatchingFile> Files { get; set; } = new List<MatchingFile>();
        public Folder(string name)
        {
            Name = name;
        }
        public void AddFile(string fileName)
        {
            Files.Add(new MatchingFile(fileName));
        }
    }
}
