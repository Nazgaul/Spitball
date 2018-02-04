using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Zbang.Zbox.Infrastructure.Enums.Resources;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Zbox.Infrastructure
{
    public enum Language
    {
        Undefined,
        [ResourceDescription(typeof(EnumResources), "LanguageEnglishUs")]
        EnglishUs,
        [ResourceDescription(typeof(EnumResources), "LanguageHebrew")]
        Hebrew
    }

    public static class Languages
    {
        public static readonly ReadOnlyCollection<LanguagesDetail> SupportedCultures = new List<LanguagesDetail> {
            new LanguagesDetail(Language.EnglishUs,new [] {"en" ,"en-US"}),
            new LanguagesDetail(Language.Hebrew, new [] {"he","he-IL" }),
        }.AsReadOnly();

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
                default:
                    return new CultureInfo("en-US");
            }
        }


        public static string GetCultureBaseOnCulture(string culture)
        {
            if (string.IsNullOrEmpty(culture))
            {
                return DefaultSystemCulture.Culture[0];
            }
            var supportCulture = SupportedCultures.FirstOrDefault(s => s.Culture.Any(a => a.ToLowerInvariant().StartsWith(culture.ToLower())));
            if (supportCulture == null)
            {
                return CultureInfo.CurrentCulture.Name;
            }
            return supportCulture.Culture[0];
        }

        public static LanguagesDetail DefaultSystemCulture => SupportedCultures[0];
    }

    public class LanguagesDetail
    {
        public LanguagesDetail(Language name, string[] culture)
        {
            Name = name;
            Culture = culture;
        }

        public Language Name { get;  }
        public string[] Culture { get;  }
    }
}
