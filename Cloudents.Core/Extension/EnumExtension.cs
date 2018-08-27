using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cloudents.Core.Attributes;

namespace Cloudents.Core.Extension
{
    public static class EnumExtension
    {
        public static string GetDescription(this System.Enum value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            var fi = value.GetType().GetField(value.ToString());

            switch (fi.GetCustomAttributes(typeof(ParseAttribute), false).FirstOrDefault())
            {
                case null:
                    return value.ToString();
                case ParseAttribute parse:
                    return parse.Description;
            }

            throw new InvalidCastException();
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