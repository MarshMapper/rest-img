using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Primitives;
using RestImgService.ImageFile;

namespace RestImgService.Caching
{
    public sealed class ImageCachePolicy : IOutputCachePolicy
    {
        /// <summary>
        /// Custom cache policy for image files.  Want to cache images including the query string
        /// because the query string specifies the image transformations that are applied.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public ValueTask CacheRequestAsync(OutputCacheContext context, CancellationToken cancellation)
        {
            var pathString = context.HttpContext.Request.Path;

            if (pathString == null || !pathString.HasValue)
            {
                return ValueTask.CompletedTask;
            }
            var imagePath = new ImagePath(new ImageExtension());

            // only cache images 
            if (! imagePath.IsImageRequest(pathString))
            {
                return ValueTask.CompletedTask;
            }
                
            context.Tags.Add(pathString);

            //default implementation
            var attemptOutputCaching = AttemptOutputCaching(context);
            context.EnableOutputCaching = true;
            context.AllowCacheLookup = attemptOutputCaching;
            context.AllowCacheStorage = attemptOutputCaching;
            context.AllowLocking = true;

            // Vary by any query by default
            context.CacheVaryByRules.QueryKeys = "*";

            return ValueTask.CompletedTask;
        }

        public ValueTask ServeFromCacheAsync(OutputCacheContext context, CancellationToken cancellation)
        {
            return ValueTask.CompletedTask;
        }

        public ValueTask ServeResponseAsync(OutputCacheContext context, CancellationToken cancellation)
        {
            var response = context.HttpContext.Response;

            // Verify existence of cookie headers
            if (!StringValues.IsNullOrEmpty(response.Headers.SetCookie))
            {
                context.AllowCacheStorage = false;
                return ValueTask.CompletedTask;
            }

            // Check response code
            if (response.StatusCode != StatusCodes.Status200OK)
            {
                context.AllowCacheStorage = false;
                return ValueTask.CompletedTask;
            }

            return ValueTask.CompletedTask;
        }
        private static bool AttemptOutputCaching(OutputCacheContext context)
        {
            // this policy is intended as a replacement for the default OutputCachePolicy so
            // we enforce the same rules here
            var request = context.HttpContext.Request;

            // Verify the method is GET or HEAD
            if (!HttpMethods.IsGet(request.Method) && !HttpMethods.IsHead(request.Method))
            {
                return false;
            }    
                
            // only cache anonymous requests
            if (!StringValues.IsNullOrEmpty(request.Headers.Authorization) || request.HttpContext.User?.Identity?.IsAuthenticated == true)
            {
                return false;
            }

            return true;
        }
    }
}
