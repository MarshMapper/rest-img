using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbumCrawler.Configuration
{
    public class AlbumOptions
    {
        // the name of the propery in the appsettings.json file, which is an array of AlbumOptions
        public const string Albums = "Albums";

        public string Name { get; set; } = String.Empty;
        public string Path { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public string Thumbnail { get; set; } = String.Empty;
    }
}
