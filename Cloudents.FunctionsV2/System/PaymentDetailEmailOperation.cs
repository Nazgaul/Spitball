using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.Email;
using Cloudents.Query;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Azure.WebJobs;

namespace Cloudents.FunctionsV2.System
{
    public class PaymentDetailEmailOperation : ISystemOperation<StudentPaymentMessage>
    {
        private readonly IQueryBus _queryBus;
        private readonly IDataProtectionProvider _dataProtectProvider;
        private readonly IUrlBuilder _urlBuilder;

        public PaymentDetailEmailOperation(IQueryBus queryBus, IDataProtectionProvider dataProtectProvider, IUrlBuilder urlBuilder)
        {
            _queryBus = queryBus;
            _dataProtectProvider = dataProtectProvider;
            _urlBuilder = urlBuilder;
        }

        public Task DoOperationAsync(StudentPaymentMessage msg, IBinder binder, CancellationToken token)
        {
            return Task.CompletedTask;
            //var query = new PaymentDetailEmailQuery(msg.StudyRoomId);
            //var usersData = await _queryBus.QueryAsync(query, token);
            //var dataProtector = _dataProtectProvider.CreateProtector("Spitball")
            //    .ToTimeLimitedDataProtector();
            //const string category = "Student-Payment";
            //var emailProvider = await binder.BindAsync<IAsyncCollector<SendGridMessage>>(new SendGridAttribute()
            //{
            //    ApiKey = "SendgridKey",
            //    From = "Spitball <no-reply @spitball.co>"
            //}, token);
            //foreach (var data in usersData)
            //{


            //    var code = dataProtector.Protect(data.Id.ToString(), DateTimeOffset.UtcNow.AddDays(5));
            //    var language = (Language)data.Language;
            //    var message = new SendGridMessage
            //    {
            //        Asm = new ASM { GroupId = 10926 },
            //        TemplateId = language == Language.English ? "d-23276f9feb3a4fd2b16cd0484ef31f76" : "d-f7f9c5ae4e7f4b8986fa81cdd45ceaca "
            //    };
            //    var personalization = new Personalization
            //    {
            //        TemplateData = new
            //        {
            //            tutorName = data.TutorName,
            //            paymentLink = _urlBuilder.BuildPayMeBuyerEndPoint(code), //"www.spitball.co",
            //            to = data.Email
            //        }
            //    };


            //    message.Personalizations = new List<Personalization>()
            //{
            //    personalization
            //};
            //    message.AddCategory(category);
            //    message.TrackingSettings = new TrackingSettings
            //    {
            //        Ganalytics = new Ganalytics
            //        {
            //            UtmCampaign = category,
            //            UtmSource = "SendGrid",
            //            UtmMedium = "Email",
            //            Enable = true
            //        }
            //    };
            //    message.AddTo(data.Email);
            //    await emailProvider.AddAsync(message, token);
            //}
        }
    }
}