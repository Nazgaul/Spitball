using System;

namespace Zbang.Cloudents.Mvc4WebRole.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class NoUrlLowercaseAttribute : Attribute
    {
    }
}