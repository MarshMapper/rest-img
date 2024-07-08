namespace AlbumCrawler.Configuration
{
    public class AlbumCrawlerOptions
    {
        public const string AlbumCrawler = "AlbumCrawler";
        public string[] PhotoExtensions { get; set; } = [".jpg", ".jpeg", ".png", ".gif"];
        public string AlbumRoot { get; set; } = "/photos"; // relative to the web root

        public AlbumCrawlerOptions()
        {
        }
    }
}
