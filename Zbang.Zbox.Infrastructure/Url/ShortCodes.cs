using System;
using System.Collections.Generic;

namespace Zbang.Zbox.Infrastructure.Url
{
    internal static class ShortCodes
    {
        static readonly Dictionary<ShortCodesType, ShortCodeConst> ShortCodeConstsDictionary;
        static ShortCodes()
        {
            ShortCodeConstsDictionary = new Dictionary<ShortCodesType, ShortCodeConst> {
                {ShortCodesType.Box,new BoxshortCodeConst()},
                {ShortCodesType.Item,new ItemshortCodeConst()}
                //{ShortCodesType.User,new UserShortCodeConst()}
            };
        }

        public static string LongToShortCode(long number, ShortCodesType code = ShortCodesType.Box)
        {            
            var shortcodeKeyspace = ShortCodeConstsDictionary[code].GetCodeHast;
            var shortcodeXor = ShortCodeConstsDictionary[code].GetCodeXor;
            var ksLen = shortcodeKeyspace.Length;
            var scResult = string.Empty;
            var numToEncode = number ^ shortcodeXor;
            do
            {
                scResult = shortcodeKeyspace[(int)(numToEncode % ksLen)] + scResult;
                numToEncode = ((numToEncode - (numToEncode % ksLen)) / ksLen);
            }
            while (numToEncode != 0);
            return scResult;
        }

        public static long ShortCodeToLong(string shortcode, ShortCodesType code)
        {
            string shortcodeKeyspace = ShortCodeConstsDictionary[code].GetCodeHast;
            long shortcodeXor = ShortCodeConstsDictionary[code].GetCodeXor;

            var ksLen = shortcodeKeyspace.Length;
            long scResult = 0;
            var scLength = shortcode.Length;
            var codeToDecode = shortcode;
            for (var i = 0; i < codeToDecode.Length; i++)
            {
                scLength--;
                var codeChar = codeToDecode[i];
                scResult += shortcodeKeyspace.IndexOf(codeChar) * (long)(Math.Pow(ksLen, scLength));
            }

            return scResult ^ shortcodeXor;
        }

        abstract class ShortCodeConst
        {
            public abstract string GetCodeHast { get; }
            public abstract long GetCodeXor { get; }
        }

        class BoxshortCodeConst : ShortCodeConst
        {
            const string ShortcodeKeyspace = "TKZ04Vy936lEaFBtv8bxjiSOczUI7hR1GHuwYLDWpq5dMXJmkANof2rCQsgenP";
            const long ShortcodeXor = 8742710247705968543;


            public override string GetCodeHast { get { return ShortcodeKeyspace; } }
            public override long GetCodeXor { get { return ShortcodeXor; } }
        }

        class ItemshortCodeConst : ShortCodeConst
        {
            const string ShortcodeKeyspace = "jw0pUlRDYmkObN91EKh54PFd6QSMB7J3WeGg8frqHXInsvuzAaZciVtxT2oCLy";
            const long ShortcodeXor = 2613247983192947260;
            public override string GetCodeHast { get { return ShortcodeKeyspace; } }
            public override long GetCodeXor { get { return ShortcodeXor; } }
        }

        class UserShortCodeConst : ShortCodeConst
        {
            const string ShortcodeKeyspace = "yUseufp6FlT0q1H7gdwJmi9YXLb8tChaOSr3ojMAD5ZBc2GRIEN4xnkVvWKPQz";
            const long ShortcodeXor = 8432764110506944251;
            public override string GetCodeHast { get { return ShortcodeKeyspace; } }
            public override long GetCodeXor { get { return ShortcodeXor; } }
           
        }


      

        //2154954112509818557
        //Yl46ErcADwPGX0niyVCM8Bo2TR9xbv1jeKk5F7HQdsU3IufzJOSqZhNtamLgpW
    }
    public enum ShortCodesType
    {
        Box = 1,
        Item = 2
       // User = 3
    }



}
