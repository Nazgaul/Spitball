using System;
using System.Linq;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Thumbnail;
using Zbang.Zbox.Infrastructure.WebWorkerRoleJoinData.QueueDataTransfer;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class DeleteItemCommandHandler : ICommandHandler<DeleteItemCommand>
    {

        private readonly IRepository<Box> m_BoxRepository;
        private readonly IUserRepository m_UserRepository;
        private readonly IBlobProvider m_BlobProvider;
        private readonly IQueueProvider m_QueueProvider;
        //private readonly IActionRepository m_ActionRepository;
        private readonly IRepository<Item> m_ItemRepository;

        public DeleteItemCommandHandler(IQueueProvider queueProvider,
            IRepository<Box> boxRepository, IBlobProvider blobProvider,
            IUserRepository userRepository, 
            //IActionRepository actionRepository,
            IRepository<Item> itemRepository)
        {

            m_BoxRepository = boxRepository;
            m_BlobProvider = blobProvider;
            m_QueueProvider = queueProvider;
            m_UserRepository = userRepository;
            m_ItemRepository = itemRepository;
        }

        public void Handle(DeleteItemCommand command)
        {
            var userType = m_UserRepository.GetUserToBoxRelationShipType(command.UserId, command.BoxId);//user.GetUserType(command.BoxId);
            if (userType == UserRelationshipType.None || userType == UserRelationshipType.Invite)
            {
                throw new UnauthorizedAccessException("User is not connected to box");
            }


            Item item = m_ItemRepository.Get(command.ItemId);
            User user = m_UserRepository.Load(command.UserId);

            bool isAuthorize = userType == UserRelationshipType.Owner || item.Uploader == user;

            if (!isAuthorize)
            {
                throw new UnauthorizedAccessException("User is unauthorized to delete file");
            }



            //item.DeleteAllComments(user.Email);

            item.IsDeleted = true;
            item.DateTimeUser.UpdateUserTime(user.Email);
            File file = item as File;

            Box box = m_BoxRepository.Get(command.BoxId);
            Throw.OnNull(box, "box");
            box.UpdateItemCount();
            if (file != null)
            {
                //if (!string.IsNullOrEmpty(file.BlobName))
                //{
                //    m_BlobProvider.DeleteFile(file.BlobName);
                //}

                if (file.ThumbnailBlobName == box.Picture)
                {
                    ChangeBoxPicture(box, item.Id);
                }
            }
            var uploaderFile = item.Uploader;
            uploaderFile.Quota.UsedSpace = m_UserRepository.GetItemsByUser(uploaderFile.Id).Sum(s => s.Size);
            uploaderFile.AddReputation(-5);

            m_BoxRepository.Save(box);
            m_UserRepository.Save(uploaderFile);
        }

        private void ChangeBoxPicture(Box box, long itemId)
        {
            var itemToTakePicture = box.Items.OfType<File>().Where(w => w.Id != itemId && w.IsDeleted == false).OrderBy(o => o.DateTimeUser.CreationTime).FirstOrDefault();
            if (itemToTakePicture == null)
            {
                box.RemovePicture();
                return;
            }
            if (itemToTakePicture.ThumbnailBlobName == ThumbnailProvider.DefaultFileTypePicture)
            {
                box.RemovePicture();
                return;
            }
            box.AddPicture(itemToTakePicture.ThumbnailBlobName);
        }
    }
}
