using Cloudents.Core.Entities.Db;
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
    public class EmailQuestionDeleted : EmailEventHandler, IEventHandler<QuestionDeletedEvent> ,IEventHandler<QuestionRejectEvent>
    {
        
        private readonly Random _random = new Random();


        public EmailQuestionDeleted(IQueueProvider serviceBusProvider): base(serviceBusProvider)
        {
        }

        public Task HandleAsync(QuestionDeletedEvent eventMessage, CancellationToken token)
        {
            return SendEmailAsync(eventMessage.Question.User, token);
            
        }

        private Task SendEmailAsync(User user, CancellationToken token)
        {
            var invisibleTime = _random.Next(10, 20);
            return SendEmail(
                new QuestionDeletedEmail(user.Email, user.Culture),
                TimeSpan.FromMinutes(invisibleTime), user, token);
        }

        public Task HandleAsync(QuestionRejectEvent eventMessage, CancellationToken token)
        {
            return SendEmailAsync(eventMessage.User, token);
        }
    }
}

