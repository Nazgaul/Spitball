using System;
using System.Reflection;
using System.Text.RegularExpressions;
using Zbang.Zbox.Infrastructure.Extensions;


// ReSharper disable once CheckNamespace - this is an extension
public static class EnumExtension
{
    public static string GetStringValue(this string value)
    {
        var seperation = Regex.Matches(value, "[A-Z][a-z]*");
        if (seperation.Count == 0)
        {
            return value;
        }

        if (seperation.Count == 1)
        {
            return seperation[0].ToString();
        }

        string seperateWord = string.Empty;

        foreach (var word in seperation)
        {
            seperateWord += word + " ";
        }

        return seperateWord;
    }
    public static string GetStringValue(this Enum value)
    {
        if (value == null) throw new ArgumentNullException("value");
        return GetStringValue(value.ToString("G"));
    }


    public static string GetStringValueLowerCase(this Enum value)
    {
        if (value == null) throw new ArgumentNullException("value");
        return value.ToString("G").Substring(0, 1).ToLower() + value.ToString("G").Substring(1);
    }


    public static string GetEnumDescription(this Enum value)
    {
        if (value == null) throw new ArgumentNullException("value");
        FieldInfo fi = value.GetType().GetField(value.ToString());

        var attributes = (EnumDescription[])fi.GetCustomAttributes(typeof(EnumDescription), false);


        if ((attributes.Length <= 0)) return value.GetStringValue();
        var att = attributes[0];
        if (att.ResorceType != null)
        {
            var x = new System.Resources.ResourceManager(att.ResorceType);
            return x.GetString(att.ResourceName);
        }
        return att.Description;
    }


}

