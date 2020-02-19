using System;
using Cloudents.Core.Message.Email;
using Cloudents.Query;
using Cloudents.Query.Email;
using Microsoft.Azure.WebJobs;
using SendGrid.Helpers.Mail;
using System.Globalization;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.FunctionsV2.System
{
    public class RedeemTransactionMessageEmailOperation : ISystemOperation<RedeemTransactionMessage>
    {
        private readonly IQueryBus _queryBus;

        public RedeemTransactionMessageEmailOperation(IQueryBus queryBus)
        {
            _queryBus = queryBus;
        }

        public async Task DoOperationAsync(RedeemTransactionMessage msg, IBinder binder, CancellationToken token)
        {
            var query = new RedeemEmailQuery(msg.TransactionId);
            var result = await _queryBus.QueryAsync(query, token);
            if (result is null)
            {
                return;
            }
            var message = new SendGridMessage();


            message.AddContent("text/html", $"User id: {result.UserId} want to redeem {result.Amount}");

            var culture = new CultureInfo("en");
            if (result.Country.Equals("IN", StringComparison.OrdinalIgnoreCase))
            {
                culture = new CultureInfo("en-IN");

            }
            CultureInfo.CurrentCulture = CultureInfo.DefaultThreadCurrentCulture = culture;
            var emailTo = ResourceWrapper.GetString("email_support");

            message.AddTo(emailTo);
            var emailProvider = await binder.BindAsync<IAsyncCollector<SendGridMessage>>(new SendGridAttribute()
            {
                ApiKey = "SendgridKey",
                From = "Spitball <no-reply@spitball.co>",
                Subject = $"Redeem Email {result.UserId}"

            }, token);
            await emailProvider.AddAsync(message, token);
        }
    }

    public static class SendGridMessageExtensions
    {
        public static void AddFromResource(this SendGridMessage message, CultureInfo culture)
        {

            CultureInfo.DefaultThreadCurrentCulture = culture;
            var mailAddress = new MailAddress(ResourceWrapper.GetString("email_from"));
            message.From = new EmailAddress(mailAddress.Address, mailAddress.DisplayName);
        }
    }
}