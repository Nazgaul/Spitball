using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zbang.Cloudents.Jared.Filters
{
    [AttributeUsageAttribute(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class PagingAttribute : Attribute
    {
    }
}