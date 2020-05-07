using NHibernate.Type;
using System;

namespace Cloudents.Persistence
{
    [Serializable]
    public class GenericEnumStringType<TEnum> : EnumStringType where TEnum : Enum
    {
        public GenericEnumStringType()
            : base(typeof(TEnum))
        {
        }

        public override object? GetInstance(object code)
        {
            if (!(code is string p))
            {
                return null;
            }
            return Enum.Parse(typeof(TEnum), p, true);
        }
    }
}

