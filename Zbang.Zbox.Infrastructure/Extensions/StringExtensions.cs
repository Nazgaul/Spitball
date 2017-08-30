using System;

namespace Zbang.Zbox.Infrastructure.Extensions
{
    public static class StringExtensions
    {
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

        public static string NullIfWhiteSpace(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return null;
            }
            return str;
        }
    }
}



