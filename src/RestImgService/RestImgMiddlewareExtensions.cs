using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RestImgService.Caching;
using RestImgService.ImageFile;
using RestImgService.ImageTransform;

namespace RestImgService
{
    public static class RestImgMiddlewareExtensions
    {
        public static IApplicationBuilder UseRestImg(this IApplicationBuilder app)
        {
            return app.UseOutputCache().
                UseMiddleware<RestImgMiddleware>();
        }
        public static void AddRestImg(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddOutputCache(options =>
            {
                options.AddBasePolicy(policy => policy
                    .AddPolicy<ImageCachePolicy>()
                    .Expire(TimeSpan.FromMinutes(5)));
            });
            services.AddTransient<DynamicImage>();
            services.AddTransient<TransformRequestReader>();
            services.AddTransient<ImageExtension>();
            services.AddTransient<TransformCache>();
            services.AddTransient<ImagePath>();
        }
    }
}
