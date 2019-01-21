﻿using System.Collections.Generic;
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


        public static IEnumerable<Language> SystemSupportLanguage()
        {
            yield return English;// new CultureInfo(English.Id);
            yield return Hebrew;// new CultureInfo(Hebrew.Id);

        }



        public static implicit operator CultureInfo(Language tb)
        {
            return new CultureInfo(tb.Id);
        }

        //public static implicit operator Language(string tb)
        //{
        //    if (SystemSupportLanguage()
        //    {
        //        return tb;
        //    }

        //    return English;
        //}
        public static implicit operator Language(CultureInfo info)
        {
            var result = SystemSupportLanguage().FirstOrDefault(f => f.Id == info.Name);
            if (result != null)
            {
                return result;
            }
            //if (SystemSupportLanguage().Contains(info.Name))
            //{
            //    return tb;
            //}

            return English;
        }

        public static implicit operator Language(string tb)
        {
            var result = SystemSupportLanguage().FirstOrDefault(f=>f.Id == tb);
            if (result != null)
            {
                return result;
            }

            //if (SystemSupportLanguage().Contains(tb))
            //{
            //    return tb;
            //}

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


        public static implicit operator string(SystemEvent tb)
        {
            return tb.Id;
        }
    }
}