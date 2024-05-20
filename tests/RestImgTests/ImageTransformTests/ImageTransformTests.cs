using RestImgService.ImageTransform;
using SkiaSharp;

namespace RestImgTests.ImageTransformTests
{
    public class ImageBoundsTests
    {
        [Theory]
        [InlineData(400, 300)]
        [InlineData(600, 450)]
        [InlineData(1600, 1200)]
        [InlineData(100, 75)]
        [InlineData(240, 180)]
        public void GetResizedImageBounds_MaintainsAspectRatioZeroHeight(int specifiedWidth, int expectedHeight)
        {
            SKBitmap bitmap = new SKBitmap(800, 600);
            ImageBounds bounds = ImageBounds.GetResizedImageBounds(new PixelMap(bitmap), specifiedWidth, 0);
            Assert.Equal(expectedHeight, bounds.Height);
        }
        [Theory]
        [InlineData(300, 400)]
        [InlineData(450, 600)]
        [InlineData(1200, 1600)]
        [InlineData(75, 100)]
        [InlineData(180, 240)]
        public void GetResizedImageBounds_MaintainsAspectRatioZeroWidth(int specifiedHeight, int expectedWidth)
        {
            SKBitmap bitmap = new SKBitmap(800, 600);
            ImageBounds bounds = ImageBounds.GetResizedImageBounds(new PixelMap(bitmap), 0, specifiedHeight);
            Assert.Equal(expectedWidth, bounds.Width);
        }
    }
}