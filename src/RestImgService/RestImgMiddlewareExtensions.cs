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
        public const string ProjectSection = "RestImg";
        public const string ImageResizerSection = "ImageResizer";
        public const string CachingSection = $"{ProjectSection}:{ImageResizerSection}:Caching";
        public static OutputCacheOptions GetOutputCacheOptions(ConfigurationManager configurationManager)
        {
            OutputCacheOptions outputCacheOptions = new();
            configurationManager.GetSection($"{CachingSection}:{OutputCacheOptions.OutputCache}")
                .Bind(outputCacheOptions);
            return outputCacheOptions;
        }
        public static IApplicationBuilder UseRestImg(this IApplicationBuilder app,
            ConfigurationManager configurationManager)
        {
            if (GetOutputCacheOptions(configurationManager).Enabled)
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

            OutputCacheOptions outputCacheOptions = GetOutputCacheOptions(configurationManager);
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
            services.Configure<ImageResizerOptions>(
                configurationManager.GetSection(ImageResizerOptions.ImageResizer));
        }
    }
}
