using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestImgService.ImageFile;
using RestImgService.ImageTransform;

namespace RestImgService
{
    /// <summary>
    /// .NET Middleware providing a RESTful image transformation service.
    /// Intercepts requests for images and applies transformations based on query parameters.
    /// </summary>
    public class RestImgMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RestImgMiddleware> _logger;
        private readonly DynamicImage _dynamicImage;
        private readonly TransformRequestReader _transformRequestReader;
        private readonly ImageExtension _imageExtension;
        private readonly ImageResizerOptions _imageResizerOptions;

        public RestImgMiddleware(RequestDelegate next,
            ILogger<RestImgMiddleware> logger,
            DynamicImage dynamicImage,
            TransformRequestReader transformRequestReader,
            ImageExtension imageExtension,
            IOptions<ImageResizerOptions> imageResizerOptions)
        {
            _next = next;
            _logger = logger;
            _dynamicImage = dynamicImage;
            _transformRequestReader = transformRequestReader;
            _imageExtension = imageExtension;
            _imageResizerOptions = imageResizerOptions.Value;
        }
        public async Task InvokeAsync(HttpContext context, ImagePath imagePath)
        {
            _logger.LogInformation($"RestImg called for URL: {UriHelper.GetDisplayUrl(context.Request)}");

            imagePath.SetContext(context);
            TransformRequest? transformRequest = GetValidTransformRequest(context, imagePath);
            if (transformRequest == null)
            {
                // if request was not for a resized image or was invalid, let the next middleware handle it
                await _next.Invoke(context);
                return;
            }
            _logger.LogInformation($"RestImg valid request: Width: {transformRequest.Width}, Height: {transformRequest.Height}");

            if (!File.Exists(imagePath.MapImagePath()))
            {
                await _next.Invoke(context);
                return;
            }

            using (var imageData = _dynamicImage.GetImageData(imagePath, transformRequest))
            {
                // provide the response and terminate the pipeline
                context.Response.ContentType = _imageExtension.GetContentType(transformRequest.Format);
                context.Response.ContentLength = imageData.Size;
                await context.Response.Body.WriteAsync(imageData.ToArray(), 0, (int)imageData.Size);
            }
        }

        private TransformRequest? GetValidTransformRequest(HttpContext context, ImagePath imagePath)
        {
            TransformRequest? transformRequest = null;
            PathString path = context.Request.Path;

            // first check if the request is for an image and has query parameters
            if (path != null && path.HasValue && context.Request.Query.Count > 0 && imagePath.IsImageRequest())
            {
                // get the transformation parameters from the query string
                transformRequest = _transformRequestReader.ReadRequest(context.Request.Query, _imageResizerOptions.DefaultQuality);
                if (!transformRequest.IsValid(_imageResizerOptions))
                {
                    // don't return an invalid request
                    transformRequest = null;
                }
            }
            
            return transformRequest;
        }
    }
}
