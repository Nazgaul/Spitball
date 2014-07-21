using System;


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
}



