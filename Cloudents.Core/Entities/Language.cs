using System.Collections.Generic;
using System.Globalization;

namespace Cloudents.Core.Entities
{
    public class Language : Entity<string>
    {
        protected Language()
        {

        }

        private Language(CultureInfo name)
        {
            Id = name.TwoLetterISOLanguageName;
        }

        public static readonly Language English = new Language(new CultureInfo("en"));
        public static readonly Language Hebrew = new Language(new CultureInfo("he"));


        public static IEnumerable<CultureInfo> SystemSupportLanguage()
        {
            yield return English;
            yield return Hebrew;

        }

        public static implicit operator CultureInfo(Language tb)
        {
            return new CultureInfo(tb.Id);
        }

        public static implicit operator Language(CultureInfo tb)
        {
            foreach (var cultureInfo in SystemSupportLanguage())
            {
                if (tb.Equals(cultureInfo))
                {
                    return tb;
                }
            }

            return English;
        }


    }
}