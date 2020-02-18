using System;
using System.Collections.Generic;
using System.Text;
using Cloudents.Core;
using Cloudents.Core.Extension;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using SixLabors.Shapes;

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
            return source.DrawText(options,text.ReverseOnlyHebrew(), font, color, location);
        }

        public static IImageProcessingContext ApplyRoundedCorners(this IImageProcessingContext ctx, float cornerRadius)
        {
            Size size = ctx.GetCurrentSize();
            IPathCollection corners = BuildCorners(size.Width, size.Height, cornerRadius);

            var graphicOptions = new GraphicsOptions(true)
            {
                AlphaCompositionMode = PixelAlphaCompositionMode.DestOut // enforces that any part of this shape that has color is punched out of the background
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


}
