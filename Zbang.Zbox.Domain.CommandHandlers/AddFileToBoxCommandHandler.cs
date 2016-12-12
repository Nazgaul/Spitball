using System;
using System.IO;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class AddFileToBoxCommandHandler : ICommandHandlerAsync<AddItemToBoxCommand, AddItemToBoxCommandResult>
    {

        private readonly IBoxRepository m_BoxRepository;
        private readonly IUserRepository m_UserRepository;
        private readonly IQueueProvider m_QueueProvider;
        private readonly IItemRepository m_ItemRepository;
        private readonly IItemTabRepository m_ItemTabRepository;
        private readonly IGuidIdGenerator m_IdGenerator;
        private readonly IRepository<Comment> m_CommentRepository;



        public AddFileToBoxCommandHandler(IQueueProvider queueProvider,
            IBoxRepository boxRepository, IUserRepository userRepository,
            IItemRepository itemRepository,
            IItemTabRepository itemTabRepository,
            IGuidIdGenerator idGenerator, IRepository<Comment> commentRepository)
        {
            m_BoxRepository = boxRepository;
            m_UserRepository = userRepository;
            m_QueueProvider = queueProvider;
            m_ItemRepository = itemRepository;
            m_ItemTabRepository = itemTabRepository;
            m_IdGenerator = idGenerator;
            m_CommentRepository = commentRepository;
        }

        public async Task<AddItemToBoxCommandResult> ExecuteAsync(AddItemToBoxCommand itemCommand)
        {
            if (itemCommand == null) throw new ArgumentNullException(nameof(itemCommand));

            var command = itemCommand as AddFileToBoxCommand;
            if (command == null) throw new NullReferenceException("command");


            var box = m_BoxRepository.Load(command.BoxId);
            var user = m_UserRepository.Load(command.UserId);

            if (user.Quota.FreeSpace < command.Length)
            {
                throw new FileQuotaExceedException();
            }


            var fileName = GetUniqueFileNameToBox(command.FileName, box.Id);

            var item = box.AddFile(fileName,
                user,
                command.Length,
                command.BlobAddressName);

            m_ItemRepository.Save(item, true);
            item.GenerateUrl();
            m_ItemRepository.Save(item);

            if (!command.IsQuestion)
            {
                var comment = m_ItemRepository.GetPreviousCommentId(box.Id, user.Id) ??
                            box.AddComment(user, null, m_IdGenerator.GetId(), null, FeedType.AddedItems);
                comment.AddItem(item);
                m_CommentRepository.Save(comment);
            }

            box.UserTime.UpdateUserTime(user.Id);

            m_BoxRepository.Save(box);

            AddItemToTab(command.TabId, item);
            var t2 = m_QueueProvider.InsertMessageToTranactionAsync(new UpdateData(user.Id, box.Id, itemId: item.Id));
            var t4 = m_QueueProvider.InsertMessageToTranactionAsync(new UploadItemsBadgeData(user.Id));
            var t3 = m_QueueProvider.InsertMessageToTranactionAsync(new QuotaData(user.Id));
            var t1 = m_QueueProvider.InsertFileMessageAsync(new BoxFileProcessData(item.Id));
            var t5 = m_QueueProvider.InsertFileMessageAsync(new BoxProcessData(item.Box.Id));

            await Task.WhenAll(t1, t2, t3, t4, t5);

            var result = new AddFileToBoxCommandResult(item);

            return result;
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

        private void AddItemToTab(Guid? tabid, Item item)
        {
            if (!tabid.HasValue)
            {
                return;
            }
            var itemTab = m_ItemTabRepository.Get(tabid);
            itemTab.AddItemToTab(item);
            m_ItemTabRepository.Save(itemTab);
        }

    }
}
