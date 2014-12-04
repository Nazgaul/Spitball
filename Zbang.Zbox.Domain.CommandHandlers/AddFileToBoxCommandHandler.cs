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
using Zbang.Zbox.Infrastructure.Thumbnail;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.Infrastructure.File;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class AddFileToBoxCommandHandler : ICommandHandlerAsync<AddFileToBoxCommand, AddFileToBoxCommandResult>
    {

        private readonly IBoxRepository m_BoxRepository;
        private readonly IUserRepository m_UserRepository;
        private readonly IQueueProvider m_QueueProvider;
        private readonly IItemRepository m_ItemRepository;
        private readonly IRepository<Reputation> m_ReputationRepository;
        private readonly IItemTabRepository m_ItemTabRepository;
        private readonly IFileProcessorFactory m_FileProcessorFactory;
        private readonly IBlobProvider m_BlobProvider;
        private readonly IIdGenerator m_IdGenerator;
        private readonly IRepository<Comment> m_CommentRepository;



        public AddFileToBoxCommandHandler(IQueueProvider queueProvider,
            IBoxRepository boxRepository, IUserRepository userRepository,
            IItemRepository itemRepository,
            IItemTabRepository itemTabRepository,
            IFileProcessorFactory fileProcessorFactory,
            IRepository<Reputation> reputationRepository,
            IBlobProvider blobProvider, IIdGenerator idGenerator, IRepository<Comment> commentRepository)
        {
            m_BoxRepository = boxRepository;
            m_UserRepository = userRepository;
            m_QueueProvider = queueProvider;
            m_ItemRepository = itemRepository;
            m_ItemTabRepository = itemTabRepository;
            m_FileProcessorFactory = fileProcessorFactory;
            m_ReputationRepository = reputationRepository;
            m_BlobProvider = blobProvider;
            m_IdGenerator = idGenerator;
            m_CommentRepository = commentRepository;
        }

        public async Task<AddFileToBoxCommandResult> ExecuteAsync(AddFileToBoxCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");

            var box = m_BoxRepository.Load(command.BoxId);
            var user = m_UserRepository.Load(command.UserId);

            if (user.Quota.FreeSpace < command.Length)
            {
                throw new FileQuotaExceedException();
            }

            AddUserToBox(command, box, user);



            var processor = m_FileProcessorFactory.GetProcessor(new Uri(m_BlobProvider.GetBlobUrl(command.BlobAddressName)));
            string thumbnailImgLink = ThumbnailProvider.DefaultFileTypePicture;
            if (processor != null)
            {
                thumbnailImgLink = processor.GetDefaultThumbnailPicture();
            }
            var fileName = GetUniqueFileNameToBox(command.FileName, box);

            var item = box.AddFile(fileName,
                user,
                command.Length,
                command.BlobAddressName,
                thumbnailImgLink,
                m_BlobProvider.GetThumbnailUrl(thumbnailImgLink));

            m_ItemRepository.Save(item, true);
            item.GenerateUrl();
            m_ItemRepository.Save(item);

            if (!command.IsQuestion)
            {
                var comment = m_ItemRepository.GetPreviousCommentId(box, user) ??
                             new Comment(user, null, box, m_IdGenerator.GetId(), null);
                comment.AddItem(item);
                m_CommentRepository.Save(comment);
            }

            box.UserTime.UpdateUserTime(user.Name);

            m_BoxRepository.Save(box, true);

            user.Quota.UsedSpace = m_UserRepository.GetItemsByUser(user.Id);
            m_ReputationRepository.Save(user.AddReputation(ReputationAction.AddItem));
            m_UserRepository.Save(user);

            AddItemToTab(command.TabId, item);

            var t1 = TriggerCacheDocument(command.BlobAddressName, item.Id);
            var t2 = m_QueueProvider.InsertMessageToTranactionAsync(new UpdateData(user.Id, box.Id, item.Id));

            await Task.WhenAll(t1, t2);

            var result = new AddFileToBoxCommandResult(item);

            return result;
        }

        private string GetUniqueFileNameToBox(string fileName, Box box)
        {
            var origFileName = fileName.RemoveEndOfString(Item.NameLength);
            var fileExists = m_ItemRepository.CheckFileNameExists(origFileName, box);

            if (fileExists)
            {
                var index = 0;
                //Find next available index
                do
                {
                    index++;
                    fileName = string.Format("{0}({1}){2}", Path.GetFileNameWithoutExtension(origFileName), index,
                        Path.GetExtension(fileName));
                    fileExists = m_ItemRepository.CheckFileNameExists(fileName, box);
                } while (fileExists);
            }
            return fileName;
        }

        private void AddUserToBox(AddFileToBoxCommand command, Box box, User user)
        {
            var type = m_UserRepository.GetUserToBoxRelationShipType(command.UserId, command.BoxId);
            if (type == UserRelationshipType.Invite || type == UserRelationshipType.None)
            {
                user.ChangeUserRelationShipToBoxType(box, UserRelationshipType.Subscribe);
                box.CalculateMembers();
                m_UserRepository.Save(user);
            }
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

        private Task TriggerCacheDocument(string blobAddress, long itemId)
        {
            var uri = new Uri(m_BlobProvider.GetBlobUrl(blobAddress));

            var queueMessage = new FileProcessData { BlobName = uri, ItemId = itemId };
            return m_QueueProvider.InsertMessageToCacheAsync(queueMessage);

        }

    }
}
