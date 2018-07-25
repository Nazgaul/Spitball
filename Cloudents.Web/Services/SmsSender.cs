using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message;
using Cloudents.Core.Storage;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;

namespace Cloudents.Web.Services
{
    [UsedImplicitly]
    public class SmsSender : ISmsSender
    {
        private readonly IRestClient _client;
        private readonly IServiceBusProvider _serviceBusProvider;
       // private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;

        public SmsSender(IRestClient client, UserManager<User> userManager, IServiceBusProvider serviceBusProvider)
        {
            _client = client;
           // _configuration = configuration;
            _userManager = userManager;
            _serviceBusProvider = serviceBusProvider;
        }

        public async Task SendSmsAsync(User user, CancellationToken token)
        {
            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber).ConfigureAwait(false);
            var message = new SmsMessage2(user.PhoneNumber, code);

            await _serviceBusProvider.InsertMessageAsync(message, token);
            //return await _client.PostJsonAsync(
            //    new Uri(
            //        $"{_configuration["AzureFunction:EndPoint"]}/api/sms?code={_configuration["AzureFunction:Secret"]}"),
            //    message,
            //    null, token).ConfigureAwait(false);
        }

        public async Task<string> ValidateNumberAsync(string phoneNumber, CancellationToken token)
        {
            var uri = new Uri($"https://lookups.twilio.com/v1/PhoneNumbers/{phoneNumber}");

            var byteArray = Encoding.ASCII.GetBytes($"AC1796f09281da07ec03149db53b55db8d:c4cdf14c4f6ca25c345c3600a72e8b49");
            var authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            var headers = new List<KeyValuePair<string, string>>();

            headers.Add(new KeyValuePair<string, string>("Authorization", authorization.ToString()));

            var result = await _client.GetAsync<PhoneValidator>(uri, null, headers, token);

            return result?.phone_number;
        }

        public class PhoneValidator
        {
           // public object caller_name { get; set; }
           // public string country_code { get; set; }
            public string phone_number { get; set; }
           // public string national_format { get; set; }
           // public object carrier { get; set; }
           // public object add_ons { get; set; }
           // public string url { get; set; }
        }
    }

    public interface ISmsSender
    {
        Task SendSmsAsync(User user, CancellationToken token);
        Task<string> ValidateNumberAsync(string phoneNumber, CancellationToken token);
    }
}
