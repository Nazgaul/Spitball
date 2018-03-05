using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Cloudents.Core.Attributes;

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

        public static string CamelCase(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToLowerInvariant(str[0]) + str.Substring(1);
        }

        /// <summary>
        /// Wraps matched strings in HTML span elements styled with a background-color
        /// </summary>
        /// <param name="text"></param>
        /// <param name="keywords">Comma-separated list of strings to be highlighted</param>
        /// <param name="fullMatch">false for returning all matches, true for whole word matches only</param>
        /// <returns>string</returns>
        public static string HighlightKeyWords(this string text, string[] keywords,  bool fullMatch)
        {

            if (text?.Length == 0 /*|| keywords == String.Empty*/)
                return text;
            if (keywords?.Length == 0)
            {
                return text;
            }
            if (!fullMatch)
            {
                return keywords.Select(word => word.Trim()).Aggregate(text,
                   (current, pattern) =>
                       Regex.Replace(current,
                           pattern,
$"<b>{"$0"}</b>",
                           RegexOptions.IgnoreCase));
            }

            return keywords.Select(word => "\\b" + word.Trim() + "\\b")
                .Aggregate(text, (current, pattern) =>
                    Regex.Replace(current,
                        pattern,
$"<b>{"$0"}</b>",
                        RegexOptions.IgnoreCase));

        }
    }
}
