using System;

namespace Cloudents.MobileApi.Filters
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    internal sealed class PagingAttribute : Attribute
    {
    }
}