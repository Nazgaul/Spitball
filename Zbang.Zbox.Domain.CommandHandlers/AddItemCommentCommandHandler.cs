using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class AddItemCommentCommandHandler : ICommandHandler<AddItemCommentCommand, AddItemCommentCommandResult>
    {
        private readonly IUserRepository m_UserRepository;
        private readonly IRepository<Item> m_ItemRepository;
        private readonly IBoxRepository m_BoxRepository;
        public AddItemCommentCommandHandler(IUserRepository userRepository,
            IRepository<Item> itemRepository,
            IBoxRepository boxRepository)
        {
            m_UserRepository = userRepository;
            m_ItemRepository = itemRepository;
            m_BoxRepository = boxRepository;
        }


        public AddItemCommentCommandResult Execute(AddItemCommentCommand command)
        {
            var user = m_UserRepository.Get(command.UserId);
            var item = m_ItemRepository.Get(command.ItemId);

            var userType = m_UserRepository.GetUserToBoxRelationShipType(user.Id, item.Box.Id);// user.GetUserType(item.Box.Id);
            if (userType == UserRelationshipType.None || userType == UserRelationshipType.Invite)
            {
                throw new UnauthorizedAccessException("User is not connected to box");
            }

            //Decode the comment to html friendly
            var encodeUserComment = TextManipulation.EncodeComment(command.CommentText);

            var comment = item.AddItemComment(user, encodeUserComment);
            item.DateTimeUser.UpdateUserTime(user.Email);
            
            item.Box.IncreaseCommentCounter();

            m_ItemRepository.Save(item);
            m_BoxRepository.Save(item.Box);
            return new AddItemCommentCommandResult(comment, item.Box, item, user);
        }
    }
}
