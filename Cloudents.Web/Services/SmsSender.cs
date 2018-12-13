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
        private readonly IServiceBusProvider _serviceBusProvider;
        private readonly UserManager<RegularUser> _userManager;
        private readonly ISmsProvider _smsProvider;

        public SmsSender(UserManager<RegularUser> userManager, IServiceBusProvider serviceBusProvider, ISmsProvider smsProvider)
        {
            _userManager = userManager;
            _serviceBusProvider = serviceBusProvider;
            _smsProvider = smsProvider;
        }

        private async Task SendSmsAsync(string phoneNumber,string code, CancellationToken token)
        {
            var message = new SmsMessage2(phoneNumber, code);
            await _serviceBusProvider.InsertMessageAsync(message, token);
        }

        public Task<string> ValidateNumberAsync(string phoneNumber, CancellationToken token)
        {
            return _smsProvider.ValidateNumberAsync(phoneNumber, token);
        }

        public async Task SendSmsAsync(RegularUser user, CancellationToken token)
        {
            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);
            await SendSmsAsync(user.PhoneNumber, code, token);
        }
    }
}
