using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Repositories;
using System;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class AddReplyToCommentCommandHandler : ICommandHandler<AddReplyToCommentCommand, AddReplyToCommentCommandResult>
    {
        //Fields        

        private readonly IUserRepository m_UserRepository;
        private readonly IRepository<Comment> m_CommentReplyRepository;
        //private IEventBus m_EventBus;

        //Ctors
        public AddReplyToCommentCommandHandler(IUserRepository userRepository, IRepository<Comment> targetRepository)
        {
            m_UserRepository = userRepository;
            m_CommentReplyRepository = targetRepository;
        }

        //Methods
        public AddReplyToCommentCommandResult Execute(AddReplyToCommentCommand command)
        {
            // Box box = m_BoxRepository.Get(command.BoxId);

            var target = m_CommentReplyRepository.Get(command.CommentToReplyToId);
            var user = m_UserRepository.Get(command.UserId);
            var userType = m_UserRepository.GetUserToBoxRelationShipType(user.Id, command.BoxId); //user.GetUserType(command.BoxId);
            if (userType == UserRelationshipType.None || userType == UserRelationshipType.Invite)
            {
                throw new UnauthorizedAccessException("User is not connected to box");
            }

            //UserPermissionSettings permission = user.GetUserPermission(command.BoxId);

            var encodeUserComment = TextManipulation.EncodeComment(command.CommentText);
            var comment = target.AddComment(user, encodeUserComment);
            
            m_CommentReplyRepository.Save(target);
            return new AddReplyToCommentCommandResult(comment, user, command.BoxId);
        }
    }
}
