using System;
using System.Linq;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Thumbnail;
using Zbang.Zbox.Infrastructure.WebWorkerRoleJoinData.QueueDataTransfer;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.Infrastructure.File;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class AddFileToBoxCommandHandler : ICommandHandler<AddFileToBoxCommand, AddFileToBoxCommandResult>
    {

        private readonly IBoxRepository m_BoxRepository;
        private readonly IUserRepository m_UserRepository;
        private readonly IQueueProvider m_QueueProvider;
        private readonly IRepository<Item> m_ItemRepository;
        private readonly IItemTabRepository m_ItemTabRepository;
        private readonly IFileProcessorFactory m_FileProcessorFactory;

        private const int UploadReputationRate = 10;


        public AddFileToBoxCommandHandler(IQueueProvider queueProvider,
            IBoxRepository boxRepository, IUserRepository userRepository,
            IRepository<Item> itemRepository,
            IItemTabRepository itemTabRepository,
            IFileProcessorFactory fileProcessorFactory
            )
        {
            m_BoxRepository = boxRepository;
            m_UserRepository = userRepository;
            m_QueueProvider = queueProvider;
            m_ItemRepository = itemRepository;
            m_ItemTabRepository = itemTabRepository;
            m_FileProcessorFactory = fileProcessorFactory;
        }

        public AddFileToBoxCommandResult Execute(AddFileToBoxCommand command)
        {

            var box = m_BoxRepository.Get(command.BoxId);
            var user = m_UserRepository.Get(command.UserId);

            Throw.OnNull(user, "user");
            Throw.OnNull(box, "box");
            if (user.Quota.FreeSpace < command.Length)
            {
                throw new FileQuotaExceedException();
            }
            UserRelationshipType type = m_UserRepository.GetUserToBoxRelationShipType(command.UserId, command.BoxId);
            if (type == UserRelationshipType.Invite || type == UserRelationshipType.None)
            {
                user.ChangeUserRelationShipToBoxType(box, UserRelationshipType.Subscribe);
                m_UserRepository.Save(user);
            }

            
            var processor = m_FileProcessorFactory.GetProcessor(new Uri(BlobProvider.GetBlobUrl(command.BlobAddressName)));
            string thumbnailImgLink = ThumbnailProvider.DefaultFileTypePicture;
            if (processor != null)
            {
                thumbnailImgLink = processor.GetDefaultThumbnailPicture();
            }

            //CheckPermission(userType);
            var item = box.AddFile(command.FileName.RemoveEndOfString(Item.NameLength),
                user, command.Length,
                command.BlobAddressName,
                thumbnailImgLink);

            m_ItemRepository.Save(item, true);
            box.UserTime.UpdateUserTime(user.Name);

            m_BoxRepository.Save(box, true);

            user.Quota.UsedSpace = m_UserRepository.GetItemsByUser(user.Id).Sum(s => s.Size);
            user.AddReputation(UploadReputationRate);
            m_UserRepository.Save(user);


            AddItemToTab(command.TabId, item);

            TriggerCacheDocument(command.BlobAddressName, item.Id);

            var result = new AddFileToBoxCommandResult(item);

            return result;
        }

        private void AddItemToTab(Guid? tabid, File item)
        {
            if (!tabid.HasValue)
            {
                return;
            }
            var itemTab = m_ItemTabRepository.Get(tabid);
            itemTab.AddItemToTab(item);
            m_ItemTabRepository.Save(itemTab);
        }

        private void TriggerCacheDocument(string blobAddress, long itemId)
        {
            Uri uri = new Uri(BlobProvider.GetBlobUrl(blobAddress));

            var queueMessage = new FileProcessData { BlobName = uri, ItemId = itemId };
            m_QueueProvider.InsertMessageToCache(queueMessage);

        }
    }
}
