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
        private readonly IQueueProvider _serviceBusProvider;
        private readonly UserManager<User> _userManager;
        private readonly ISmsProvider _smsProvider;

        public SmsSender(UserManager<User> userManager, IQueueProvider serviceBusProvider, ISmsProvider smsProvider)
        {
            _userManager = userManager;
            _serviceBusProvider = serviceBusProvider;
            _smsProvider = smsProvider;
        }

        private async Task SendSmsAsync(string phoneNumber,string code, CancellationToken token)
        {
            //var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber).ConfigureAwait(false);
            var message = new SmsMessage2(phoneNumber, code);

            await _serviceBusProvider.InsertMessageAsync(message, token);
        }

        public Task<string> ValidateNumberAsync(string phoneNumber, CancellationToken token)
        {
            return _smsProvider.ValidateNumberAsync(phoneNumber, token);
        }

        public async Task SendSmsAsync(User user, CancellationToken token)
        {
            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);
            await SendSmsAsync(user.PhoneNumber, code, token);
        }
    }
}
