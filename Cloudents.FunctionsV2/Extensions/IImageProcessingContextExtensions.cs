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
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors;
using SixLabors.Shapes;
using Color = SixLabors.ImageSharp.Color;
using Font = SixLabors.Fonts.Font;
using FontStyle = SixLabors.Fonts.FontStyle;
using Image = SixLabors.ImageSharp.Image;
using PointF = SixLabors.Primitives.PointF;
using Rectangle = SixLabors.Primitives.Rectangle;
using RectangleF = SixLabors.Primitives.RectangleF;
using Size = SixLabors.Primitives.Size;
using SolidBrush = SixLabors.ImageSharp.Processing.SolidBrush;

namespace Cloudents.FunctionsV2.Extensions
{
    public static class DrawTextExtensions
    {

        public static IImageProcessingContext DrawTextWithHebrew(
            this IImageProcessingContext source,
            TextGraphicsOptions options,
            string text,
            Font font,
            Color color,
            PointF location)
        {
            var textToApply = text;
            return source.DrawText(options, textToApply, font, color, location);
        }


        public static IImageProcessingContext DrawText(
            this IImageProcessingContext source,
            string text,
            int fontSize,
            string color,
            Size size,
            SixLabors.Primitives.Point location)
        {

            using var myBitmap = new Bitmap(size.Width, size.Height + 10);



            //pfcoll.AddFontFile(Server.MapPath("~/Fonts/" + fontName));
            //FontFamily ff = pfcoll.Families[0];
            var colorToWorkWith = ColorTranslator.FromHtml(color);
            using (var g = Graphics.FromImage(myBitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.AntiAlias;

                //var colorToWorkWith = System.Drawing.Color.FromArgb(color.ToArgb());


                var brush = new System.Drawing.SolidBrush(colorToWorkWith);
                g.DrawString(text,
                    new System.Drawing.Font("Calibri", fontSize, System.Drawing.FontStyle.Regular, GraphicsUnit.Pixel),
                    brush,
                    new System.Drawing.RectangleF(0, 0, size.Width, size.Height),
                    new StringFormat()
                    {
                        Alignment = StringAlignment.Center,
                        Trimming = StringTrimming.EllipsisWord,

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
            return ctx.ApplyProcessor(new CropImageProcessor());

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


        //public static string ReverseOnlyHebrew3(string t)
        //{
        //    var aa = new List<string>();
        //    foreach (var s in t.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        //    {
        //        aa.Add(ReverseOnlyHebrew(s));
        //    }

        //    return string.Join(" ", aa);
        //}

        //public static string ReverseOnlyHebrew2(string t)
        //{

        //    char[] charArray = t.ToCharArray();
        //    string a = "";
        //    int last = 0;
        //    for (int i = 0; i <= charArray.Length - 1; i++)
        //    {

        //        if (!IsHebrew(charArray[i]))
        //        {
        //            List<char> temp = new List<char>();

        //            for (; last < i; last++)
        //            {
        //                int k = 0;
        //                temp.Insert(0, charArray[last]);
        //            }

        //            foreach (char g in temp)
        //            {
        //                a += g.ToString();
        //            }
        //            a += charArray[i];
        //            last += 1;
        //        }
        //    }

        //    return a;
        //}
        //private const char FirstHebChar = (char)1488; //א
        //private const char LastHebChar = (char)1514; //ת
        //private static bool IsHebrew(char c)
        //{
        //    return c >= FirstHebChar && c <= LastHebChar;
        //}


        //private static string ReverseOnlyHebrew(string str)
        //{
        //    if (string.IsNullOrEmpty(str))
        //    {
        //        return str;
        //    }

        //    var arrSplit = RegEx.RtlLettersSentence.Split(str);
        //    var result = new StringBuilder();


        //    for (var i =0 ;i < arrSplit.Length ; i++)
        //    //for (int i = arrSplit.Length - 1; i >= 0; i--)
        //    {
        //        if (arrSplit[i] == "")
        //        {
        //            continue;

        //        }
        //        if (arrSplit[i] == " ")
        //        {
        //            result.Append(" ");
        //            continue;
        //        }

        //        if (int.TryParse(arrSplit[i], out var outInt))
        //        {
        //            result.Append(outInt);
        //            continue;
        //            // str += Convert.ToInt32(arrSplit[i]);
        //        }

        //        arrSplit[i] = arrSplit[i].Trim();
        //        byte[] codes = Encoding.Default.GetBytes(arrSplit[i].ToCharArray(), 0, 1);
        //        if (codes[0] > 47 && codes[0] < 58 || codes[0] > 64 && codes[0] < 91 || codes[0] > 96 && codes[0] < 123)//EDIT 3.1 reverse just hebrew words                              
        //        {
        //            result.Append(arrSplit[i].Trim());

        //        }
        //        else
        //        {
        //            result.Append(Reverse(arrSplit[i]));
        //        }
        //    }

        //    return result.ToString();
        //}
        //private static string Reverse(string str)
        //{
        //    char[] strArray = str.ToCharArray();
        //    Array.Reverse(strArray);
        //    return new string(strArray);
        //}
    }

    public class CropImageProcessorXXX<TPixel> : IImageProcessor<TPixel> where TPixel : struct, IPixel<TPixel>
    {
        private readonly CropImageProcessor _cropImageProcessor;
        private readonly Image<TPixel> _source;
        private readonly Rectangle _sourceRectangle;

        public CropImageProcessorXXX(CropImageProcessor cropImageProcessor, Image<TPixel> source,
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


    public class CropImageProcessor : IImageProcessor
    {




        public IImageProcessor<TPixel> CreatePixelSpecificProcessor<TPixel>(Image<TPixel> source,
            Rectangle sourceRectangle)
            where TPixel : struct, IPixel<TPixel>
        {

            return new CropImageProcessorXXX<TPixel>(this, source, sourceRectangle);
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
