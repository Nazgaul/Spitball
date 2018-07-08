using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message;
using Cloudents.Core.Storage;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Cloudents.Web.Services
{
    [UsedImplicitly]
    public class SmsSender : ISmsSender
    {
        private readonly IRestClient _client;
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;

        public SmsSender(IRestClient client, IConfiguration configuration, UserManager<User> userManager)
        {
            _client = client;
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task<bool> SendSmsAsync(User user, CancellationToken token)
        {
            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber).ConfigureAwait(false);
            var message = new SmsMessage(user.PhoneNumber, code);
            return await _client.PostJsonAsync(
                new Uri(
                    $"{_configuration["AzureFunction:EndPoint"]}/api/sms?code={_configuration["AzureFunction:Secret"]}"),
                message,
                null, token).ConfigureAwait(false);
        }
    }

    public interface ISmsSender
    {
        Task<bool> SendSmsAsync(User user, CancellationToken token);
    }
}
