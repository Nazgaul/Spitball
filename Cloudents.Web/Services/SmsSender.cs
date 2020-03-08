using Cloudents.Core.Entities;
using Cloudents.Core.Message;
using Cloudents.Core.Storage;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Web.Services
{
    public class SmsSender : ISmsSender
    {
        private readonly IServiceBusProvider _serviceBusProvider;
        private readonly UserManager<User> _userManager;

        public SmsSender(UserManager<User> userManager, IServiceBusProvider serviceBusProvider)
        {
            _userManager = userManager;
            _serviceBusProvider = serviceBusProvider;
        }

        private async Task SendSmsAsync(string phoneNumber, string code, SmsMessage.MessageType type, CancellationToken token)
        {
            var message = new SmsMessage(phoneNumber,  code, type);
            await _serviceBusProvider.InsertMessageAsync(message, token);
        }

        //public Task<(string phoneNumber,string country)> ValidateNumberAsync(string phoneNumber,string countryCode, CancellationToken token)
        //{
        //    return _smsProvider.ValidateNumberAsync(phoneNumber, countryCode, token);
        //}

        public async Task SendSmsAsync(User user, CancellationToken token)
        {
            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);
            await SendSmsAsync(user.PhoneNumber, code, SmsMessage.MessageType.Sms, token);
        }

        public async Task SendPhoneAsync(User user, CancellationToken token)
        {
            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);
            await SendSmsAsync(user.PhoneNumber, code, SmsMessage.MessageType.Phone, token);
        }
    }
}
