using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Processors;
using SixLabors.Primitives;

namespace Cloudents.FunctionsV2.Services
{
    public class CropImageProcessorCreator : IImageProcessor
    {
        public IImageProcessor<TPixel> CreatePixelSpecificProcessor<TPixel>(Image<TPixel> source,
            Rectangle sourceRectangle)
            where TPixel : struct, IPixel<TPixel>
        {
            return new CropImageProcessor<TPixel>( source);
        }
    }

    public class ChangeLogoProcessorCreator : IImageProcessor
    {
        public IImageProcessor<TPixel> CreatePixelSpecificProcessor<TPixel>(Image<TPixel> source,
            Rectangle sourceRectangle)
            where TPixel : struct, IPixel<TPixel>
        {
            return new ChangeLogoProcessor<TPixel>(source);
        }
    }
}