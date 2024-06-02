using RestImgService.ImageFile;
using SkiaSharp;

namespace RestImgService.ImageTransform
{
    /// <summary>
    /// Represents an image that has been encoded into a specific format.
    /// </summary>
    public class EncodedImage : IDisposable
    {
        private readonly SKImage _image;
        private readonly ImageExtension _imageExtension = new ImageExtension();
        public EncodedImage(SKImage image)
        {
            _image = image;
        }
        public static EncodedImage FromPixelMap(PixelMap pixelMap)
        {
            return new EncodedImage(SKImage.FromBitmap(pixelMap.GetBitmap()));
        }
        public ImageData Encode(TransformRequest transformRequest)
        {
            var encodeFormat = _imageExtension.GetEncodedImageFormat(transformRequest.Format);
            return new ImageData(_image.Encode(encodeFormat, transformRequest.Quality));
        }
        public void Dispose()
        {
            // we don't "own" the image so we don't dispose it
        }
    }
}
