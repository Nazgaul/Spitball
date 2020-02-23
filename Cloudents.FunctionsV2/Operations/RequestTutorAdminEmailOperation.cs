using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.Email;
using Cloudents.FunctionsV2.Services;
using Cloudents.Query;
using Cloudents.Query.Email;
using Microsoft.Azure.WebJobs;
using SendGrid.Helpers.Mail;
using shortid;

namespace Cloudents.FunctionsV2.Operations
{
    
    public class RequestTutorAdminEmailOperation : ISystemOperation<RequestTutorMessage>
    {
        private readonly ICommandBus _commandBus;
        private readonly IUrlBuilder _urlBuilder;
        private readonly IDataProtectionService _dataProtectionService;
        private readonly IQueryBus _queryBus;
        private readonly IConfigurationKeys _configuration;
        private string _email = "yaniv@spitball.co";

        public RequestTutorAdminEmailOperation(ICommandBus commandBus, IUrlBuilder urlBuilder, IDataProtectionService dataProtectionService, IQueryBus queryBus,
            IConfigurationKeys configuration)
        {
            _commandBus = commandBus;
            _urlBuilder = urlBuilder;
            _dataProtectionService = dataProtectionService;
            _queryBus = queryBus;
            _configuration = configuration;
        }

        
        public async Task DoOperationAsync(RequestTutorMessage message2, IBinder binder, CancellationToken token)
        {
            if (_configuration.Search.IsDevelop)
            //if (bool.Parse(_configuration["IsDevelop"]))
            {
                _email = "elad@cloudents.com";
            }
            var query = new RequestTutorAdminEmailQuery(message2.LeadId);
            var result = await _queryBus.QueryAsync(query, token);

            if (result == null)
            {
                return;
            }

            foreach (var obj in result)
            {
                var code = _dataProtectionService.ProtectData(obj.UserId.ToString(), DateTimeOffset.UtcNow.AddDays(5));
                var identifierChat = ShortId.Generate(true, false);

                var url = _urlBuilder.BuildChatEndpoint(code, new { utm_source = "request-tutor-admin-email" });
                var commandChat = new CreateShortUrlCommand(identifierChat, url.PathAndQuery, DateTime.UtcNow.AddDays(5));
                await _commandBus.DispatchAsync(commandChat, token);

                var whatsAppLink = new UriBuilder($"https://wa.me/{obj.UserPhone.Replace("+", string.Empty)}")
                            .AddQuery(new
                            {
                                text = $"שובץ לך מורה ל{obj.CourseName} בשם {obj.TutorName}. לשוחח עם המורה לחץ {_urlBuilder.BuildShortUrlEndpoint(identifierChat)}"
                            });

                string body = whatsAppLink.ToString();

                var message = new SendGridMessage()
                {
                    Subject = "New Tutor request",
                    HtmlContent = $"<html><body>{body.Replace(Environment.NewLine, "<br><br>")}</body></html>"
                };

                message.AddTo(_email);
                var emailProvider =
                    await binder.BindAsync<IAsyncCollector<SendGridMessage>>(new SendGridAttribute()
                    { ApiKey = "SendgridKey", From = "Spitball <no-reply@spitball.co>" }, token);
                await emailProvider.AddAsync(message, token);
                await emailProvider.FlushAsync(token);
            }
        }
    }
}
