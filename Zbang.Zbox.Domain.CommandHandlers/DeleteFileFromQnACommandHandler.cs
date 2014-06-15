using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class DeleteFileFromQnACommandHandler : ICommandHandler<DeleteFileFromQnACommand>
    {
        private readonly IRepository<Item> m_ItemRepository;
        public DeleteFileFromQnACommandHandler(IRepository<Item> itemRepository)
        {
            m_ItemRepository = itemRepository;
        }
        public void Handle(DeleteFileFromQnACommand message)
        {
            var item = m_ItemRepository.Load(message.ItemId);
            var box = item.Box;

            var authorize = item.Uploader.Id != message.UserId || box.Owner.Id != message.UserId;
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
