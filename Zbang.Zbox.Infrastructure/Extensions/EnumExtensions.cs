using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Zbang.Zbox.Infrastructure.Extensions;


// ReSharper disable once CheckNamespace - this is an extension
public static class EnumExtension
{
    public static string GetStringValue(this string value)
    {
        var separation = Regex.Matches(value, "[A-Z][a-z]*");
        if (separation.Count == 0)
        {
            return value;
        }

        if (separation.Count == 1)
        {
            return separation[0].ToString();
        }

        string separateWord = string.Empty;

        foreach (var word in separation)
        {
            separateWord += word + " ";
        }

        return separateWord;
    }
    public static string GetStringValue(this Enum value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        return GetStringValue(value.ToString("G"));
    }


    public static string GetStringValueLowercase(this Enum value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        return value.ToString("G").Substring(0, 1).ToLower() + value.ToString("G").Substring(1);
    }

    public static string GetEnumDescription(this Enum value)
    {
        return GetEnumDescription(value, CultureInfo.CurrentCulture);
    }
    public static string GetEnumDescription(this Enum value, CultureInfo culture)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        var fi = value.GetType().GetField(value.ToString());

        var attributes = (ResourceDescriptionAttribute[])fi.GetCustomAttributes(typeof(ResourceDescriptionAttribute), false);

        if ((attributes.Length <= 0)) return value.GetStringValue();
        var att = attributes[0];
        if (att.ResourceType == null) return att.Description;
        var x = new System.Resources.ResourceManager(att.ResourceType);
        return x.GetString(att.ResourceName, culture);
    }


}

