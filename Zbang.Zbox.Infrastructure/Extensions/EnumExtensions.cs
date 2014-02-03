using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Zbang.Zbox.Infrastructure.Enums.Resources;
using Zbang.Zbox.Infrastructure.Extensions;


public static class EnumExtension
{
    public static string GetStringValue(this string value)
    {
        MatchCollection seperation = Regex.Matches(value, "[A-Z][a-z]*");
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
        return GetStringValue(value.ToString("G"));
    }




    public static string GetStringValueLowerCase(this Enum value)
    {
        return value.ToString("G").Substring(0, 1).ToLower() + value.ToString("G").Substring(1);
    }


    public static string GetEnumDescription(this Enum value)
    {
        FieldInfo fi = value.GetType().GetField(value.ToString());

        EnumDescription[] attributes = (EnumDescription[])fi.GetCustomAttributes(typeof(EnumDescription), false);


        if ((attributes != null) && (attributes.Length > 0))
        {
            var att = attributes[0];
            if (att.ResorceType != null)
            {
                System.Resources.ResourceManager x = new System.Resources.ResourceManager(att.ResorceType);
                return x.GetString(att.ResourceName);
            }
            return att.Description;
        }
        else
            return value.GetStringValue();
    }


}

