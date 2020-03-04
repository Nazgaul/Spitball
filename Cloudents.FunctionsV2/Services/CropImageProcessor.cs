using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors;
using SixLabors.Primitives;

namespace Cloudents.FunctionsV2.Services
{
    public sealed class CropImageProcessor<TPixel> : IImageProcessor<TPixel> where TPixel : struct, IPixel<TPixel>
    {
        private readonly Image<TPixel> _source;

        public CropImageProcessor(Image<TPixel> source
           )
        {
            _source = source;
        }

        public void Apply()
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


    public sealed class ChangeLogoProcessor<TPixel> : IImageProcessor<TPixel> where TPixel : struct, IPixel<TPixel>
    {
        private readonly Image<TPixel> _source;

        public ChangeLogoProcessor(Image<TPixel> source
        )
        {
            _source = source;
        }

        public void Apply()
        {
            var z = new TPixel();
            z.FromRgba32(new Rgba32(0, 0, 0, 0));
            int y;

            //var result = new TPixel();
           // result.FromRgba32(new Rgba32(255, 0, 0, 255));

            for (y = _source.Height - 1; y >= 0; y--)
            {
                Span<TPixel> pixelRowSpan = _source.GetPixelRowSpan(y);
                for (int x = 0; x < _source.Width; x++)
                {
                    if (pixelRowSpan[x].Equals(z))
                    {
                        continue;
                    }

                    Rgba32 pixel = Rgba32.Black ;
                    pixelRowSpan[x].ToRgba32(ref pixel);

                    pixelRowSpan[x].FromRgba32(new Rgba32(0x43, 0x42,0x5d, pixel.A));
                    //pixelRowSpan[x] = result;
                }
            }
        }

        public void Dispose()
        {
            // throw new NotImplementedException();
        }
    }
}