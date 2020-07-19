using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using Cloudents.FunctionsV2.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Color = SixLabors.ImageSharp.Color;
using Image = SixLabors.ImageSharp.Image;
using Point = SixLabors.ImageSharp.Point;
using Size = SixLabors.ImageSharp.Size;


namespace Cloudents.FunctionsV2.Extensions
{
    public static class DrawTextExtensions
    {
        public static IImageProcessingContext DrawText(
            this IImageProcessingContext source,
            string? text,
            int fontSize,
            string color,
            Size size,
            Point location)
        {
            if (text == null)
            {
                return source;
            }
            using var myBitmap = new Bitmap(size.Width, size.Height + 10);

            var x = new Span<char>(text.ToCharArray());

            for (int i = x.Length - 1; i >= 0; i--)
            {
                
                if (char.IsPunctuation(x[i]) || char.IsSeparator(x[i]) || char.IsSymbol(x[i]))
                {
                    x = x.Slice(0, i);

                }
                else
                {
                    break;

                }
            }

            var textToDraw = new string(x);
            var colorToWorkWith = ColorTranslator.FromHtml(color);
            using (var g = Graphics.FromImage(myBitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.AntiAlias;

                var brush = new System.Drawing.SolidBrush(colorToWorkWith);
                g.DrawString(textToDraw,
                    new Font("Calibri", fontSize, FontStyle.Regular, GraphicsUnit.Pixel),
                    brush,
                    new System.Drawing.RectangleF(0, 0, size.Width, size.Height),
                    new StringFormat()
                    {
                        Alignment = StringAlignment.Center,
                        Trimming = StringTrimming.Word,

                    });
            }

            using var ms = new MemoryStream();
            myBitmap.Save(ms, ImageFormat.Png);
            var image = Image.Load(ms.ToArray());

            
            return source.DrawImage(image, location, new GraphicsOptions());
        }

        public static IImageProcessingContext CropBottomEdge(this IImageProcessingContext ctx)
        {
            return ctx.ApplyProcessor(new CropImageProcessorCreator());
        }


        public static IImageProcessingContext ApplyRoundedCorners(this IImageProcessingContext ctx, float cornerRadius)
        {
            var size = ctx.GetCurrentSize();
            IPathCollection corners = BuildCorners(size.Width, size.Height, cornerRadius);

           
            ctx.SetGraphicsOptions(new GraphicsOptions()
            {
                Antialias = true,
                AlphaCompositionMode = PixelAlphaCompositionMode.DestOut // enforces that any part of this shape that has color is punched out of the background
            });
            // mutating in here as we already have a cloned original
            // use any color (not Transparent), so the corners will be clipped
            //ctx.ApplyProcessor()
            foreach (var c in corners)
            {
                ctx = ctx.Fill(Color.Red, c);
            }
            return ctx;//.Fill(Color.LimeGreen, corners);
        }

        private static IPathCollection BuildCorners(int imageWidth, int imageHeight, float cornerRadius)
        {
            // first create a square
            var rect = new RectangularPolygon(-0.5f, -0.5f, cornerRadius, cornerRadius);

            // then cut out of the square a circle so we are left with a corner
            IPath cornerTopLeft = rect.Clip(new EllipsePolygon(cornerRadius - 0.5f, cornerRadius - 0.5f, cornerRadius));

            // corner is now a corner shape positions top left
            //lets make 3 more positioned correctly, we can do that by translating the original around the center of the image

            float rightPos = imageWidth - cornerTopLeft.Bounds.Width + 1;
            float bottomPos = imageHeight - cornerTopLeft.Bounds.Height + 1;

            // move it across the width of the image - the width of the shape
            IPath cornerTopRight = cornerTopLeft.RotateDegree(90).Translate(rightPos, 0);
            IPath cornerBottomLeft = cornerTopLeft.RotateDegree(-90).Translate(0, bottomPos);
            IPath cornerBottomRight = cornerTopLeft.RotateDegree(180).Translate(rightPos, bottomPos);

            return new PathCollection(cornerTopLeft, cornerBottomLeft, cornerTopRight, cornerBottomRight);
        }



    }



    public static class ImageExtensions
    {
        public static Stream SaveAsJpeg(this Image image)
        {
            var ms = new MemoryStream();
            image.SaveAsJpeg(ms,new JpegEncoder()
            {
                Quality = 80
            });
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }
    }
}
