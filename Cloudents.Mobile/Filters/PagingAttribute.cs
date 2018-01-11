using System;

namespace Cloudents.Mobile.Filters
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class PagingAttribute : Attribute
    {
    }
}