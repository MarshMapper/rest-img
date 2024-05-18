namespace RestImgService.ImageTransform
{
    public class ImageExtension
    {
        public static string DefaultExtension = ".jpg";
        private readonly string[] _imageExtensions = [
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
                "gif" => "image/gif",
                "bmp" => "image/bmp",
                _ => "image/jpeg",
            };
        }
    }
}
