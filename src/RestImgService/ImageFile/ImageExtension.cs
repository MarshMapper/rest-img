using SkiaSharp;

namespace RestImgService.ImageFile
{
    public class ImageExtension
    {
        public static string DefaultExtension = ".jpg";
        private readonly string[] _imageExtensions = [
            ".bmp",
            ".gif",
            ".jpg",
            ".jpeg",
            ".png"
        ];

        public ImageExtension() { }
        public bool IsValidExtension(string extension)
        {
            return _imageExtensions.Contains(extension);
        }
        public string GetContentType(string extension)
        {
            return extension switch
            {
                "jpg" => "image/jpeg",
                "png" => "image/png",
                "webp" => "image/webp",
                "gif" => "image/gif",
                "bmp" => "image/bmp",
                _ => "image/jpeg"
            };
        }
        public SKEncodedImageFormat GetEncodedImageFormat(string extension)
        {
            return extension switch
            {
                "jpg" => SKEncodedImageFormat.Jpeg,
                "png" => SKEncodedImageFormat.Png,
                "webp" => SKEncodedImageFormat.Webp,
                // SkiaSharp only supports PNG, JPEG, and WEBP, default to JPEG for other formats
                _ => SKEncodedImageFormat.Jpeg
            };
        }
    }
}
