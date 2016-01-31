using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class LikeCommentCommandHandler : ICommandHandler<LikeCommentCommand>
    {
        private readonly ICommentLikeRepository m_CommentLikeRepository;
        private readonly IUserRepository m_UserRepository;
        private readonly IRepository<Comment> m_CommentRepository;
        private readonly IGuidIdGenerator m_GuidGenerator;

        public LikeCommentCommandHandler(ICommentLikeRepository commentLikeRepository, IUserRepository userRepository, IRepository<Comment> commentRepository, IGuidIdGenerator guidGenerator)
        {
            m_CommentLikeRepository = commentLikeRepository;
            m_UserRepository = userRepository;
            m_CommentRepository = commentRepository;
            m_GuidGenerator = guidGenerator;
        }

        public void Handle(LikeCommentCommand message)
        {
            if (message == null) throw new ArgumentNullException("message");

            var commentLike = m_CommentLikeRepository.GetUserLike(message.UserId, message.CommentId);
            var comment = m_CommentRepository.Load(message.CommentId);
            if (commentLike == null)
            {
            
                comment.LikeCount++;
                var user = m_UserRepository.Load(message.UserId);
                commentLike = new CommentLike(comment, user, m_GuidGenerator.GetId());

                m_CommentRepository.Save(comment);
                m_CommentLikeRepository.Save(commentLike);
                return;
            }

            comment.LikeCount--;
            m_CommentRepository.Save(comment);
            m_CommentLikeRepository.Delete(commentLike);


        }
    }
}
