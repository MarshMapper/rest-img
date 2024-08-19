using RestImgService;
using AlbumCrawler;
using AlbumCrawler.Models;

namespace RestImg
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<CorsHostOptions>(builder.Configuration.GetSection(CorsHostOptions.Cors));

            builder.Services.AddCors(options =>
            {
                CorsHostOptions? corsSettings = builder.Configuration.GetSection(CorsHostOptions.Cors).Get<CorsHostOptions>();

                if (!(corsSettings is null) && corsSettings.AllowedHosts.Length > 0)
                {
                    options.AddDefaultPolicy(policy =>
                    {
                        policy.WithOrigins(corsSettings.AllowedHosts);
                    });
                }
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddRestImg(builder.Configuration);
            builder.Services.AddAlbumCrawler(builder.Configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors();

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.UseRestImg(builder.Configuration);
            app.UseStaticFiles();

            // add endpoint to get all albums found by the crawler
            app.MapGet("/albums", (PhotoAlbumCrawler albumCrawler) =>
            {
                return albumCrawler.GetAlbumSummaries();
            });

            // add endpoint to get a specific album by id
            app.MapGet("/albums/{id}", (PhotoAlbumCrawler albumCrawler, string id) =>
            {
                Album? album = albumCrawler.GetAlbum(id);
                return album is null ? Results.NotFound() : Results.Ok(album);
            });
            app.Run();
        }
    }
}
