using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace RestImgService.ImageFile
{
    /// <summary>
    /// Manages the file paths that potentially reference image files
    /// </summary>
    public class ImagePath
    {
        private readonly ImageExtension _imageExtension;
        private readonly IWebHostEnvironment? _environment;
        private PathString _pathString = new PathString();

        public ImagePath(ImageExtension imageExtension)
        {
            _imageExtension = imageExtension;
        }
        public ImagePath(ImageExtension imageExtension, 
            IWebHostEnvironment environment) : this(imageExtension)
        {
            _environment = environment;
        }
        /// <summary>
        /// Important to call this method before using the ImagePath object.  Gets the PathString
        /// from the HttpContext, which is not available in the constructor.
        /// </summary>
        /// <param name="context"></param>
        public void SetContext(HttpContext context)
        {
            if (context != null && context.Request.Path != null && context.Request.Path.HasValue)
            {
                _pathString = context.Request.Path;
            }
        }
        /// <summary>
        /// Get the full path to the file on the server
        /// </summary>
        /// <returns></returns>
        public string MapImagePath()
        {
            if ((!_pathString.HasValue) || (_environment == null))
            {
                return String.Empty;
            }

            return Path.Combine(_environment.WebRootPath ?? String.Empty,
                _pathString.Value.Replace('/', Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar));
        }
        /// <summary>
        /// get the relative path for the requested file
        /// </summary>
        /// <returns></returns>
        public string GetImagePath()
        {
            if (!_pathString.HasValue)
            {
                return String.Empty;
            }

            return _pathString.Value.ToString();
        }
        /// <summary>
        /// determine if the request is for an image file based on the file extension
        /// </summary>
        /// <returns></returns>
        public bool IsImageRequest()
        {
            if (!_pathString.HasValue)
            {
                return false;
            }
            return IsImageRequest(_pathString.Value);
        }
        public bool IsImageRequest(string imagePath)
        {
            string extension = Path.GetExtension(imagePath).ToLower();
            return _imageExtension.IsValidExtension(extension);
        }
    }
}
