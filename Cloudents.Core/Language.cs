using System.Collections.Generic;
using System.Globalization;

namespace Cloudents.Application
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
        public static readonly IList<CultureInfo> SystemSupportLanguage = new List<CultureInfo>()
        {
            English,Hebrew
        };
    }
}