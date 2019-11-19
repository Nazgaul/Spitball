using Microsoft.Azure.WebJobs.Description;
using System;

namespace Cloudents.Functions.Di
{
    [Binding]
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class InjectAttribute : Attribute
    {
    }
}
