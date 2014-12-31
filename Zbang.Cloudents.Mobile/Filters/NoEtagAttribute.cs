using System;

namespace Zbang.Cloudents.Mobile.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class NoEtagAttribute : Attribute
    {
    }
}