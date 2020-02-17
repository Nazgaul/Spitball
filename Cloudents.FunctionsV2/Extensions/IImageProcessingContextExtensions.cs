using System;
using System.Collections.Generic;
using System.Text;
using Cloudents.Core;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

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
            return source.DrawText(options,ReverseOnlyHebrew(text), font, color, location);
        }




        private static string ReverseOnlyHebrew(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            var arrSplit = RegEx.RtlLettersSentence.Split(str);
            var result = new StringBuilder();

            
            for (int i = arrSplit.Length - 1; i >= 0; i--)
            {
                if (arrSplit[i] == "")
                {
                    continue;

                }
                if (arrSplit[i] == " ")
                {
                    result.Append(" ");
                    continue;
                }

                if (int.TryParse(arrSplit[i], out var outInt))
                {
                    result.Append(outInt);
                    continue;
                    // str += Convert.ToInt32(arrSplit[i]);
                }

                arrSplit[i] = arrSplit[i].Trim();
                byte[] codes = Encoding.Default.GetBytes(arrSplit[i].ToCharArray(), 0, 1);
                if (codes[0] > 47 && codes[0] < 58 || codes[0] > 64 && codes[0] < 91 || codes[0] > 96 && codes[0] < 123)//EDIT 3.1 reverse just hebrew words                              
                {
                    result.Append(arrSplit[i].Trim());

                }
                else
                {
                    result.Append(Reverse(arrSplit[i]));
                }
            }

            return result.ToString();
        }
        private static string Reverse(string str)
        {
            char[] strArray = str.ToCharArray();
            Array.Reverse(strArray);
            return new string(strArray);
        }
    }


}
