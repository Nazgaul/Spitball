using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class DeleteItemCommentReplyCommandHandler : ICommandHandler<DeleteItemCommentReplyCommand>
    {
        private readonly IUserRepository m_UserRepository;
        private readonly IRepository<ItemCommentReply> m_ItemCommentReplyRepository;
        private readonly IRepository<Item> m_ItemRepository;

        public DeleteItemCommentReplyCommandHandler(IUserRepository userRepository,
            IRepository<ItemCommentReply> itemCommentReplyRepository,
            IRepository<Item> itemRepository)
        {
            m_UserRepository = userRepository;
            m_ItemCommentReplyRepository = itemCommentReplyRepository;
            m_ItemRepository = itemRepository;
        }
        public void Handle(DeleteItemCommentReplyCommand message)
        {
            if (message == null) throw new ArgumentNullException("message");

            //var user = m_UserRepository.Load(message.UserId);

            //var itemComment = m_ItemCommentRepository.Load(message.AnnotationId);
            //if (!Equals(itemComment.Author, user))
            //{
            //    throw new UnauthorizedAccessException("User is unauthorized to delete annotation");
            //}
            //itemComment.Item.DecreaseNumberOfComments();
            //m_ItemRepository.Save(itemComment.Item);

            //m_ItemCommentRepository.Delete(itemComment);
        }
    }
}
