using SkiaSharp;

namespace RestImgService.ImageTransform
{
    /// <summary>
    /// Thin wrapper around a SkiaSharp bitmap, representing the unencoded pixel data of an image,
    /// sometimes called a bitmap :)
    /// </summary>
    public class PixelMap : IDisposable
    {
        private readonly SKBitmap _bitmap;
        public PixelMap(SKBitmap bitmap)
        {
            _bitmap = bitmap;
        }
        // for use within the library only
        internal SKBitmap GetBitmap() => _bitmap;
        public int Width => _bitmap.Width;
        public int Height => _bitmap.Height;
        public PixelMap Resize(int width, int height)
        {
            var resizedImageInfo = new SKImageInfo(width, height, SKImageInfo.PlatformColorType, _bitmap.AlphaType);
            return new PixelMap(_bitmap.Resize(resizedImageInfo, SKFilterQuality.High));
        }
        public static PixelMap Load(Stream stream)
        {
            using (var codec = SKCodec.Create(stream))
            {
                var info = codec.Info;
                var bitmap = new SKBitmap(info.Width, info.Height, SKImageInfo.PlatformColorType, info.IsOpaque ? SKAlphaType.Opaque : SKAlphaType.Premul);

                var result = codec.GetPixels(bitmap.Info, bitmap.GetPixels(out nint rowBytes));
                if (result == SKCodecResult.Success || result == SKCodecResult.IncompleteInput)
                {
                    return new PixelMap(bitmap);
                }
                else
                {
                    throw new Exception($"Could not load bitmap: {result}");
                }
            }
        }
        public void Dispose()
        {
            _bitmap.Dispose();
        }
    }
}
