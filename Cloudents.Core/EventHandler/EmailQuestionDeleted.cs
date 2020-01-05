using Cloudents.Core.Entities;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.Email;
using Cloudents.Core.Storage;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.EventHandler
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Ioc inject")]
    public class EmailQuestionDeleted : EmailEventHandler,
        IEventHandler<QuestionDeletedAdminEvent>
    {

        private readonly Random _random = new Random();


        public EmailQuestionDeleted(IQueueProvider serviceBusProvider) : base(serviceBusProvider)
        {
        }

        public Task HandleAsync(QuestionDeletedAdminEvent eventMessage, CancellationToken token)
        {
            //if (eventMessage.Question.User is User u)
            //{
            //    return SendEmailAsync(u, token);
            //}

            return Task.CompletedTask;
        }

        //private Task SendEmailAsync(User user, CancellationToken token)
        //{
        //    var invisibleTime = _random.Next(10, 20);
        //    return SendEmail(
        //        new QuestionDeletedEmail(user.Email, user.Language),
        //        TimeSpan.FromMinutes(invisibleTime), token);
        //}

        //public Task HandleAsync(QuestionRejectEvent eventMessage, CancellationToken token)
        //{
        //    return SendEmailAsync(eventMessage.User, token);
        //}
    }
}

