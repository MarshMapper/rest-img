namespace AlbumCrawler.Configuration
{
    public class AlbumCrawlerOptions
    {
        public const string ProjectSection = "RestImg";
        public const string AlbumCrawler = $"{ProjectSection}:AlbumCrawler";
        public string[] PhotoExtensions { get; set; } = ["jpg", "jpeg", "png", "gif"];
        public string AlbumRoot { get; set; } = "/photos"; // relative to the web root
        public AlbumOptions[] Albums { get; set; } = Array.Empty<AlbumOptions>();
        public int CacheDuration { get; set; } = 60 * 30; // 30 minutes

        public AlbumCrawlerOptions()
        {
        }
    }
}
