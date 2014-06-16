using System;

using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class AddAnnotationCommandHandler : ICommandHandler<AddAnnotationCommand>
    {
        private readonly IUserRepository m_UserRepository;
        private readonly IRepository<Item> m_ItemRepository;
        private readonly IRepository<ItemComment> m_ItemCommentRepository;

        public AddAnnotationCommandHandler(IUserRepository userRepository, IRepository<Item> itemRepository,
            IRepository<ItemComment> itemCommentRepository
           )
        {
            m_UserRepository = userRepository;
            m_ItemRepository = itemRepository;
            m_ItemCommentRepository = itemCommentRepository;
        }
        public void Handle(AddAnnotationCommand message)
        {
            Throw.OnNull(message, "message");
            Throw.OnNegative(message.ImageId, "imageId");
            Throw.OnNegative(message.X, "cordX");
            Throw.OnNegative(message.Y, "cordY");
            Throw.OnNegative(message.Width, "width");
            Throw.OnNegative(message.Height, "height");
            Throw.OnNull(message.Comment, "comment", false);


            var user = m_UserRepository.Load(message.UserId);
            Throw.OnNull(user, "user");
            var item = m_ItemRepository.Load(message.ItemId);
            Throw.OnNull(item, "item");

            var userType = m_UserRepository.GetUserToBoxRelationShipType(message.UserId, item.Box.Id); //user.GetUserType(box.Id);
            if (userType == UserRelationshipType.None || userType == UserRelationshipType.Invite)
            {
                throw new UnauthorizedAccessException("User is not connected to box");
            }
            var text = TextManipulation.EncodeText(message.Comment);
            var comment = new ItemComment(user, item, message.ImageId, text, message.X, message.Y, message.Width, message.Height);
            item.IncreaseNumberOfComments();
            m_ItemCommentRepository.Save(comment);
            m_ItemRepository.Save(item);
            message.AnnotationId = comment.Id;
        }
    }
}
