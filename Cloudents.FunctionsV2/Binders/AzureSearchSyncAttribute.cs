using Microsoft.Azure.WebJobs.Description;
using System;

namespace Cloudents.FunctionsV2.Binders
{
    [AttributeUsage(AttributeTargets.ReturnValue | AttributeTargets.Parameter)]
    [Binding]
    public class AzureSearchSyncAttribute : Attribute
    {
        public AzureSearchSyncAttribute(string indexName)
        {
            IndexName = indexName;
        }

        [AppSetting(Default = "SearchServiceAdminApiKey")]
        public string Key { get; set; }

        [AppSetting(Default = "SearchServiceName")]
        public string Name { get; set; }

        //[AppSetting(Default = "IsDevelop")]
        public bool IsDevelopIndex { get; set; }

        [AppSetting(Default = "IsDevelop")]
        public string IsDevelop { get; set; }

        [AutoResolve]
        public string IndexName { get; set; }
    }
}