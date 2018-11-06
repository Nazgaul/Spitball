using System;
using System.Collections.Generic;
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


        public static readonly SortedSet<string> ListOfWhiteListCountries = new SortedSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "US", "CA", "AU" , "GB", "IE", "IL", "NZ", "MX", "SE" ,
            "NO", "DK", "FI", "NL", "BE","LU","DE","CH","AT","ZA"
        };
        //public static readonly string[] ListOfWhiteListCountries2 = { "US", "CA", "AU" , "GB", "IE", "IL", "NZ", "MX", "SE" ,
        //    "NO", "DK", "FI", "NL", "BE","LU","DE","CH","AT","ZA" };
    }
}