using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.Email;
using Cloudents.Core.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.EventHandler
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Ioc inject")]
    public class EmailQuestionDeleted : IEventHandler<QuestionDeletedEvent> ,IEventHandler<QuestionRejectEvent>
    {
        private readonly IQueueProvider _serviceBusProvider;
        private readonly Random _random = new Random();


        public EmailQuestionDeleted(IQueueProvider serviceBusProvider)
        {
            _serviceBusProvider = serviceBusProvider;
        }

        public Task HandleAsync(QuestionDeletedEvent eventMessage, CancellationToken token)
        {
            return SendEmailAsync(eventMessage.Question.User.Email, eventMessage.Question.User.Culture, token);
            
        }

        private Task SendEmailAsync(string email, CultureInfo info, CancellationToken token)
        {
            var invisibleTime = _random.Next(10, 20);
            return _serviceBusProvider.InsertMessageAsync(
                new QuestionDeletedEmail(email, info),
                TimeSpan.FromMinutes(invisibleTime), token);
        }

        public Task HandleAsync(QuestionRejectEvent eventMessage, CancellationToken token)
        {
            return SendEmailAsync(eventMessage.User.Email, eventMessage.User.Culture, token);
        }
    }
}

