using System;
using System.Linq;
using Cloudents.Core.Enum;

namespace Cloudents.Core.Extension
{
    public static class EnumExtension
    {
        public static string GetDescription(this System.Enum value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            var fi = value.GetType().GetField(value.ToString());

            switch (fi.GetCustomAttributes(typeof(ParseAttribute), false).FirstOrDefault())
            {
                case null:
                    return value.ToString();
                case ParseAttribute parse:
                    return parse.Description;
            }

            throw new InvalidCastException();
        }
    }
}