﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
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
        private readonly IWebHostEnvironment _environment;

        public RestImgMiddleware(RequestDelegate next,
            ILogger<RestImgMiddleware> logger,
            DynamicImage dynamicImage,
            TransformRequestReader transformRequestReader,
            ImageExtension imageExtension,
            IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _dynamicImage = dynamicImage;
            _transformRequestReader = transformRequestReader;
            _imageExtension = imageExtension;
            _environment = environment;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation($"RestImg called for URL: {UriHelper.GetDisplayUrl(context.Request)}");

            TransformRequest? transformRequest = GetValidTransformRequest(context);
            if (transformRequest == null)
            {
                // if request was not for a resized image or was invalid, let the next middleware handle it
                await _next.Invoke(context);
                return;
            }
            _logger.LogInformation($"RestImg valid request: Width: {transformRequest.Width}, Height: {transformRequest.Height}");

            string imagePath = MapImagePath(context.Request.Path);
            if (!File.Exists(imagePath))
            {
                await _next.Invoke(context);
                return;
            }

            var imageData = _dynamicImage.GetImageData(imagePath, transformRequest);

            // provide the response and terminate the pipeline
            context.Response.ContentType = _imageExtension.GetContentType(transformRequest.Format);
            context.Response.ContentLength = imageData.Size;
            await context.Response.Body.WriteAsync(imageData.ToArray(), 0, (int)imageData.Size);

            imageData.Dispose();
        }
        private string MapImagePath(PathString path)
        {
            if (path == null || !path.HasValue)
            {
                return String.Empty;
            }

            return Path.Combine(_environment.WebRootPath ?? String.Empty,
                path.Value.Replace('/', Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar));
        }
        private TransformRequest? GetValidTransformRequest(HttpContext context)
        {
            TransformRequest? transformRequest = null;
            PathString path = context.Request.Path;

            // first check if the request is for an image and has query parameters
            if (path != null && path.HasValue && context.Request.Query.Count > 0 && IsImageRequest(path))
            {
                // get the transformation parameters from the query string
                transformRequest = _transformRequestReader.ReadRequest(context.Request.Query);
                if (!transformRequest.IsValid())
                {
                    // don't return an invalid request
                    transformRequest = null;
                }
            }
            
            return transformRequest;
        }
        private bool IsImageRequest(PathString path)
        {
            if ((path == null) || !path.HasValue)
            {
                return false;
            }
            string extension = Path.GetExtension(path.Value).ToLower();

            return _imageExtension.IsValidExtension(extension);
        }
    }
}
