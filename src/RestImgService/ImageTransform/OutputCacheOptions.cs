using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestImgService.ImageTransform
{
    public class OutputCacheOptions
    {
        public const string OutputCache = "OutputCache";
        public bool Enabled { get; set; } = true;
        public int CacheDuration { get; set; } = 180;

        public OutputCacheOptions()
        {
        }
    }
}
