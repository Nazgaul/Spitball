using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Config;
using System;
using System.Collections.Concurrent;
using System.Linq;
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

        public TwilioExtensionConfigProvider(INameResolver nameResolver)
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


            var rule = context.AddBindingRule<TwilioCallAttribute>();
            rule.AddValidator(ValidateBinding);

            rule.BindToCollector(CreateContext);
        }

        private void ValidateBinding(TwilioCallAttribute attribute, Type type)
        {
            string accountSid = new[] { attribute.AccountSidSetting, _defaultAccountSid }.FirstOrDefault(f => !string.IsNullOrEmpty(f));
            string authToken = new[] { attribute.AuthTokenSetting, _defaultAuthToken }.FirstOrDefault(f => !string.IsNullOrEmpty(f));
            if (string.IsNullOrEmpty(accountSid))
            {
                throw new ArgumentException();
            }

            if (string.IsNullOrEmpty(authToken))
            {
                throw new ArgumentException();
            }
        }

        private IAsyncCollector<CreateCallOptions> CreateContext(TwilioCallAttribute attribute)
        {
            string accountSid = new[] { attribute.AccountSidSetting, _defaultAccountSid }.FirstOrDefault(f => !string.IsNullOrEmpty(f));
            string authToken = new[] { attribute.AuthTokenSetting, _defaultAuthToken }.FirstOrDefault(f => !string.IsNullOrEmpty(f));


            TwilioRestClient client = _twilioClientCache.GetOrAdd(new Tuple<string, string>(accountSid, authToken), t => new TwilioRestClient(t.Item1, t.Item2));

            var context = new TwilioCallMessageAsyncCollector(client);


            return context;
        }


    }
}