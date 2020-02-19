using Microsoft.Azure.WebJobs.Description;
using System;

namespace Cloudents.FunctionsV2.Binders
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    [Binding]
    public sealed class TwilioCallAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets an optional string value indicating the app setting to use as the Twilio Account SID, 
        /// if different than the one specified in the <see cref="Cloudents.FunctionsV2.Binders"/>.
        /// </summary>
        [AppSetting]
        public string AccountSidSetting { get; set; }

        /// <summary>
        /// Gets or sets an optional string value indicating the app setting to use as the Twilio Auth Token, 
        /// if different than the one specified in the <see cref="Cloudents.FunctionsV2.Binders"/>.
        /// </summary>
        [AppSetting]
        public string AuthTokenSetting { get; set; }

        /// <summary>
        /// Gets or sets the message "From" field. May include binding parameters.
        /// </summary>
        [AutoResolve]
        public string From { get; set; }

    }
}