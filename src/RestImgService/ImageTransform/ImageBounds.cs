namespace RestImgService.ImageTransform
{
    public class ImageBounds
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public ImageBounds(int width, int height)
        {
            Width = width;
            Height = height;
        }
        public static ImageBounds GetResizedImageBounds(PixelMap bitmap, int width, int height)
        {
            if (height == 0)
            {
                height = (int)Math.Round(bitmap.Height * (float)width / bitmap.Width);
            }
            else
            {
                if (width == 0)
                {
                    width = (int)Math.Round(bitmap.Width * (float)height / bitmap.Height);
                }
            }
            return new ImageBounds(width, height);
        }
    }
}
