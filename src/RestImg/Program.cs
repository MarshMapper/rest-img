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

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.UseRestImg(builder.Configuration);
            app.UseStaticFiles();

            app.MapGet("/albums", (PhotoAlbumCrawler albumCrawler) =>
            {
                return albumCrawler.GetAlbumSummaries();
            });

            app.MapGet("/albums/{id}", (PhotoAlbumCrawler albumCrawler, string id) =>
            {
                Album? album = albumCrawler.GetAlbum(id);
                return album is null ? Results.NotFound() : Results.Ok(album);
            });
            app.Run();
        }
    }
}
