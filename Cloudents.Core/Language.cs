using System;
using System.Globalization;

namespace Cloudents.Core
{
    public sealed class Language
    {
        public CultureInfo Culture { get; }

        private Language(CultureInfo culture)
        {
            Culture = culture;
        }

        public static readonly Language Hebrew = new Language(new CultureInfo("he"));
        public static readonly Language English = new Language(new CultureInfo("en"));


        public static implicit operator CultureInfo(Language tb)
        {
            return tb.Culture;
        }
    }

    //public sealed class Country
    //{
    //    public Language DefaultLanguage { get; }

    //    public string Symbol { get;  }

    //    private Country(Language culture, string symbol)
    //    {
    //        DefaultLanguage = culture;
    //        Symbol = symbol;
    //    }

    //    public static readonly Country Israel = new Country(Language.Hebrew, "il");


    //    public static bool operator ==(Country x, string y)
    //    {
    //        return string.Equals(x?.Symbol, y, StringComparison.OrdinalIgnoreCase);
    //    }

    //    public static bool operator !=(Country x, string y)
    //    {
    //        return !(x == y);
    //    }
    //}
}