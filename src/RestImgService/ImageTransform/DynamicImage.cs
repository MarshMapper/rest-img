using Microsoft.Extensions.Logging;

namespace RestImgService.ImageTransform
{
    public class DynamicImage
    {
        private ILogger<RestImgMiddleware> _logger;
        private ImageExtension _imageExtension;
        private TransformCache _transformCache;

        public DynamicImage(ILogger<RestImgMiddleware> logger,
            ImageExtension imageExtension,
            TransformCache transformCache)
        {
            _logger = logger;
            _imageExtension = imageExtension;
            _transformCache = transformCache;
        }
        public ImageData GetImageData(string imagePath, TransformRequest transformRequest)
        {
            ImageData? imageData = _transformCache.Get(imagePath, transformRequest);
            if (imageData != null)
            {
                _logger.LogInformation("Image found in cache");
                return imageData;
            }

            var bitmap = PixelMap.Load(File.OpenRead(imagePath));
            ImageBounds bounds = ImageBounds.GetResizedImageBounds(bitmap, transformRequest.Width, transformRequest.Height);

            using (var resizedBitmap = bitmap.Resize(bounds.Width, bounds.Height))
            using (var resizedImage = EncodedImage.FromPixelMap(resizedBitmap))
            {
                imageData = resizedImage.Encode(transformRequest);

                // cache the result
                _transformCache.Set(imagePath, transformRequest, imageData);
            }
            bitmap.Dispose();

            return imageData;
        }
    }
}
