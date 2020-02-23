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

            return new CropImageProcessor<TPixel>(this, source, sourceRectangle);
            //        //var pixelToLookFor = new Rgba32(colorToWorkWith.R, colorToWorkWith.G, colorToWorkWith.B, colorToWorkWith.A);
            //       // var size = source.GetCurrentSize();
            //        int y;
            //        var z = new TPixel();
            //        z.FromRgba32(new Rgba32(0,0,0,0));
            //        //z.FromRgba32();


            //        for (y = source.Height - 1; y >= 0; y--)
            //        {

            //            Span<TPixel> pixelRowSpan = source.GetPixelRowSpan(y);
            //            //TPixel zz = new TPixel();
            //            //foreach (var tPixel in pixelRowSpan)
            //            //{
            //            //    if (zz is null)
            //            //    {

            //            //    }
            //            //    if (zz != tPixel)
            //            //}
            //            //pixelRowSpan.IndexOf()
            //            var i = pixelRowSpan.IndexOf(z);
            //            if (i != -1)
            //            {
            //                break;

            //            }


            //            //for (int x = 0; x < image.Width; x++)
            //            //{
            //            //    pixelRowSpan[x] = new Rgba32(x / 255, y / 255, 50, 255);
            //            //}
            //        }


            //        //return image;
            //        //var textToApply = text;
            //        return source.Mutate(x => x.Crop(x.GetCurrentSize().Width, y + 1));
        }



        //    public void Apply()
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public void Dispose()
        //    {
        //        throw new NotImplementedException();
        //    }
        //}
    }
}