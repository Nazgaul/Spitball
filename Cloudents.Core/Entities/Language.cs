using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Cloudents.Core.Entities
{
    public sealed class Language
    {
        private Language(CultureInfo info)
        {
            Info = info;
        }

        public CultureInfo Info { get; private set; }

        public static readonly Language English = new Language(new CultureInfo("en"));
        public static readonly Language Hebrew = new Language(new CultureInfo("he"));
        public static readonly Language EnglishIndia = new Language(new CultureInfo("en-IN"));
        public static readonly Language EnglishUsa = new Language(new CultureInfo("en-US"));
        //public static readonly Language EnglishIsrael = new Language(new CultureInfo("en-IL"));


        public static IEnumerable<Language> SystemSupportLanguage()
        {
            yield return English;
            yield return Hebrew;
            yield return EnglishIndia;
            yield return EnglishUsa;
            // yield return EnglishIsrael;
        }

        public static IEnumerable<Language> SystemSupportLanguage(bool isFrymo)
        {
            if (isFrymo)
            {
                yield return EnglishIndia;
            }
            else
            {
                yield return English;
                yield return Hebrew;
                yield return EnglishUsa;
            }
        }




        public static implicit operator CultureInfo(Language tb)
        {
            return tb.Info;
        }

        public override string ToString()
        {
            return $"{nameof(Info)}: {Info}";
        }


        public static implicit operator Language(CultureInfo info)
        {
            return AssignLanguage(info);
        }

        public static implicit operator Language(string tb)
        {
            var result = SystemSupportLanguage().FirstOrDefault(f => Equals(f.Info, new CultureInfo(tb)));
            if (result != null)
            {
                return result;
            }

            return English;
        }


        private static Language AssignLanguage(CultureInfo info)
        {
            if (info == null)
            {
                return English;
            }
            while (true)
            {
                if (Equals(info, info.Parent))
                {
                    break;
                }

                var result = SystemSupportLanguage().FirstOrDefault(f => Equals(f.Info, info));
                if (result == null)
                {
                    return AssignLanguage(info.Parent);
                }
                return result;
                //info = info.Parent;
            }

            return English;
        }
    }


    //public enum CountryGroup
    //{
    //    All,
    //    Tier3,
    //    Israel

    //}

}