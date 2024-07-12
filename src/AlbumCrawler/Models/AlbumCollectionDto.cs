using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbumCrawler.Models
{
    public class AlbumCollectionDto
    {
        public string StartingFolderWebPath { get; set; } = "/";
        public List<AlbumDto> Albums { get; set; } = new List<AlbumDto>();
        public AlbumCollectionDto(AlbumCollection albumCollection) 
        {
            StartingFolderWebPath = albumCollection.StartingFolderWebPath;
            if (albumCollection.Albums != null)
            {
                foreach (var album in albumCollection.Albums)
                {
                    Albums.Add(new AlbumDto(album));
                }
            }
        }
    }
}
