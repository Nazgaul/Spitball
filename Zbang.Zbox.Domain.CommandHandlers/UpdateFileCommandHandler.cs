using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Thumbnail;
using Zbang.Zbox.Infrastructure.WebWorkerRoleJoinData.QueueDataTransfer;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class UpdateFileCommandHandler : ICommandHandler<UpdateFileCommand>
    {
        readonly IRepository<File> m_ItemRepository;
        readonly IUserRepository m_UserRepository;
        readonly IQueueProvider m_QueueProvider;

        public UpdateFileCommandHandler(IRepository<File> itemRepository, IUserRepository userRepository, IQueueProvider queueProvider)
        {
            m_ItemRepository = itemRepository;
            m_UserRepository = userRepository;
            m_QueueProvider = queueProvider;
        }

        public void Handle(UpdateFileCommand message)
        {
            var file = m_ItemRepository.Get(message.ItemId);
            if (file == null)
            {
                throw new ArgumentException("file does not exisits");
            }

            var userType = m_UserRepository.GetUserToBoxRelationShipType(message.UserId, file.Box.Id);
            if (file.Uploader.Id != message.UserId || userType != Infrastructure.Enums.UserRelationshipType.Owner)
            {
                throw new UnauthorizedAccessException("User cannot update file content");
            }

            file.ItemContentUrl = message.BlobName;
            file.ThumbnailBlobName = ThumbnailProvider.DefaultFileTypePicture;
           // TriggerGenerateThumbnail(file.ItemContentUrl, file.Id);

            m_ItemRepository.Save(file);
           

        }
        //private void TriggerGenerateThumbnail(string blobName, long fileId)
        //{
        //    var queueThumbnailData = new GenerateThumbnail(blobName, fileId);
        //    m_QueueProvider.InsertMessageToThumbnail(queueThumbnailData);
        //}
    }
}
