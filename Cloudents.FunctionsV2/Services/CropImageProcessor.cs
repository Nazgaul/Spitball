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
        private readonly CropImageProcessorCreator _cropImageProcessor;
        private readonly Image<TPixel> _source;
        private readonly Rectangle _sourceRectangle;

        public CropImageProcessor(CropImageProcessorCreator cropImageProcessor, Image<TPixel> source,
            Rectangle sourceRectangle)
        {
            _cropImageProcessor = cropImageProcessor;
            _source = source;
            _sourceRectangle = sourceRectangle;
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
                //TPixel zz = new TPixel();
                //foreach (var tPixel in pixelRowSpan)
                //{
                //    if (zz is null)
                //    {

                //    }
                //    if (zz != tPixel)
                //}
                //pixelRowSpan.IndexOf()
                
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
                //var i = pixelRowSpan.IndexOf(z);
                //if (i != -1)
                //{
                //    break;

                //}


                //for (int x = 0; x < image.Width; x++)
                //{
                //    pixelRowSpan[x] = new Rgba32(x / 255, y / 255, 50, 255);
                //}
            }


            //return image;
            //var textToApply = text
            _source.Mutate(x => x.Crop(x.GetCurrentSize().Width, y + 1));
        }

        public void Dispose()
        {
            // throw new NotImplementedException();
        }
    }
}