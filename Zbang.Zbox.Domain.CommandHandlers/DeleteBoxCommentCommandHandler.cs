using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Domain.DataAccess;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class DeleteBoxCommentCommandHandler : ICommandHandler<DeleteBoxCommentCommand, DeleteBoxCommentCommandResult>
    {
        //Fields             
        private IRepository<Comment> m_CommentRepository;
        private IRepository<Box> m_boxRepository;
        private IUserRepository m_UserRepository;

        //Ctors
        public DeleteBoxCommentCommandHandler(IRepository<Comment> commentRepository,
            IRepository<Box> boxRepository, IUserRepository userRepository)
        {
            m_CommentRepository = commentRepository;
            m_boxRepository = boxRepository;
            m_UserRepository = userRepository;
        }

        //Methods
        public DeleteBoxCommentCommandResult Execute(DeleteBoxCommentCommand command)
        {
           // Box box = m_boxRepository.Get(command.BoxId);
            User user = m_UserRepository.GetUserByEmail(command.EmailId);
            Comment comment = m_CommentRepository.Get(command.CommentId);

            eUserPermissionSettings permission = user.GetUserPermission(command.BoxId);
            if (comment == null)
                throw new ArgumentException("Comment does not exist");

            bool isAuthorized = permission.HasFlag(eUserPermissionSettings.Owner) || permission.HasFlag(eUserPermissionSettings.Manager) || user == comment.Author;

            if (!isAuthorized)
                throw new UnauthorizedAccessException("User is not authorized to delete comment");

            comment.IsDeleted = true;
            //box.Comments.Remove(comment);
            //m_CommentRepository.Delete(comment);
            m_CommentRepository.Save(comment);
            //m_boxRepository.Save(box);

            return new DeleteBoxCommentCommandResult(); ;



        }
    }
}
