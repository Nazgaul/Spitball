using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Cloudents.Core.Attributes;

namespace Cloudents.Core.Extension
{
    public static class StringExtensions
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }

        //public static bool Contains(this string source, IEnumerable<string> toCheck, StringComparison comp)
        //{
        //    foreach (var check in toCheck)
        //    {
        //        if (source.Contains(check, comp))
        //        {
        //            return true;
        //        }
        //    }

        //    return false;

        //}

        public static bool TryToEnum<TEnum>(this string value, out TEnum result) where TEnum : struct
        {
            if (System.Enum.TryParse(value, true, out result))
            {
                return true;
            }
            foreach (var field in typeof(TEnum).GetFields())
            {
                if (Attribute.GetCustomAttribute(field,
                        typeof(ParseAttribute)) is ParseAttribute attribute
                    && attribute.Description.Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    result = (TEnum)System.Enum.Parse(typeof(TEnum), field.Name);
                    return true;
                }
            }
            return false;
        }

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

        public static string RemoveWords(this string str, IEnumerable<string> occurrences)
        {
            //var sb = new StringBuilder(str);

            return Regex.Replace(str, "\\b" + string.Join("\\b|\\b", occurrences) + "\\b", "", RegexOptions.IgnoreCase);
            //foreach (var occurrence in occurrences)
            //{
            //    sb.Replace($" {occurrence} ", string.Empty);
            //}

            //return sb.ToString();
        }

        public static string ToCamelCase(this string str)
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
        /// <remarks>https://www.mikesdotnetting.com/article/139/highlighting-keywords-found-in-search-results</remarks>
        public static string HighlightKeyWords(this string text, IEnumerable<string> keywords, bool fullMatch)
        {
            if (text?.Length == 0 /*|| keywords == String.Empty*/)
                return text;

            if (!fullMatch)
            {
                return keywords.Select(word => word.Trim()).Aggregate(text,
                   (current, pattern) =>
                       Regex.Replace(current,
                           pattern,
"<b>$0</b>",
                           RegexOptions.IgnoreCase));
            }

            return keywords.Select(word => "\\b" + word.Trim() + "\\b")
                .Aggregate(text, (current, pattern) =>
                    Regex.Replace(current,
                        pattern,
"<b>$0</b>",
                        RegexOptions.IgnoreCase));
        }


        private static string DecodeHtmlEntities(this string text)
        {
            return HttpUtility.HtmlDecode(text);
        }

        public static string StripAndDecode(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }
            return RegEx.RemoveHtmlTags.Replace(text, string.Empty).DecodeHtmlEntities();
        }

        public static string Encrypt(this string text, string keyString)
        {
            var key = Encoding.UTF8.GetBytes(keyString);

            using (var aesAlg = Aes.Create())
            {
                using (var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV))
                {
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(text);
                        }

                        var iv = aesAlg.IV;

                        var decryptedContent = msEncrypt.ToArray();

                        var result = new byte[iv.Length + decryptedContent.Length];

                        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                        Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

                        return Convert.ToBase64String(result);
                    }
                }
            }
        }

        public static string Decrypt(this string cipherText, string keyString)
        {
            var fullCipher = Convert.FromBase64String(cipherText);

            var iv = new byte[16];
            var cipher = new byte[16];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, iv.Length);
            var key = Encoding.UTF8.GetBytes(keyString);

            using (var aesAlg = Aes.Create())
            {
                using (var decryptor = aesAlg.CreateDecryptor(key, iv))
                {
                    string result;
                    using (var msDecrypt = new MemoryStream(cipher))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                result = srDecrypt.ReadToEnd();
                            }
                        }
                    }

                    return result;
                }
            }
        }
    }
}
