# rest-img
.NET RESTful image transformation middleware 

Dynamically transforms images based on query parameters.  These can be added after the path to any image.

# Using
When adding services at startup, include the following line:

            builder.Services.AddRestImg();

Then Use the service before the static file service:

            app.UseRestImg();
            app.UseStaticFiles();

# Options / Parameters 

One of w or h must be specified, all others are optional

	w = width in pixels
	h = height in pixels
	fmt = format, must be one of "jpg", "png" or "webp"
	q = quality, 0 to 100 (100 is best)
	
## Example
	
	https://localhost:xxxx/zowie.jpg?h=300
	
	Will return the included sample image resized to a height of 300 pixels with aspect ration maintained
