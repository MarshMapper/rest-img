# rest-img
.NET RESTful image services including:   
    - image transformation middleware based on the SkiaSharp library.   
    - photo album services that automatically locate and serve images from a given directory.   

## Demo
GitHub actions deploy changes to Azure when changes are merged: [https://rest-img.azurewebsites.net/](https://rest-img.azurewebsites.net/)  
See the sections below on how to access the album and image services included in the sample project.

# RestImgMiddleware
Dynamically transforms images based on query parameters.  These can be added after the path to any image.

## Using
When adding services at startup, include the following line:

            builder.Services.AddRestImg();

Then Use the service before the static file service:

            app.UseRestImg();
            app.UseStaticFiles();

## Configuration
By default, the middleware will cache the original image files and the resized images.  The ImageFileCache
will cache the original files and the OutputCache will cache the resized images.  The following configuration 
settings control this behavior:

      "Caching": {
        // configure caching of the original files that will be resized
        "ImageFileCache": {
          "Enabled": true,
          "TimeoutInSeconds": 300
        },
        // configure caching of the resized images using .NET Output Cache
        "OutputCache": {
          "Enabled": true,
          "TimeoutInSeconds": 300
        }
      },

## Query Parameters 
One of w or h must be specified, all others are optional

	w = width in pixels
	h = height in pixels
	fmt = format, must be one of "jpg", "png" or "webp"
	q = quality, 0 to 100 (100 is best)
	
## Examples
	
	https://rest-img.azurewebsites.net/photos/flowers/zowie.jpg?h=300
	https://rest-img.azurewebsites.net/photos/flowers/zowie.jpg?w=400
	https://rest-img.azurewebsites.net/photos/flowers/zowie.jpg?w=600&fmt=wepb
	
will return the included sample image resized to the width or height specified while maintaining the aspect ratio.

# AlbumCrawler library
The AlbumCrawler library crawls a specified directory and returns a list of albums (folders that contain photos).  
It can be used to create a photo album service, as demonstrated in the included sample project.

## Using
To use the AlbumCrawler, add the following line to the ConfigureServices method in the Startup class:

            builder.Services.AddAlbumCrawler(builder.Configuration);

Then PhotoAlbumCaller can be injected and used in Minimal APIs or controllers:

            app.MapGet("/albums", (PhotoAlbumCrawler albumCrawler) =>
            {
                return albumCrawler.Crawl();
            });

            app.MapGet("/albums/{id}", (PhotoAlbumCrawler albumCrawler, string id) =>
            {
                Album? album = albumCrawler.GetAlbum(id);
                return album is null ? Results.NotFound() : Results.Ok(album);
            });

The first call above will return a list of albums, while the second will return the photos in the specified album.

## Examples
	https://rest-img.azurewebsites.net/albums - gets the list of albums
	https://rest-img.azurewebsites.net/ablums/flowers - gets the photos in the flowers album


# Roadmap
There are several general purpose image content delivery networks but most do much more than simple resizing, and are not written in .NET.

This is simple image transformation middleware for .NET that provides the functinality required to use Angular's NgOptimizedImage.
This will allow simple self-hosting of images while using the directive.

A future step will be to provide the required loader, or make the middleware compatible with an existing one: 

[Configuring an image loader for NgOptimizedImage](https://angular.io/guide/image-directive#configuring-an-image-loader-for-ngoptimizedimage)
