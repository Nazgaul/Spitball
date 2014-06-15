using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Zbang.Zbox.Infrastructure.Culture
{
    public static class Languages
    {
        public const string EnglishUsName = "English \u200E(US)\u200E";
        public const string EnglishGbName = "English \u200E(UK)\u200E";
        public const string HebrewName = "עברית";
        public const string ArabicName = "العربية";
        public const string RussianName = "Pусский";
        public const string ChineseName = "中文";

        public static readonly List<LanguagesDetail> SupportedCultures = new List<LanguagesDetail> {
            new LanguagesDetail(EnglishUsName,"en-US"),
            new LanguagesDetail(EnglishGbName,"en-GB"),
            new LanguagesDetail(HebrewName,"he-IL"),
            new LanguagesDetail(ArabicName,"ar-AE"),
            new LanguagesDetail(RussianName,"ru-RU"),
            new LanguagesDetail(ChineseName,"zh-CN")
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
            if (string.IsNullOrEmpty(countryPrefix))
            {
                return new CultureInfo("en-US");
            }
            switch (countryPrefix.ToLower())
            {
                case "il":
                    return new CultureInfo("he-IL");
                case "ru" :
                    return new CultureInfo("ru-RU");
                case "gb" :
                    return new CultureInfo("en-GB");
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
