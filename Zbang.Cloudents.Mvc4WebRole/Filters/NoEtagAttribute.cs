using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zbang.Cloudents.Mvc4WebRole.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class NoEtagAttribute : Attribute
    {
    }


    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class NoUrlLowercaseAttribute : Attribute
    {
    }
}