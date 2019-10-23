using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Cloudents.Core.Attributes;

namespace Cloudents.Core.Extension
{
    public static class EnumExtension
    {
        //public static string GetDescription(this System.Enum value)
        //{
        //    if (value == null) throw new ArgumentNullException(nameof(value));
        //    var val = GetAttributeValue<ParseAttribute>(value);

        //    switch (val)
        //    {
        //        case null:
        //            return value.ToString();
        //        case ParseAttribute parse:
        //            return parse.Description;
        //    }
        //}

        public static string GetEnumLocalization(this System.Enum value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            var val = GetAttributeValue<ResourceDescriptionAttribute>(value);
            if (val == null)
            {
                return value.ToString("G");
            }

            if (val.ResourceType == null) return val.Description;
            var x = new System.Resources.ResourceManager(val.ResourceType);
            return x.GetString(val.ResourceName, CultureInfo.CurrentUICulture);
        }

        //public static IEnumerable<string> GetEnumLocalizationAllValues(this System.Enum value)
        //{
        //    if (value == null) throw new ArgumentNullException(nameof(value));

        //    var val = GetAttributeValue<ResourceDescriptionAttribute>(value);
        //    if (val == null)
        //    {
        //        yield return value.ToString("G");
               
        //    }
        //    else
        //    {
        //        if (val.ResourceType == null)
        //        {
        //            yield return val.Description;
        //        }
        //        else
        //        {
        //            var x = new System.Resources.ResourceManager(val.ResourceType);
        //            foreach (var cultureInfo in Language.SystemSupportLanguage())
        //            {
        //                yield return x.GetString(val.ResourceName, cultureInfo);
        //            }
        //        }
        //    }
        //}

        private static T GetAttributeValue<T>(this System.Enum value) where T : Attribute
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            var fi = value.GetType().GetField(value.ToString());
            return fi.GetCustomAttribute<T>();
        }

        public static IEnumerable<T> GetValues<T>() where T : System.Enum
        {
            return (T[])System.Enum.GetValues(typeof(T));
        }
    }
}