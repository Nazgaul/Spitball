using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class DeleteItemCommentReplyCommandHandler : ICommandHandler<DeleteItemCommentReplyCommand>
    {
        private readonly IRepository<ItemCommentReply> m_ItemCommentReplyRepository;
        private readonly IQueueProvider m_QueueProvider;

        public DeleteItemCommentReplyCommandHandler(
            IRepository<ItemCommentReply> itemCommentReplyRepository, IQueueProvider queueProvider)
        {
            m_ItemCommentReplyRepository = itemCommentReplyRepository;
            m_QueueProvider = queueProvider;
        }

        public void Handle(DeleteItemCommentReplyCommand message)
        {
            if (message == null) throw new ArgumentNullException("message");

            //var user = m_UserRepository.Load(message.UserId);

            var itemReply = m_ItemCommentReplyRepository.Load(message.ReplyId);
            if (itemReply.Author.Id != message.UserId)
            {
                throw new UnauthorizedAccessException("User is unauthorized to delete annotation");
            }
            //itemComment.Item.DecreaseNumberOfComments();
            m_QueueProvider.InsertMessageToTranaction(new ReputationData(itemReply.Author.Id));
            m_ItemCommentReplyRepository.Delete(itemReply);
        }
    }
}
