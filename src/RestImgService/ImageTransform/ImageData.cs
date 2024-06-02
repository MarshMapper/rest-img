using SkiaSharp;

namespace RestImgService.ImageTransform
{
    /// <summary>
    /// the raw image data
    /// </summary>
    public class ImageData : IDisposable
    {
        private readonly SKData sKData;
        public ImageData(SKData sKData)
        {
            this.sKData = sKData;
        }
        public ImageData(byte[] data) : this(SKData.CreateCopy(data))
        {
        }
        public byte[] ToArray()
        {
            return sKData.ToArray();
        }

        public long Size => sKData.Size;
        public void Dispose()
        {
            sKData.Dispose();
        }
    }
}
