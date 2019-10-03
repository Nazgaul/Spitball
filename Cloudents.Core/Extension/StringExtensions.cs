using System;
using System.Collections.Generic;
using System.Web;

namespace Cloudents.Core.Extension
{
    public static class StringExtensions
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }

        public static bool Contains(this string source, IEnumerable<string> toCheck, StringComparison comp)
        {
            foreach (var check in toCheck)
            {
                if (source.Contains(check, comp))
                {
                    return true;
                }
            }

            return false;

        }

        //public static bool TryToEnum<TEnum>(this string value, out TEnum result) where TEnum : struct
        //{
        //    if (System.Enum.TryParse(value, true, out result))
        //    {
        //        return true;
        //    }
        //    foreach (var field in typeof(TEnum).GetFields())
        //    {
        //        if (Attribute.GetCustomAttribute(field,
        //                typeof(ParseAttribute)) is ParseAttribute attribute
        //            && attribute.Description.Equals(value, StringComparison.OrdinalIgnoreCase))
        //        {
        //            result = (TEnum)System.Enum.Parse(typeof(TEnum), field.Name);
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        //public static string RemoveEndOfString(this string word, int length)
        //{
        //    return word?.Substring(0, Math.Min(word.Length, length));
        //}

        public static string Truncate(this string value, int maxChars, bool threeDotsAtTheEnd = false)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value.Length <= maxChars)
            {
                return value;
            }

            var concatString = value.Substring(0, maxChars);
            if (threeDotsAtTheEnd)
            {
                return concatString + "...";
            }

            return concatString;
        }

        //public static string UppercaseFirst(this string str)
        //{
        //    // Check for empty string.
        //    if (string.IsNullOrEmpty(str))
        //    {
        //        return string.Empty;
        //    }
        //    // Return char and concat substring.
        //    return char.ToUpperInvariant(str[0]) + str.Substring(1).ToLowerInvariant();
        //}

        //public static string RemoveWords(this string str, IEnumerable<string> occurrences)
        //{
        //    //var sb = new StringBuilder(str);

        //    return Regex.Replace(str, "\\b" + string.Join("\\b|\\b", occurrences) + "\\b", "", RegexOptions.IgnoreCase);
        //    //foreach (var occurrence in occurrences)
        //    //{
        //    //    sb.Replace($" {occurrence} ", string.Empty);
        //    //}

        //    //return sb.ToString();
        //}

        public static string ToCamelCase(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToLowerInvariant(str[0]) + str.Substring(1);
        }

       
        // <remarks>https://www.mikesdotnetting.com/article/139/highlighting-keywords-found-in-search-results</remarks>
//        public static string HighlightKeyWords(this string text, IEnumerable<string> keywords, bool fullMatch)
//        {
//            if (text?.Length == 0 /*|| keywords == String.Empty*/)
//                return text;

//            if (!fullMatch)
//            {
//                return keywords.Select(word => word.Trim()).Aggregate(text,
//                   (current, pattern) =>
//                       Regex.Replace(current,
//                           pattern,
//"<b>$0</b>",
//                           RegexOptions.IgnoreCase));
//            }

//            return keywords.Select(word => "\\b" + word.Trim() + "\\b")
//                .Aggregate(text, (current, pattern) =>
//                    Regex.Replace(current,
//                        pattern,
//"<b>$0</b>",
//                        RegexOptions.IgnoreCase));
//        }


        private static string DecodeHtmlEntities(this string text)
        {
            return HttpUtility.HtmlDecode(text);
        }

        //public static string StripAndDecode(this string text)
        //{
        //    if (string.IsNullOrEmpty(text))
        //    {
        //        return text;
        //    }
        //    return RegEx.RemoveHtmlTags.Replace(text, string.Empty).DecodeHtmlEntities();
        //}

       
    }
}
