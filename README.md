# rest-img
.NET RESTful image transformation middleware based on the SkiaSharp library.

Dynamically transforms images based on query parameters.  These can be added after the path to any image.

# Using
When adding services at startup, include the following line:

            builder.Services.AddRestImg();

Then Use the service before the static file service:

            app.UseRestImg();
            app.UseStaticFiles();

# Configuration
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

# Query Parameters 
One of w or h must be specified, all others are optional

	w = width in pixels
	h = height in pixels
	fmt = format, must be one of "jpg", "png" or "webp"
	q = quality, 0 to 100 (100 is best)
	
## Example
	
	https://localhost:xxxx/zowie.jpg?h=300
	https://localhost:xxxx/zowie.jpg?w=400
	https://localhost:xxxx/zowie.jpg?w=600&fmt=wepb
	
will return the included sample image resized to the width or height specified while maintaining the aspect ratio.


# Roadmap
There are several general purpose image content delivery networks but most do much more than simple resizing, and are not written in .NET.

This is simple image transformation middleware for .NET that provides the functinality required to use Angular's NgOptimizedImage.
This will allow simple self-hosting of images while using the directive.

The next step will be to provide the required loader:

[Configuring an image loader for NgOptimizedImage](https://angular.io/guide/image-directive#configuring-an-image-loader-for-ngoptimizedimage)
