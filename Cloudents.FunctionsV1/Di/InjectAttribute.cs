using System;
using Microsoft.Azure.WebJobs.Description;

namespace Cloudents.FunctionsV1.Di
{

    [Binding]
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class InjectAttribute : Attribute
    {
    }
}
