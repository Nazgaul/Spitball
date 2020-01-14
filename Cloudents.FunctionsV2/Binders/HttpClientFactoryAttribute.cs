using System;
using Microsoft.Azure.WebJobs.Description;

namespace Cloudents.FunctionsV2.Binders
{
    [Binding]
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    public sealed class HttpClientFactoryAttribute : Attribute
    { }
}