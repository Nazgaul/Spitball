using System;

namespace Zbang.Cloudents.Jared.Filters
{
    [AttributeUsageAttribute(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class PagingAttribute : Attribute
    {
    }
}