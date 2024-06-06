using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestImgService.ImageTransform
{
    public class ImageCacheOptions
    {
        public const string ImageCache = "ImageFileCache";
        public bool Enabled { get; set; } = true;
        public int CacheDuration { get; set; } = 180;

        public ImageCacheOptions()
        {
        }
    }
}
