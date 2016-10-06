using System;


// ReSharper disable once CheckNamespace -- this is extension
public static class StringExtensions
{
    public static string RemoveEndOfString(this string word, int length)
    {
        return word?.Substring(0, Math.Min(word.Length, length));
    }

    public static string UppercaseFirst(this string s)
    {
        // Check for empty string.
        if (string.IsNullOrEmpty(s))
        {
            return string.Empty;
        }
        // Return char and concat substring.
        return char.ToUpper(s[0]) + s.Substring(1).ToLowerInvariant();
    }

    //public static string TrimEnd(this string s, params string[] remove)
    //{
    //    foreach (string item in remove)
    //        if (s.EndsWith(item))
    //        {
    //            s = s.Substring(0, s.LastIndexOf(item, StringComparison.Ordinal));
    //            break; //only allow one match at most
    //        }
    //    //if (!string.IsNullOrEmpty(value))
    //    //{
    //    //    while (!string.IsNullOrEmpty(s) && s.EndsWith(value, comparisonType))
    //    //    {
    //    //        s = s.Substring(0, (s.Length - value.Length));
    //    //    }
    //    //}

    //    return s;
    //}

    //public static string Replace(this string s, char[] separators, string newVal)
    //{
        
    //    if (s.IndexOf(separators[0]) > -1)
    //    {
            
    //    }
    //    string[] temp = s.Split(separators, StringSplitOptions.RemoveEmptyEntries);
    //    return string.Join(newVal, temp);
    //}
}



