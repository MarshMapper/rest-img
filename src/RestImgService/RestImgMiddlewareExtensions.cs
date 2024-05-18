using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RestImgService.ImageTransform;

namespace RestImgService
{
    public static class RestImgMiddlewareExtensions
    {
        public static IApplicationBuilder UseRestImg(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RestImgMiddleware>();
        }
        public static void AddRestImg(this IServiceCollection services)
        {
            services.AddTransient<DynamicImage>();
            services.AddTransient<TransformRequestReader>();
            services.AddTransient<ImageExtension>();
            services.AddTransient<TransformCache>();
        }
    }
}
