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

        public static Language Hebrew = new Language(new CultureInfo("he"));
        public static readonly Language English = new Language(new CultureInfo("en"));


        public static readonly string[] ListOfWhiteListCountries = { "US", "CA", "AU" , "GB", "IE", "IL", "NZ", "MX", "SE" ,
            "NO", "DK", "FI", "NL", "BE","LU","DE","CH","AT","ZA" };
    }
}