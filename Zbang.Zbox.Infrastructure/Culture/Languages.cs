using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Zbang.Zbox.Infrastructure.Culture
{
    public static class Languages
    {
        public const string EnglishUSName = "English \u200E(US)\u200E";
        public const string EnglishGBName = "English \u200E(UK)\u200E";
        public const string HebrewName = "עברית";
        public const string ArabicName = "العربية";
        public const string RussianName = "Pусский";
        public const string ChineseName = "中文";

        public static readonly List<LanguagesDetail> SupportedCultures = new List<LanguagesDetail> {
            new LanguagesDetail(EnglishUSName,"en-us"),
            new LanguagesDetail(EnglishGBName,"en-gb"),
            new LanguagesDetail(HebrewName,"he-il"),
            new LanguagesDetail(ArabicName,"ar-ae"),
            new LanguagesDetail(RussianName,"ru-ru"),
            new LanguagesDetail(ChineseName,"zh-cn")
        };


        public static LanguagesDetail GetLanguageDetailByName(string name)
        {

            var result = SupportedCultures.FirstOrDefault(f => f.Name == name);
            if (result == null)
            {
                Trace.TraceLog.WriteError("GetLanguageDetailByName name" + name);
                return GetDefaultSystemCulture();
            }
            return result;
        }

        public static CultureInfo GetCultureBaseOnCountry(string countryPrefix)
        {
            switch (countryPrefix.ToLower())
            {
                case "il":
                    return new CultureInfo("he-IL");
                default:
                    return new CultureInfo("en-US");
            }

        }


        public static bool CheckIfLanguageIsSupported(string culture)
        {
            if (string.IsNullOrWhiteSpace(culture))
            {
                return false;
            }
            return SupportedCultures.Any(s => s.Culture.ToLower().StartsWith(culture.ToLower()));
        }


        public static LanguagesDetail GetDefaultSystemCulture()
        {
            return SupportedCultures[0];
        }
    }
    public class LanguagesDetail
    {
        public LanguagesDetail(string name, string culture)
        {
            Name = name;
            Culture = culture;
        }
        public string Name { get; set; }
        public string Culture { get; set; }
    }
}
