using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Processors;
using Rectangle = SixLabors.ImageSharp.Rectangle;

namespace Cloudents.FunctionsV2.Services
{
    public class CropImageProcessorCreator : IImageProcessor
    {
       

        public IImageProcessor<TPixel> CreatePixelSpecificProcessor<TPixel>(
            Configuration configuration,
            Image<TPixel> source,
            Rectangle sourceRectangle) where TPixel : unmanaged, IPixel<TPixel>
        {
            return new CropImageProcessor<TPixel>( source);
        }
    }
}