using RestImgService.ImageFile;

namespace RestImgService.ImageTransform
{
    public class TransformRequest
    {
        private readonly string[] _outputFormats = [
            "jpg",
            "jpeg",
            "webp",
            "png"
        ];
        public int Width { get; set; }
        public int Height { get; set; }
        public string Format { get; set; }
        public int Quality { get; set; } = 75;
        public TransformRequest()
        {
            Format = ImageExtension.DefaultExtension;
        }
        public TransformRequest(int width, int height, string format, int quality)
        {
            Width = width;
            Height = height;
            Format = format;
            Quality = quality;
        }
        public bool IsValid(ImageResizerOptions imageResizerOptions)
        {
            // check if the request is valid.  if the width or height is greater than the maximum,
            // just set the value to the maximum and consider it valid
            if (Width > imageResizerOptions.MaximumWidth)
            {
                Width = imageResizerOptions.MaximumWidth;
            }
            if (Height > imageResizerOptions.MaximumHeight)
            {
                Height = imageResizerOptions.MaximumHeight;
            }
            return (Width > 0 || Height > 0) && _outputFormats.Contains(Format) &&
                (Quality >= 0 && Quality <= 100);
        }
    }
}
