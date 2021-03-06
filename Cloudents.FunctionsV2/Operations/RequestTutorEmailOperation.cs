﻿using System;
using System.Globalization;
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
using shortid.Configuration;

namespace Cloudents.FunctionsV2.Operations
{
    public class RequestTutorEmailOperation : ISystemOperation<RequestTutorMessage>
    {
        private readonly ICommandBus _commandBus;
        private readonly IUrlBuilder _urlBuilder;
        private readonly IDataProtectionService _dataProtectionService;
        private readonly IQueryBus _queryBus;

        public RequestTutorEmailOperation(ICommandBus commandBus, IUrlBuilder urlBuilder, IDataProtectionService dataProtectionService, IQueryBus queryBus)
        {
            _commandBus = commandBus;
            _urlBuilder = urlBuilder;
            _dataProtectionService = dataProtectionService;
            _queryBus = queryBus;
        }

        public async Task DoOperationAsync(RequestTutorMessage message2, IBinder binder, CancellationToken token)
        {
            var query = new RequestTutorEmailQuery(message2.LeadId);
            var result = await _queryBus.QueryAsync(query, token);
            foreach (var obj in result)
            {
                CultureInfo.DefaultThreadCurrentCulture = obj.TutorLanguage.ChangeCultureBaseOnCountry(obj.TutorCountry);
                var body = ResourceWrapper.GetString("unread_message_request_email_body");


                var code = _dataProtectionService.ProtectData(obj.TutorId.ToString(), DateTimeOffset.UtcNow.AddDays(5));
                var identifierChat = ShortId.Generate(new GenerationOptions() {
                    UseNumbers = true,
                    UseSpecialCharacters = false}
                );

                var url = _urlBuilder.BuildChatEndpoint(code, obj.ChatIdentifier, new { utm_source = "request-tutor-email" });
                var commandChat = new CreateShortUrlCommand(identifierChat, url.PathAndQuery, DateTime.UtcNow.AddDays(5));
                await _commandBus.DispatchAsync(commandChat, token);

                var request = string.Empty;
                //TODO -  whatsapp link
                if (!string.IsNullOrEmpty(obj.Request))
                {
                    request = ResourceWrapper.GetString("unread_message_request_email_body_lead_request")
                        .InjectSingleValue("Request", obj.Request);
                }


                var whatsAppLink = new UriBuilder($"https://wa.me/{obj.StudentPhoneNumber.Replace("+", string.Empty)}")
                    .AddQuery(new
                    {
                        text = ResourceWrapper.GetString("unread_message_request_email_body_whatsapp_text").InjectSingleValue("CourseName", obj.CourseName),
                    });

                var identifierWhatsApp = ShortId.Generate(new GenerationOptions() {
                    UseNumbers = true,
                    UseSpecialCharacters = false}
                );
                var commandWhatsApp = new CreateShortUrlCommand(identifierWhatsApp, whatsAppLink.ToString(), DateTime.UtcNow.AddDays(30));
                await _commandBus.DispatchAsync(commandWhatsApp, token);
                var urlShortWhatsApp = _urlBuilder.BuildShortUrlEndpoint(identifierWhatsApp, new
                {
                    eventCategory = "After tutor Submit",
                    eventAction = "Whatsapp email",
                    eventLabel = $"Tutor{obj.TutorId}, Student {obj.StudentId}"
                });
                body = body.InjectSingleValue("Request", request);

                body = body.InjectSingleValue("WhatsappLink", urlShortWhatsApp);

                var htmlBodyDirection = CultureInfo.CurrentCulture.TextInfo.IsRightToLeft ? "rtl" : "ltr";
                body = body.Inject(new
                {
                    Request = request,
                    WhatsappLink = whatsAppLink.ToString(),
                    UserFirstName = obj.TutorFirstName,
                    link = _urlBuilder.BuildShortUrlEndpoint(identifierChat/*, obj.TutorCountry*/),
                    obj.StudentName,
                    obj.CourseName,
                    FirstMessageStudentName = obj.StudentName
                });
                var message = new SendGridMessage()
                {

                    Subject = ResourceWrapper.GetString("unread_message_request_email_subject").InjectSingleValue("FirstMessageStudentName", obj.StudentName)
                        .Inject(obj),
                    HtmlContent = $"<html><body dir=\"{htmlBodyDirection}\">{body.Replace(Environment.NewLine, "<br><br>")}</body></html>",



                };
                message.AddFromResource(CultureInfo.DefaultThreadCurrentCulture);
                message.AddTo(obj.TutorEmail);
                var emailProvider =
                    await binder.BindAsync<IAsyncCollector<SendGridMessage>>(new SendGridAttribute()
                    { ApiKey = "SendgridKey" }, token);
                await emailProvider.AddAsync(message, token);
            }
        }
    }
}
