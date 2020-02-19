using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Text;
using Cloudents.Core;
using Cloudents.Core.Extension;
using Cloudents.FunctionsV2.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Shapes;
using Color = SixLabors.ImageSharp.Color;
using Font = SixLabors.Fonts.Font;
using FontStyle = SixLabors.Fonts.FontStyle;
using Image = SixLabors.ImageSharp.Image;
using PointF = SixLabors.Primitives.PointF;
using RectangleF = SixLabors.Primitives.RectangleF;
using Size = SixLabors.Primitives.Size;
using SolidBrush = SixLabors.ImageSharp.Processing.SolidBrush;

namespace Cloudents.FunctionsV2.Extensions
{
    public static class DrawTextExtensions
    {

        //public static IImageProcessingContext DrawTextWithHebrew(
        //    this IImageProcessingContext source,
        //    TextGraphicsOptions options,
        //    string text,
        //    Font font,
        //    Color color,
        //    PointF location)
        //{
        //    var textToApply = text;
        //    return source.DrawText(options, textToApply, font, color, location);
        //}


        public static IImageProcessingContext DrawText(
            this IImageProcessingContext source,
            string text,
            int fontSize,
            string color,
            Size size,
            SixLabors.Primitives.Point location)
        {

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
            //pfcoll.AddFontFile(Server.MapPath("~/Fonts/" + fontName));
            //FontFamily ff = pfcoll.Families[0];
            var colorToWorkWith = ColorTranslator.FromHtml(color);
            using (var g = Graphics.FromImage(myBitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.AntiAlias;

                //var colorToWorkWith = System.Drawing.Color.FromArgb(color.ToArgb());


                var brush = new System.Drawing.SolidBrush(colorToWorkWith);
                g.DrawString(textToDraw,
                    new System.Drawing.Font("Calibri", fontSize, System.Drawing.FontStyle.Regular, GraphicsUnit.Pixel),
                    brush,
                    new System.Drawing.RectangleF(0, 0, size.Width, size.Height),
                    new StringFormat()
                    {
                        Alignment = StringAlignment.Center,
                        Trimming = StringTrimming.Word,

                    });
                //g.DrawString("My Text very very nice",
                //    new Font("Tahoma", 20),
                //    Brushes.White,
                //    new PointF(0, 0));
            }

            using var ms = new MemoryStream();
            myBitmap.Save(ms, ImageFormat.Png);
            var image = Image.Load(ms.ToArray());

            return source.DrawImage(image, location, GraphicsOptions.Default);
        }

        public static IImageProcessingContext CropBottomEdge(this IImageProcessingContext ctx)
        {
            return ctx.ApplyProcessor(new CropImageProcessorCreator());

            // ctx.ApplyProcessor(new )
            //var pixelToLookFor = new Rgba32(colorToWorkWith.R, colorToWorkWith.G, colorToWorkWith.B, colorToWorkWith.A);
            ////    var size = ctx.GetCurrentSize();
            //int y;

            ////    ctx.ApplyProcessor()
            //for (y = ctx.Height - 1; y >= 0; y--)
            //{

            //    Span<Rgba32> pixelRowSpan = ctx.GetPixelRowSpan(y);
            //    var i = pixelRowSpan.IndexOf(pixelToLookFor);
            //    if (i != -1)
            //    {
            //        break;

            //    }
            //}
        }


        public static IImageProcessingContext ApplyRoundedCorners(this IImageProcessingContext ctx, float cornerRadius)
        {
            Size size = ctx.GetCurrentSize();
            IPathCollection corners = BuildCorners(size.Width, size.Height, cornerRadius);

            var graphicOptions = new GraphicsOptions(true)
            {
                AlphaCompositionMode =
                    PixelAlphaCompositionMode
                        .DestOut // enforces that any part of this shape that has color is punched out of the background
            };
            // mutating in here as we already have a cloned original
            // use any color (not Transparent), so the corners will be clipped
            return ctx.Fill(graphicOptions, Rgba32.LimeGreen, corners);
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
}
