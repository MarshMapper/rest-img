using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using AlbumCrawler.Configuration;

namespace AlbumCrawler
{
    public static class AlbumCrawlerExtensions
    {
        public static void AddAlbumCrawler(this IServiceCollection services,
            ConfigurationManager configurationManager)
        {
            services.AddTransient<PhotoAlbumCrawler>();

            services.Configure<AlbumCrawlerOptions>(
                configurationManager.GetSection($"{AlbumCrawlerOptions.AlbumCrawler}"));
        }
    }
}
