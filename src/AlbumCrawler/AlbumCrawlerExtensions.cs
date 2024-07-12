using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using AlbumCrawler.Configuration;

namespace AlbumCrawler
{
    public static class AlbumCrawlerExtensions
    {
        /// <summary>
        /// Adds the services required for the AlbumCrawler
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configurationManager"></param>
        public static void AddAlbumCrawler(this IServiceCollection services,
            ConfigurationManager configurationManager)
        {
            services.AddTransient<PhotoAlbumCrawler>();
            services.AddTransient<AlbumCache>();

            services.Configure<AlbumCrawlerOptions>(
                configurationManager.GetSection($"{AlbumCrawlerOptions.AlbumCrawler}"));
        }
    }
}
