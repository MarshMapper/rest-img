using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestImgService.ImageFile;

namespace RestImgService.ImageTransform
{
    /// <summary>
    /// Gets an image file with the requested transformations applied
    /// </summary>
    public class DynamicImage
    {
        private ILogger<RestImgMiddleware> _logger;
        private ImageExtension _imageExtension;
        private TransformCache _transformCache;
        private ImageCacheOptions _options;

        public DynamicImage(ILogger<RestImgMiddleware> logger,
            ImageExtension imageExtension,
            TransformCache transformCache,
            IOptions<ImageCacheOptions> options)
        {
            _logger = logger;
            _imageExtension = imageExtension;
            _transformCache = transformCache;
            _options = options.Value;
        }
        public ImageData GetImageData(ImagePath imagePath, TransformRequest transformRequest)
        {
            string fullPath = imagePath.MapImagePath();

            // if enabled, .NET OutputCaching will be used to cache the resized image so we should not
            // get multiple requests for the same resized image.  we may get multiple requests for the
            // same image in different sizes, so cache the original image and resize it as needed.
            //       
            PixelMap? pixelMap = null;
            if (_options.Enabled)
            {
                pixelMap = _transformCache.Get(imagePath, transformRequest);
            }
            if (pixelMap == null)
            {
                pixelMap = PixelMap.Load(File.OpenRead(fullPath));

                if (_options.Enabled)
                {
                    // cache the result
                    _transformCache.Set(imagePath, transformRequest, pixelMap);
                }
            }
            else
            {
                // restore the pixelmap from the image data
                _logger.LogInformation("Image found in cache");
            }

            ImageBounds bounds = ImageBounds.GetResizedImageBounds(pixelMap, transformRequest.Width, transformRequest.Height);
            ImageData imageData;

            using (var resizedBitmap = pixelMap.Resize(bounds.Width, bounds.Height))
            using (var resizedImage = EncodedImage.FromPixelMap(resizedBitmap))
            {
                imageData = resizedImage.Encode(transformRequest);
            }

            return imageData;
        }
    }
}
