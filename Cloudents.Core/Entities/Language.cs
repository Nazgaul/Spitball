using System.Collections.Generic;
using System.Globalization;
using System.Linq;

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
            if (SystemSupportLanguage().Contains(tb))
            {
                return tb;
            }

            return English;
        }
    }

    public class SystemEvent : Entity<string>
    {
        protected SystemEvent()
        {

        }

        private SystemEvent(string name)
        {
            Id = name;
        }

        public static readonly SystemEvent DocumentPurchased = new SystemEvent("DocumentPurchased");
    }

    public class Email : Entity<int>
    {
        public virtual string Subject { get; set; }
        public virtual EmailBlock EmailBlock1 { get; set; }
        public virtual EmailBlock EmailBlock2 { get; set; }
        public virtual bool SocialShare { get; set; }
        public virtual SystemEvent Event { get; set; }
        public virtual Language Language { get; set; }

    }

    public class EmailBlock
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Body { get; set; }

        public string Cta { get; set; }
    }
}