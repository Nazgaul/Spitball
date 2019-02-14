﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Config;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;

namespace Cloudents.FunctionsV2.Binders
{
    internal class TwilioExtensionConfigProvider : IExtensionConfigProvider
    {
        internal const string AzureWebJobsTwilioAccountSidKeyName = "AzureWebJobsTwilioAccountSid";
        internal const string AzureWebJobsTwilioAccountAuthTokenName = "AzureWebJobsTwilioAuthToken";

        //private readonly IOptions<TwilioSmsOptions> _options;
        private readonly INameResolver _nameResolver;
        private readonly ConcurrentDictionary<Tuple<string, string>, TwilioRestClient> _twilioClientCache = new ConcurrentDictionary<Tuple<string, string>, TwilioRestClient>();

        private string _defaultAccountSid;
        private string _defaultAuthToken;

        public TwilioExtensionConfigProvider( INameResolver nameResolver)
        {
            _nameResolver = nameResolver;
        }

        public void Initialize(ExtensionConfigContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            _defaultAccountSid = _nameResolver.Resolve(AzureWebJobsTwilioAccountSidKeyName);
            _defaultAuthToken = _nameResolver.Resolve(AzureWebJobsTwilioAccountAuthTokenName);

            //context.AddConverter<JObject, CreateMessageOptions>(CreateMessageOptions);

            var rule = context.AddBindingRule<TwilioCallAttribute>();
            rule.AddValidator(ValidateBinding);

            //context
            //    .AddBindingRule<AzureSearchSyncAttribute>()
            //    .BindToCollector(CreateCollector);


            rule.BindToCollector(CreateContext);
        }

        private void ValidateBinding(TwilioCallAttribute attribute, Type type)
        {
            string accountSid = new [] { attribute.AccountSidSetting, _defaultAccountSid}.FirstOrDefault(f=>!string.IsNullOrEmpty(f));
            string authToken = new[] { attribute.AuthTokenSetting, _defaultAuthToken }.FirstOrDefault(f => !string.IsNullOrEmpty(f)); 
            if (string.IsNullOrEmpty(accountSid))
            {
                throw new ArgumentException();
                // ThrowMissingSettingException("AccountSID", AzureWebJobsTwilioAccountSidKeyName, "AccountSID");
            }

            if (string.IsNullOrEmpty(authToken))
            {
                throw new ArgumentException();
                // ThrowMissingSettingException("AuthToken", AzureWebJobsTwilioAccountAuthTokenName, "AuthToken");
            }
        }

        private IAsyncCollector<CreateCallOptions> CreateContext(TwilioCallAttribute attribute)
        {
            string accountSid = new[] { attribute.AccountSidSetting, _defaultAccountSid }.FirstOrDefault(f => !string.IsNullOrEmpty(f));
            string authToken = new[] { attribute.AuthTokenSetting, _defaultAuthToken }.FirstOrDefault(f => !string.IsNullOrEmpty(f));


            TwilioRestClient client = _twilioClientCache.GetOrAdd(new Tuple<string, string>(accountSid, authToken), t => new TwilioRestClient(t.Item1, t.Item2));

            var context = new TwilioCallMessageAsyncCollector(client);
            //{
            //    Client = client,
            //    Body = Utility.FirstOrDefault(attribute.Body, _options.Value.Body),
            //    From = Utility.FirstOrDefault(attribute.From, _options.Value.From),
            //    To = Utility.FirstOrDefault(attribute.To, _options.Value.To)
            //};

            return context;
        }

        //internal static CreateMessageOptions CreateMessageOptions(JObject messageOptions)
        //{
        //    var options = new CreateMessageOptions(new PhoneNumber(GetValueOrDefault<string>(messageOptions, "to")))
        //    {
        //        ProviderSid = GetValueOrDefault<string>(messageOptions, "providerSid"),
        //        Body = GetValueOrDefault<string>(messageOptions, "body"),
        //        ForceDelivery = GetValueOrDefault<bool?>(messageOptions, "forceDelivery"),
        //        MaxRate = GetValueOrDefault<string>(messageOptions, "maxRate"),
        //        ValidityPeriod = GetValueOrDefault<int?>(messageOptions, "validityPeriod"),
        //        ProvideFeedback = GetValueOrDefault<bool?>(messageOptions, "provideFeedback"),
        //        MaxPrice = GetValueOrDefault<decimal?>(messageOptions, "maxPrice"),
        //        ApplicationSid = GetValueOrDefault<string>(messageOptions, "applicationSid"),
        //        MessagingServiceSid = GetValueOrDefault<string>(messageOptions, "messagingServiceSid"),
        //        PathAccountSid = GetValueOrDefault<string>(messageOptions, "pathAccountSid")
        //    };

        //    string value = GetValueOrDefault<string>(messageOptions, "from");
        //    if (!string.IsNullOrEmpty(value))
        //    {
        //        options.From = new PhoneNumber(value);
        //    }

        //    value = GetValueOrDefault<string>(messageOptions, "statusCallback");
        //    if (!string.IsNullOrEmpty(value))
        //    {
        //        options.StatusCallback = new Uri(value);
        //    }

        //    JArray mediaUrls = GetValueOrDefault<JArray>(messageOptions, "mediaUrl");
        //    if (mediaUrls != null)
        //    {
        //        List<Uri> uris = new List<Uri>();
        //        foreach (var url in mediaUrls)
        //        {
        //            uris.Add(new Uri((string)url));
        //        }
        //        options.MediaUrl = uris;
        //    }

        //    return options;
        //}

        //private static TValue GetValueOrDefault<TValue>(JObject messageObject, string propertyName)
        //{
        //    JToken result = null;
        //    if (messageObject.TryGetValue(propertyName, StringComparison.OrdinalIgnoreCase, out result))
        //    {
        //        return result.Value<TValue>();
        //    }

        //    return default(TValue);
        //}

        //private static string ThrowMissingSettingException(string settingDisplayName, string settingName, string configPropertyName)
        //{
        //    string message = string.Format("The Twilio {0} must be set either via a '{1}' app setting, via a '{1}' environment variable, or directly in code via TwilioSmsConfiguration.{2}.",
        //        settingDisplayName, settingName, configPropertyName);

        //    throw new InvalidOperationException(message);
        //}
    }
}