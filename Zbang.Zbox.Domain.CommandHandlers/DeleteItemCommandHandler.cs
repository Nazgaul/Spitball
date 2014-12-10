using System;
using System.Linq;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Thumbnail;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class DeleteItemCommandHandler : ICommandHandler<DeleteItemCommand>
    {

        private readonly IRepository<Box> m_BoxRepository;
        private readonly IUserRepository m_UserRepository;
        private readonly IBlobProvider m_BlobProvider;
        private readonly IRepository<Updates> m_Updates;
        private readonly IRepository<Item> m_ItemRepository;
        private readonly IRepository<Reputation> m_ReputationRepository;
        private readonly IRepository<CommentReplies> m_CommentRepliesRepository;
        private readonly IRepository<Comment> m_CommentRepository;

        public DeleteItemCommandHandler(

            IRepository<Box> boxRepository, IBlobProvider blobProvider,
            IUserRepository userRepository,
            IRepository<Updates> updates,
            IRepository<Item> itemRepository,
            IRepository<Reputation> reputationRepository, IRepository<CommentReplies> commentRepliesRepository, IRepository<Comment> commentRepository)
        {

            m_BoxRepository = boxRepository;
            m_BlobProvider = blobProvider;
            m_UserRepository = userRepository;
            m_Updates = updates;
            m_ItemRepository = itemRepository;
            m_ReputationRepository = reputationRepository;
            m_CommentRepliesRepository = commentRepliesRepository;
            m_CommentRepository = commentRepository;
        }

        public void Handle(DeleteItemCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");
            var userType = m_UserRepository.GetUserToBoxRelationShipType(command.UserId, command.BoxId);//user.GetUserType(command.BoxId);
            if (userType == UserRelationshipType.None || userType == UserRelationshipType.Invite)
            {
                throw new UnauthorizedAccessException("User is not connected to box");
            }


            Item item = m_ItemRepository.Get(command.ItemId);
            User user = m_UserRepository.Load(command.UserId);

            
            bool isAuthorize = userType == UserRelationshipType.Owner 
                || Equals(item.Uploader, user) 
                || user.Reputation > user.University.AdminScore;

            if (!isAuthorize)
            {
                throw new UnauthorizedAccessException("User is unauthorized to delete file");
            }



            //item.DeleteAllComments(user.Email);
            m_ItemRepository.Delete(item);
            //item.IsDeleted = true;
            item.DateTimeUser.UpdateUserTime(user.Email);
            var file = item as File;

            Box box = m_BoxRepository.Get(command.BoxId);
            if (box == null)
            {
                throw new NullReferenceException("box");
            }
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
            uploaderFile.Quota.UsedSpace = m_UserRepository.GetItemsByUser(uploaderFile.Id);
            var reputation = uploaderFile.AddReputation(ReputationAction.DeleteItem);
            m_ReputationRepository.Save(reputation);



            if (item.Answer != null && string.IsNullOrEmpty(item.Answer.Text))
            {
                m_CommentRepliesRepository.Delete(item.Answer);
            }
            if (item.Comment != null)
            {
                var shouldRemove = item.Comment.RemoveItem(item);
                if (shouldRemove)
                {
                    m_ReputationRepository.Save(box.DeleteComment(item.Comment));
                    m_BoxRepository.Save(box);
                    //m_CommentRepository.Delete(item.Comment);
                }
            }


            m_BoxRepository.Save(box);
            m_UserRepository.Save(uploaderFile);
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
