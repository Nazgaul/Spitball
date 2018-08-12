using System;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;

namespace Cloudents.Core.EventHandler
{
    public class EmailAnswerCreated : IConsumer<AnswerCreatedEvent>
    {
        private readonly IServiceBusProvider _serviceBusProvider;
        private readonly IDataProtect _dataProtect;


        public EmailAnswerCreated(IServiceBusProvider serviceBusProvider, IDataProtect dataProtect)
        {
            _serviceBusProvider = serviceBusProvider;
            _dataProtect = dataProtect;
        }


        public void Handle(AnswerCreatedEvent eventMessage)
        {
            var code = _dataProtect.Protect("CreateAnswer", eventMessage.QuestionUserId.ToString(),
                DateTimeOffset.UtcNow.AddDays(2));

            //var t = _serviceBusProvider.InsertMessageAsync(
            //        new GotAnswerEmail(question.Text, question.User.Email, message.Text, message.QuestionLink), token);
            throw new System.NotImplementedException();
        }
    }
}