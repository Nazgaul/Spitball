using System;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class DeleteItemCommentReplyCommandHandler : ICommandHandlerAsync<DeleteItemCommentReplyCommand>
    {
        private readonly IRepository<ItemCommentReply> m_ItemCommentReplyRepository;
        private readonly IQueueProvider m_QueueProvider;

        public DeleteItemCommentReplyCommandHandler(
            IRepository<ItemCommentReply> itemCommentReplyRepository, IQueueProvider queueProvider)
        {
            m_ItemCommentReplyRepository = itemCommentReplyRepository;
            m_QueueProvider = queueProvider;
        }

        public Task HandleAsync(DeleteItemCommentReplyCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            var itemReply = m_ItemCommentReplyRepository.Load(message.ReplyId);
            if (itemReply.Item.Id != message.ItemId)
            {
                throw new UnauthorizedAccessException("User is unauthorized to delete annotation");
            }
            if (itemReply.Author.Id != message.UserId)
            {
                throw new UnauthorizedAccessException("User is unauthorized to delete annotation");
            }
            
            m_ItemCommentReplyRepository.Delete(itemReply);
            return m_QueueProvider.InsertMessageToTranactionAsync(new ReputationData(itemReply.Author.Id));
        }
    }
}
