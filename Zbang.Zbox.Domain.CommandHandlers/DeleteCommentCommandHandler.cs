using System;

using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class DeleteCommentCommandHandler : ICommandHandler<DeleteCommentCommand>
    {
        //Fields             
        private IRepository<CommentReplies> m_CommentRepository;
        private IUserRepository m_UserRepository;
        private IRepository<Box> m_BoxRepository;

        //Ctors
        public DeleteCommentCommandHandler(IRepository<CommentReplies> commentRepository, IUserRepository userRepository , IRepository<Box> boxRepository)
        {
            m_CommentRepository = commentRepository;
            m_UserRepository = userRepository;
            m_BoxRepository = boxRepository;
        }

        //Methods
        public void Handle(DeleteCommentCommand command)
        {
            CommentReplies commentReply = m_CommentRepository.Get(command.CommentId);
            Box box = m_BoxRepository.Get(command.BoxId);
            User user = m_UserRepository.Get(command.UserId);

            var userType = m_UserRepository.GetUserToBoxRelationShipType(user.Id, box.Id); //user.GetUserType(box.Id);
           
            bool isAuthorized = (userType == UserRelationshipType.Owner) || (user == commentReply.Author);

            if (commentReply == null)
                throw new ArgumentException("Comment does not exist");

            if (!isAuthorized)
                throw new UnauthorizedAccessException("User is not authorized to delete comment");

            commentReply.IsDeleted = true;

            commentReply.DateTimeUser.UpdateUserTime(user.Email);

            Comment comment = commentReply as Comment;
            if (comment != null)
            {
                comment.DeleteReplies(user.Email);
            }
            //box.DecreaseCommentCounter();

            m_BoxRepository.Save(box);
            m_CommentRepository.Save(commentReply);
        }
    }
}
