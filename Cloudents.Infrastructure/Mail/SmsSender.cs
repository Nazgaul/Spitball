using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Cloudents.Infrastructure.Mail
{
    public class SmsSender
    {
        public Task SendSmsAsync(string number, string message)
        {
            TwilioClient.Init("AC1796f09281da07ec03149db53b55db8d", "c4cdf14c4f6ca25c345c3600a72e8b49");

            return MessageResource.CreateAsync(to: new PhoneNumber(number),
                from:new PhoneNumber("(203) 347-4577"),
                body: message);
        }
    }
}