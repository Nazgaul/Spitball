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

            if (!(fi.GetCustomAttributes(typeof(ParseAttribute), false).First() is ParseAttribute att))
            {
                throw new NullReferenceException(nameof(att));
            }
            return att.Description;
        }
    }
}