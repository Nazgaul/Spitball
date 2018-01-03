﻿using System;
using Cloudents.Core.Enum;

namespace Cloudents.Core.Extension
{
   
    public static class StringExtensions
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }

        public static bool TryToEnum<TEnum>(this string value, out TEnum result) where TEnum : struct
        {
            if (System.Enum.TryParse(value, out result))
            {
                return true;
            }
            foreach (var field in typeof(TEnum).GetFields())
            {
                if (Attribute.GetCustomAttribute(field,
                        typeof(ParseAttribute)) is ParseAttribute attribute
                    && attribute.Description.Equals(value, StringComparison.InvariantCultureIgnoreCase))
                {
                    result = (TEnum)System.Enum.Parse(typeof(TEnum), field.Name);
                    return true;
                }
            }
            return false;
        }

        public static string RemoveEndOfString(this string word, int length)
        {
            return word?.Substring(0, Math.Min(word.Length, length));
        }

        public static string UppercaseFirst(this string str)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpperInvariant(str[0]) + str.Substring(1).ToLowerInvariant();
        }
    }
}
