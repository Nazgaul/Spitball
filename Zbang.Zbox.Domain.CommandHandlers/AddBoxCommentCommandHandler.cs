using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class AddBoxCommentCommandHandler : ICommandHandler<AddBoxCommentCommand, AddBoxCommentCommandResult>
    {
        private readonly IUserRepository m_UserRepository;
        private readonly IRepository<Box> m_BoxRepository;

        public AddBoxCommentCommandHandler(
            IUserRepository userRepository, IRepository<Box> boxRepository)
        {
            m_BoxRepository = boxRepository;
            m_UserRepository = userRepository;
        }

        public AddBoxCommentCommandResult Execute(AddBoxCommentCommand command)
        {
            var user = m_UserRepository.Get(command.UserId);
            var box = m_BoxRepository.Get(command.BoxId);

            var userType = m_UserRepository.GetUserToBoxRelationShipType(command.UserId, box.Id); //user.GetUserType(box.Id);
            if (userType == UserRelationshipType.None || userType == UserRelationshipType.Invite)
            {
                throw new UnauthorizedAccessException("User is not connected to box");
            }
            //Decode the comment to html friendly
            var encodeUserComment = TextManipulation.EncodeComment(command.CommentText);

            var comment = box.AddBoxComment(user, encodeUserComment);

            box.UserTime.UpdateUserTime(user.Name);
            m_BoxRepository.Save(box);
            return new AddBoxCommentCommandResult(comment, box, user);
        }
    }
}
