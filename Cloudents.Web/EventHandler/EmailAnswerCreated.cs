﻿using Cloudents.Web.Services;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.Event;
using Cloudents.Application.EventHandler;
using Cloudents.Application.Interfaces;
using Cloudents.Application.Message.Email;
using Cloudents.Application.Storage;
using Cloudents.Domain.Entities;

namespace Cloudents.Web.EventHandler
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Ioc inject")]
    public class EmailAnswerCreated : EmailEventHandler, IEventHandler<AnswerCreatedEvent>
    {
        public const string CreateAnswer = "CreateAnswer";
       
        private readonly IDataProtect _dataProtect;
        private readonly IUrlBuilder _urlBuilder;


        public EmailAnswerCreated(IQueueProvider serviceBusProvider,
             IDataProtect dataProtect,
             IUrlBuilder urlBuilder) 
                : base(serviceBusProvider)
        {
            _dataProtect = dataProtect;
            _urlBuilder = urlBuilder;
        }


        public async Task HandleAsync(AnswerCreatedEvent eventMessage, CancellationToken token)
        {
            if (_dataProtect == null)
            {
                //This can happen if we not in the scope of the website
                return;
            }

            if (!(eventMessage.Answer.Question.User is RegularUser u))
            {
                return;
            }
            var question = eventMessage.Answer.Question;
            var code = _dataProtect.Protect(CreateAnswer, question.User.Id.ToString(),
                DateTimeOffset.UtcNow.AddDays(2));
            var link = _urlBuilder.BuildQuestionEndPoint(question.Id, new { code });
            await SendEmail(
                   new GotAnswerEmail(question.Text, question.User.Email, eventMessage.Answer.Text, link, question.User.Culture)
                   , u
                   , token);
        }
    }
}