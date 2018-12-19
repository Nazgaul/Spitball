﻿using NHibernate.Type;
using System;

namespace Cloudents.Infrastructure.Database
{
    public class GenericEnumStringType<TEnum> : EnumStringType where TEnum : Enum
    {
        public GenericEnumStringType()
            : base(typeof(TEnum))
        {
        }

        public override object GetInstance(object code)
        {
            var p = code as string;
            if (p == null)
            {
                return null;
            }
            return Enum.Parse(typeof(TEnum), p, true);
        }
    }
}