using System;
using System.IO;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class ChangeFileNameCommandHandler : ICommandHandler<ChangeFileNameCommand, ChangeFileNameCommandResult>
    {
        readonly IUserRepository m_UserRepository;
        readonly IItemRepository m_ItemRepository;
        public ChangeFileNameCommandHandler(IUserRepository userRepository, IItemRepository itemRepository)
        {
            m_UserRepository = userRepository;
            m_ItemRepository = itemRepository;
        }
        public ChangeFileNameCommandResult Execute(ChangeFileNameCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            var user = m_UserRepository.Load(command.UserId);
            var item = m_ItemRepository.Load(command.FileId);
            var fileName = GetUniqueFileNameToBox(command.NewFileName, item.BoxId);
            var newName = item.ChangeName(fileName);

            item.DateTimeUser.UpdateUserTime(user.Id);
            m_ItemRepository.Save(item);

            return new ChangeFileNameCommandResult(newName, item.Url);
        }


        private string GetUniqueFileNameToBox(string fileName, long boxId)
        {
            var origFileName = fileName.RemoveEndOfString(Item.NameLength);
            var fileExists = m_ItemRepository.CheckFileNameExists(origFileName, boxId);

            if (fileExists)
            {
                var index = 0;
                //Find next available index
                do
                {
                    index++;
                    fileName = $"{Path.GetFileNameWithoutExtension(origFileName)}({index}){Path.GetExtension(fileName)}";
                    fileExists = m_ItemRepository.CheckFileNameExists(fileName, boxId);
                } while (fileExists);
            }
            return fileName;
        }
    }
}
