using SkiaSharp;
using Microsoft.Extensions.Logging;

namespace RestImgService.ImageTransform
{
    public class DynamicImage
    {
        private ILogger<RestImgMiddleware> _logger;
        private TransformCache _transformCache;

        public DynamicImage(ILogger<RestImgMiddleware> logger,
            TransformCache transformCache)
        {
            _logger = logger;
            _transformCache = transformCache;
        }
        public SKData GetImageData(string imagePath, TransformRequest transformRequest)
        {
            SKData? imageData = _transformCache.Get(imagePath, transformRequest);
            if (imageData != null)
            {
                _logger.LogInformation("Image found in cache");
                return imageData;
            }

            var bitmap = LoadBitmap(File.OpenRead(imagePath));
            int width = transformRequest.Width;
            int height = transformRequest.Height;

            // if only one dimension is given, calculate other from aspect ratio
            if (height == 0)
            {
                height = (int)Math.Round(bitmap.Height * (float)transformRequest.Width / bitmap.Width);
            }
            else
            {
                if (width == 0)
                {
                    width = (int)Math.Round(bitmap.Width * (float)transformRequest.Height / bitmap.Height);
                }
            }

            var resizedImageInfo = new SKImageInfo(width, height, SKImageInfo.PlatformColorType, bitmap.AlphaType);
            var resizedBitmap = bitmap.Resize(resizedImageInfo, SKFilterQuality.High);

            var resizedImage = SKImage.FromBitmap(resizedBitmap);
            var encodeFormat = transformRequest.Format == "png" ? SKEncodedImageFormat.Png : SKEncodedImageFormat.Jpeg;
            imageData = resizedImage.Encode(encodeFormat, transformRequest.Quality);

            // cache the result
            _transformCache.Set(imagePath, transformRequest, imageData);

            // cleanup
            resizedImage.Dispose();
            bitmap.Dispose();
            resizedBitmap.Dispose();

            return imageData;
        }
        private SKBitmap LoadBitmap(Stream stream)
        {
            using (var codec = SKCodec.Create(stream))
            {
                var info = codec.Info;
                var bitmap = new SKBitmap(info.Width, info.Height, SKImageInfo.PlatformColorType, info.IsOpaque ? SKAlphaType.Opaque : SKAlphaType.Premul);

                var result = codec.GetPixels(bitmap.Info, bitmap.GetPixels(out nint rowBytes));
                if (result == SKCodecResult.Success || result == SKCodecResult.IncompleteInput)
                {
                    return bitmap;
                }
                else
                {
                    throw new Exception($"Could not load bitmap: {result}");
                }
            }
        }   
    }
}
