using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Processors;

namespace Cloudents.FunctionsV2.Services
{
    public class ChangeLogoProcessorCreator : IImageProcessor
    {
        public IImageProcessor<TPixel> CreatePixelSpecificProcessor<TPixel>( 
            Configuration configuration,
            Image<TPixel> source,
            Rectangle sourceRectangle)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            return new ChangeLogoProcessor<TPixel>(source);
        }
    }
}