using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class DeleteAnnotationCommandHandler : ICommandHandler<DeleteAnnotationCommand>
    {
        private readonly IUserRepository m_UserRepository;
        private readonly IRepository<ItemComment> m_ItemCommentRepository;

        public DeleteAnnotationCommandHandler(IUserRepository userRepository, IRepository<ItemComment> itemCommentRepository)
        {
            m_UserRepository = userRepository;
            m_ItemCommentRepository = itemCommentRepository;
        }
        public void Handle(DeleteAnnotationCommand message)
        {

            Throw.OnNull(message, "message");
            var user = m_UserRepository.Load(message.UserId);

            var itemComment = m_ItemCommentRepository.Load(message.AnnotationId);
            if (itemComment.Author != user)
            {
                throw new UnauthorizedAccessException("User is unauthorized to delete annotation");
            }

            m_ItemCommentRepository.Delete(itemComment);
        }
    }
}
