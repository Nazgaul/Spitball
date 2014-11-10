using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class DeleteFileFromQnACommandHandler : ICommandHandler<DeleteFileFromQnACommand>
    {
        private readonly IRepository<Item> m_ItemRepository;
        private readonly IRepository<User> m_UserRepository;
        public DeleteFileFromQnACommandHandler(IRepository<Item> itemRepository, IRepository<User> userRepository)
        {
            m_ItemRepository = itemRepository;
            m_UserRepository = userRepository;
        }

        public void Handle(DeleteFileFromQnACommand message)
        {
            if (message == null) throw new ArgumentNullException("message");
            var item = m_ItemRepository.Load(message.ItemId);
            var user = m_UserRepository.Load(message.UserId);
            var box = item.Box;

            


            var authorize = item.Uploader.Id == message.UserId || box.Owner.Id == message.UserId || user.Reputation > user.University.AdminScore;
            if (!authorize)
            {
                throw new UnauthorizedAccessException("User is unauthorized to unlink file");
            }

            item.Answer = null;
            item.Question = null;

            m_ItemRepository.Save(item);
        }
    }
}
