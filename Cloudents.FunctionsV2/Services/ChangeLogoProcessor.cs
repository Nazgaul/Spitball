using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Processors;

namespace Cloudents.FunctionsV2.Services
{
    public sealed class ChangeLogoProcessor<TPixel> : IImageProcessor<TPixel> where TPixel : unmanaged, IPixel<TPixel>
    {
        private readonly Image<TPixel> _source;

        public ChangeLogoProcessor(Image<TPixel> source
        )
        {
            _source = source;
        }

        public void Execute()
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

                    Rgba32 pixel = Rgba32.ParseHex("000000") ;
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