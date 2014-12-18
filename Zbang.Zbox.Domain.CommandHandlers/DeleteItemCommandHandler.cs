﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Thumbnail;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class DeleteItemCommandHandler : ICommandHandlerAsync<DeleteItemCommand>
    {

        private readonly IRepository<Box> m_BoxRepository;
        private readonly IUserRepository m_UserRepository;
        private readonly IBlobProvider m_BlobProvider;
        private readonly IRepository<Item> m_ItemRepository;
        private readonly IRepository<CommentReplies> m_CommentRepliesRepository;
        private readonly IQueueProvider m_QueueProvider;

        public DeleteItemCommandHandler(

            IRepository<Box> boxRepository, IBlobProvider blobProvider,
            IUserRepository userRepository,
            IRepository<Item> itemRepository,
            IRepository<CommentReplies> commentRepliesRepository, IQueueProvider queueProvider)
        {

            m_BoxRepository = boxRepository;
            m_BlobProvider = blobProvider;
            m_UserRepository = userRepository;
            m_ItemRepository = itemRepository;
            m_CommentRepliesRepository = commentRepliesRepository;
            m_QueueProvider = queueProvider;
        }

        public Task HandleAsync(DeleteItemCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");
            var userType = m_UserRepository.GetUserToBoxRelationShipType(command.UserId, command.BoxId);


            Item item = m_ItemRepository.Get(command.ItemId);
            User user = m_UserRepository.Load(command.UserId);


            bool isAuthorize = userType == UserRelationshipType.Owner
                || Equals(item.Uploader, user)
                || user.Reputation > user.University.AdminScore;

            if (!isAuthorize)
            {
                throw new UnauthorizedAccessException("User is unauthorized to delete file");
            }

            item.DateTimeUser.UpdateUserTime(user.Email);


            var file = item as File;

            Box box = m_BoxRepository.Load(command.BoxId);
            
            if (file != null)
            {
                if (file.ThumbnailBlobName == box.Picture)
                {
                    ChangeBoxPicture(box, item.Id);
                }
            }

            var uploaderFileId = item.Uploader.Id;

            var usersAffectReputation = new List<long> { uploaderFileId };
            usersAffectReputation.AddRange(item.GetItemCommentsUserIds());


            if (item.Answer != null && string.IsNullOrEmpty(item.Answer.Text))
            {
                m_CommentRepliesRepository.Delete(item.Answer);
            }
            if (item.Comment != null)
            {
                var shouldRemove = item.Comment.RemoveItem(item);
                if (shouldRemove)
                {
                    usersAffectReputation.AddRange(box.DeleteComment(item.Comment));
                }
            }
            var t1 = m_QueueProvider.InsertMessageToTranactionAsync(new ReputationData(usersAffectReputation));
            var t2 = m_QueueProvider.InsertMessageToTranactionAsync(new QuotaData(uploaderFileId));
            m_ItemRepository.Delete(item);
            box.UpdateItemCount();
            m_BoxRepository.Save(box);

            return Task.WhenAll(t1, t2);
        }

        private void ChangeBoxPicture(Box box, long itemId)
        {
            //TODO: LINQ NHIBERNATE

            var itemToTakePicture = box.Items.OfType<File>().Where(w => w.Id != itemId && w.IsDeleted == false)
                .OrderBy(o => o.DateTimeUser.CreationTime).FirstOrDefault();
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
            var thumbnailUrl = m_BlobProvider.GetThumbnailUrl(itemToTakePicture.ThumbnailBlobName);
            box.AddPicture(itemToTakePicture.ThumbnailBlobName, thumbnailUrl);
        }


    }
}
