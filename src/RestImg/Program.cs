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

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins("https://delightful-wave-003abae10.5.azurestaticapps.net");
                });
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
