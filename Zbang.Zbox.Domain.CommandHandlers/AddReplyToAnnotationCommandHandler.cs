
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class AddReplyToAnnotationCommandHandler : ICommandHandler<AddReplyToAnnotationCommand>
    {
        private readonly IUserRepository m_UserRepository;
        private readonly IRepository<Item> m_ItemRepository;
        private readonly IRepository<ItemComment> m_ItemCommentRepository;
        private readonly IRepository<ItemCommentReply> m_ItemCommentReplyRepository;

        public AddReplyToAnnotationCommandHandler(IUserRepository userRepository,
            IRepository<Item> itemRepository,
            IRepository<ItemComment> itemCommentRepository,
            IRepository<ItemCommentReply> itemCommentReplyRepository)
        {
            m_UserRepository = userRepository;
            m_ItemRepository = itemRepository;
            m_ItemCommentRepository = itemCommentRepository;
            m_ItemCommentReplyRepository = itemCommentReplyRepository;
        }
        public void Handle(AddReplyToAnnotationCommand message)
        {
            var user = m_UserRepository.Load(message.UserId);
            var item = m_ItemRepository.Load(message.ItemId);

            var itemComment = m_ItemCommentRepository.Load(message.ItemCommentId);
            var comment = new ItemCommentReply(user, item, message.ImageId, message.Comment, itemComment);
            m_ItemCommentReplyRepository.Save(comment);

            message.ReplyId = comment.Id;
            
        }
    }
}
