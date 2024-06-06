using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RestImgService.Caching;
using Microsoft.Extensions.Configuration;
using RestImgService.ImageFile;
using RestImgService.ImageTransform;

namespace RestImgService
{
    public static class RestImgMiddlewareExtensions
    {
        public const string CachingSection = "Caching";
        public static bool IsCachingEnabled(ConfigurationManager configurationManager)
        {
            OutputCacheOptions outputCacheOptions = new();
            configurationManager.GetSection($"{CachingSection}:{OutputCacheOptions.OutputCache}")
                .Bind(outputCacheOptions);
            return outputCacheOptions.Enabled;
        }
        public static IApplicationBuilder UseRestImg(this IApplicationBuilder app,
            ConfigurationManager configurationManager)
        {
            if (IsCachingEnabled(configurationManager))
            {
                return app.UseOutputCache().
                    UseMiddleware<RestImgMiddleware>();
            }
            return app.UseMiddleware<RestImgMiddleware>();
        }
        public static void AddRestImg(this IServiceCollection services, 
            ConfigurationManager configurationManager)
        {
            services.AddMemoryCache();

            OutputCacheOptions outputCacheOptions = new();
            configurationManager.GetSection($"{CachingSection}:{OutputCacheOptions.OutputCache}")
                .Bind(outputCacheOptions);
            if (outputCacheOptions.Enabled)
            {
                services.AddOutputCache(options =>
                {
                    options.AddBasePolicy(policy => policy
                        .AddPolicy<ImageCachePolicy>()
                        .Expire(TimeSpan.FromSeconds(outputCacheOptions.CacheDuration)));
                });
            }

            services.AddTransient<DynamicImage>();
            services.AddTransient<TransformRequestReader>();
            services.AddTransient<ImageExtension>();
            services.AddTransient<TransformCache>();
            services.AddTransient<ImagePath>();

            services.Configure<OutputCacheOptions>(
                configurationManager.GetSection($"{CachingSection}:{OutputCacheOptions.OutputCache}"));
            services.Configure<ImageCacheOptions>(
                configurationManager.GetSection($"{CachingSection}:{ImageCacheOptions.ImageCache}"));
        }
    }
}
