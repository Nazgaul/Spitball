using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;

namespace Cloudents.Web.Services
{
    [UsedImplicitly]
    public class SmsSender : ISmsSender
    {
        private readonly IRestClient _client;
        private readonly IConfigurationKeys _configuration;
        private readonly UserManager<User> _userManager;

        public SmsSender(IRestClient client, IConfigurationKeys configuration, UserManager<User> userManager)
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
                    $"{_configuration.FunctionEndpoint}/api/sms?code=n5lOpJBVA2aaCaOkyT3e1TcFfyqjrX8bbk3yfvoHLACBl/aB2TnWPg=="),
                message,
                null, token).ConfigureAwait(false);
        }
    }

    public interface ISmsSender
    {
        Task<bool> SendSmsAsync(User user, CancellationToken token);
    }
}
