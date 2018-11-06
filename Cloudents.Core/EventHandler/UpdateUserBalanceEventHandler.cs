using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command.Admin;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.System;
using Cloudents.Core.Storage;

namespace Cloudents.Core.EventHandler
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class UpdateUserBalanceEventHandler : IEventHandler<QuestionDeletedAdminEvent>, IEventHandler<AnswerDeletedAdminEvent>
    {
        private readonly IQueueProvider _queueProvider;

        public UpdateUserBalanceEventHandler( IQueueProvider queueProvider)
        {
            _queueProvider = queueProvider;
        }

        public Task HandleAsync(QuestionDeletedAdminEvent eventMessage, CancellationToken token)
        {
            //var command = new UpdateUserBalanceCommand(eventMessage.UserIds);
            return _queueProvider.InsertMessageAsync(new UpdateUserBalanceMessage(eventMessage.UserIds), token);
        }

        public Task HandleAsync(AnswerDeletedAdminEvent answerEventMessage, CancellationToken token)
        {
            return _queueProvider.InsertMessageAsync(new UpdateUserBalanceMessage(answerEventMessage.UserIds), token);
        }
    }
}
