using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace Zbang.Zbox.Infrastructure.Culture
{
    public static class Languages
    {
        public const string EnglishUsName = "English";
        public const string HebrewName = "עברית";


        public static readonly ReadOnlyCollection<LanguagesDetail> SupportedCultures = new List<LanguagesDetail> {
            new LanguagesDetail(EnglishUsName,new [] {"en" ,"en-US"}),
            new LanguagesDetail(HebrewName, new [] {"he","he-IL" }),
        }.AsReadOnly();

        /// <summary>
        /// used by razor to generate the languages
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static LanguagesDetail GetLanguageDetailByName(string name)
        {

            var result = SupportedCultures.FirstOrDefault(f => f.Name == name);
            if (result == null)
            {
                Trace.TraceLog.WriteError("GetLanguageDetailByName name" + name);
                return DefaultSystemCulture;
            }
            return result;
        }

        public static CultureInfo GetCultureBaseOnCountry(string countryPrefix)
        {
            if (string.IsNullOrEmpty(countryPrefix))
            {
                return new CultureInfo("en-US");
            }
            switch (countryPrefix.ToLowerInvariant())
            {
                case "il":
                    return new CultureInfo("he-IL");
                //case "ru":
                //    return new CultureInfo("ru-RU");
                //case "gb":
                //    return new CultureInfo("en-GB");
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
            return SupportedCultures.Any(a => a.Culture.Any(a1 => String.Equals(a1, culture, StringComparison.CurrentCultureIgnoreCase)));
            //return SupportedCultures.Any(s => 
            //    String.Equals(s.Culture, culture, StringComparison.CurrentCultureIgnoreCase));
        }

        public static string GetCultureBaseOnCulture(string culture)
        {
            if (string.IsNullOrEmpty(culture))
            {
                return DefaultSystemCulture.Culture.First();
            }
            var supportCulture = SupportedCultures.FirstOrDefault(s => s.Culture.Any(a => a.ToLowerInvariant().StartsWith(culture.ToLower())));
            if (supportCulture == null)
            {
                return CultureInfo.CurrentCulture.Name;
            }
            return supportCulture.Culture.First();
        }


        public static LanguagesDetail DefaultSystemCulture => SupportedCultures[0];
       
    }
    public class LanguagesDetail
    {
        public LanguagesDetail(string name, string[] culture)
        {
            Name = name;
            Culture = culture;
        }
        public string Name { get; set; }
        public string[] Culture { get; set; }
    }
}
