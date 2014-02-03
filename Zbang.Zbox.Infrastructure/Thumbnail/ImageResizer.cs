using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Zbang.Zbox.Infrastructure.Thumbnail
{
    public class ImageResizer
    {
        private Size ScaleProportionally(Size destinationSize, Size imageOriginalSize, bool shouldCrop)
        {
            if (imageOriginalSize.Width <= destinationSize.Width && imageOriginalSize.Height < destinationSize.Height)
            {
                return imageOriginalSize;
            }
            int sourceWidth = imageOriginalSize.Width;
            int sourceHeight = imageOriginalSize.Height;

            float nPercentW = (destinationSize.Width / (float)sourceWidth);
            float nPercentH = (destinationSize.Height / (float)sourceHeight);

            //want a larger image so i can crop
            //TODO - do something with this.
            float nPercent = CalculateRatio(shouldCrop, nPercentW, nPercentH);


            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);
            return new Size(destWidth, destHeight);
        }

        private float CalculateRatio(bool shouldCrop, float nPercentW, float nPercentH)
        {
            if (shouldCrop)
            {
                if (nPercentH > nPercentW)
                {
                    return nPercentH;
                }
                return nPercentW;
            }
            
            return nPercentW;

        }
        public MemoryStream ResizeImageAndSave(Stream input, int height, int width, bool shouldCrop = false)
        {

            using (Image orig = Image.FromStream(input))
            {
                return ResizeImageAndSave(orig, height, width, shouldCrop);
            }
        }
        public MemoryStream ResizeImageAndSave(Image img, int height, int width, bool shouldCrop = false)
        {
            return ResizeImage(img, height, width, shouldCrop);

        }
        public MemoryStream SaveImage(Image thumb)
        {
            var ms = new MemoryStream();

            ImageCodecInfo[] info = ImageCodecInfo.GetImageEncoders(); // info 1 is jpg encoder
            EncoderParameters encoderParameters = new EncoderParameters(2);
            encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 85L);
            encoderParameters.Param[1] = new EncoderParameter(Encoder.RenderMethod, EncoderValue.RenderProgressive.ToString());


            thumb.Save(ms, info[1], encoderParameters);

            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }




        private MemoryStream ResizeImage(Image img, int height, int width, bool shouldCrop)
        {
            Size newDimentions = ScaleProportionally(new Size(width, height), new Size(img.Width, img.Height), shouldCrop);
            Bitmap thumb = new Bitmap(newDimentions.Width, newDimentions.Height);

            using (Graphics graphic = Graphics.FromImage(thumb))
            {
                graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                graphic.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                
                graphic.FillRectangle(Brushes.White, 0, 0, newDimentions.Width, newDimentions.Height);
                graphic.DrawImage(img, 0, 0, newDimentions.Width, newDimentions.Height);
            }

            if (shouldCrop)
            {
                Point leftTopCorner = new Point(0, 0);
                var cropNeeded = false;
                if (thumb.Width > width)
                {
                    leftTopCorner.X = (thumb.Width - width) / 2;
                    cropNeeded = true;

                }
                if (thumb.Height > height)
                {
                    leftTopCorner.Y = (thumb.Height - height) / 2;
                    cropNeeded = true;
                }
                if (cropNeeded)
                {
                    var cropArea = new Rectangle(leftTopCorner, new Size(width, height));
                    thumb = thumb.Clone(cropArea, thumb.PixelFormat);
                }

                //graphic.FillRectangle(Brushes.White, cropArea);
                //graphic.DrawImage(img, cropArea);
            }
            return SaveImage(thumb);


        }





    }
}
