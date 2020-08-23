using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors;

namespace Cloudents.FunctionsV2.Services
{
    public sealed class CropImageProcessor<TPixel> : IImageProcessor<TPixel> where TPixel : unmanaged, IPixel<TPixel>
    {
        private readonly Image<TPixel> _source;

        public CropImageProcessor(Image<TPixel> source
           )
        {
            _source = source;
        }

        public void Execute()
        {
            var z = new TPixel();
            z.FromRgba32(new Rgba32(0, 0, 0, 0));
            int y;

            for (y = _source.Height - 1; y >= 0; y--)
            {
                var reach = false;
                Span<TPixel> pixelRowSpan = _source.GetPixelRowSpan(y);
                foreach (var tPixel in pixelRowSpan)
                {
                    if (!tPixel.Equals(z))
                    {
                        reach = true;
                        break;
                    }
                }

                if (reach)
                {
                    break;
                    
                }
            }
            _source.Mutate(x => x.Crop(x.GetCurrentSize().Width, y + 1));
        }

        public void Dispose()
        {
            // throw new NotImplementedException();
        }
    }
}