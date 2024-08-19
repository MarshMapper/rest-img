namespace RestImgService.ImageTransform
{
    public class ImageResizerOptions
    {
        public const string ProjectSection = "RestImg";
        public const string ImageResizerSection = "ImageResizer";
        public const string ImageResizer = $"{ProjectSection}:{ImageResizerSection}";
        public int MaximumWidth { get; set; }
        public int MaximumHeight { get; set; }
        public int DefaultQuality { get; set; }
    }
}
