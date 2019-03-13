using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message;
using Cloudents.Core.Storage;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Web.Services
{
    [UsedImplicitly]
    public class SmsSender : ISmsSender
    {
        private readonly IServiceBusProvider _serviceBusProvider;
        private readonly UserManager<RegularUser> _userManager;
        private readonly ISmsProvider _smsProvider;

        public SmsSender(UserManager<RegularUser> userManager, IServiceBusProvider serviceBusProvider, ISmsProvider smsProvider)
        {
            _userManager = userManager;
            _serviceBusProvider = serviceBusProvider;
            _smsProvider = smsProvider;
        }

        private async Task SendSmsAsync(string phoneNumber, string code, SmsMessage.MessageType type, CancellationToken token)
        {

            var message = new SmsMessage(phoneNumber, code, type);
            await _serviceBusProvider.InsertMessageAsync(message, token);
        }

        public Task<(string phoneNumber,string country)> ValidateNumberAsync(string phoneNumber, CancellationToken token)
        {
            return _smsProvider.ValidateNumberAsync(phoneNumber, token);
        }

        public async Task SendSmsAsync(RegularUser user, CancellationToken token)
        {
            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);
            await SendSmsAsync(user.PhoneNumber, code,SmsMessage.MessageType.Sms,  token);
        }

        public async Task SendPhoneAsync(RegularUser user, CancellationToken token)
        {
            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);
            await SendSmsAsync(user.PhoneNumber, code, SmsMessage.MessageType.Phone, token);
        }
    }
}
