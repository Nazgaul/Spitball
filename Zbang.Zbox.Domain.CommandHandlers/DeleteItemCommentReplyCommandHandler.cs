using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class DeleteItemCommentReplyCommandHandler : ICommandHandler<DeleteItemCommentReplyCommand>
    {
        private readonly IRepository<ItemCommentReply> m_ItemCommentReplyRepository;

        public DeleteItemCommentReplyCommandHandler(
            IRepository<ItemCommentReply> itemCommentReplyRepository)
        {
            m_ItemCommentReplyRepository = itemCommentReplyRepository;
        }

        public void Handle(DeleteItemCommentReplyCommand message)
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
            //return m_QueueProvider.InsertMessageToTransactionAsync(new ReputationData(itemReply.Author.Id));
        }
    }
}
