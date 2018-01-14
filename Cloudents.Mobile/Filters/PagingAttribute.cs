using System;

namespace Cloudents.Mobile.Filters
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    internal sealed class PagingAttribute : Attribute
    {
    }
}