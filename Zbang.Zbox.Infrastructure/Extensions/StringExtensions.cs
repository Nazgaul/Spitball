using System;
using System.Text;
using System.Text.RegularExpressions;


// ReSharper disable once CheckNamespace -- this is extension
public static class StringExtensions
{
    public static string RemoveEndOfString(this String word, int length)
    {
        if (word == null) throw new ArgumentNullException("word");
        return word.Substring(0, Math.Min(word.Length, length));
    }

    public static string UppercaseFirst(this String s)
    {
        // Check for empty string.
        if (string.IsNullOrEmpty(s))
        {
            return string.Empty;
        }
        // Return char and concat substring.
        return char.ToUpper(s[0]) + s.Substring(1);
    }

    public static string TrimEnd(this String s, params string[] remove)
    {
        



        foreach (string item in remove)
            if (s.EndsWith(item))
            {
                s = s.Substring(0, s.LastIndexOf(item, StringComparison.Ordinal));
                break; //only allow one match at most
            }
        //if (!string.IsNullOrEmpty(value))
        //{
        //    while (!string.IsNullOrEmpty(s) && s.EndsWith(value, comparisonType))
        //    {
        //        s = s.Substring(0, (s.Length - value.Length));
        //    }
        //}

        return s;
    }
}



