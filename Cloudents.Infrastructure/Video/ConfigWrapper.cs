using System;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Infrastructure.Video
{
    [SuppressMessage("ReSharper", "CS8618", Justification = "Configuration file")]
    public class ConfigWrapper
    {
        public string SubscriptionId { get; set; }

        public string ResourceGroup { get; set; }


        public string AccountName { get; set; }


        public string AadTenantId { get; set; }

        public string AadClientId { get; set; }


        public string AadSecret { get; set; }


        public Uri ArmAadAudience { get; set; }


        public Uri AadEndpoint { get; set; }


        public Uri ArmEndpoint { get; set; }


        public string Region { get; set; }

    }
}