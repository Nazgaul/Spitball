using Cloudents.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Cloudents.Core.Extension
{
    public static class EnumExtension
    {
        public static string GetDescription(this System.Enum value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            var val = GetAttributeValue<ParseAttribute>(value);

            switch (val)
            {
                case null:
                    return value.ToString();
                case ParseAttribute parse:
                    return parse.Description;
            }
        }

        public static T GetAttributeValue<T>(this System.Enum value) where T : Attribute
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            var fi = value.GetType().GetField(value.ToString());
            return fi.GetCustomAttribute<T>();
        }

        public static IEnumerable<string> GetPublicEnumNames(Type value)
        {
            var memberInfos = value.GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach (var memberInfo in memberInfos)
            {
                if (memberInfo.GetCustomAttribute<PublicValueAttribute>() != null)
                {
                    yield return memberInfo.Name;
                }
            }
        }
    }
}