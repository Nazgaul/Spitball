using System;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Thumbnail;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class AddLinkToBoxCommandHandler : ICommandHandlerAsync<AddLinkToBoxCommand, AddLinkToBoxCommandResult>
    {
        private const int LinkStorageSize = 1;

        private readonly IRepository<Box> m_BoxRepository;
        private readonly IUserRepository m_UserRepository;
        private readonly IQueueProvider m_QueueProvider;
        private readonly IItemRepository m_ItemRepository;
        private readonly IItemTabRepository m_ItemTabRepository;
        private readonly IRepository<Reputation> m_ReputationRepository;
        private readonly IBlobProvider m_BlobProvider;
        private readonly IIdGenerator m_IdGenerator;
        private readonly IRepository<Comment> m_CommentRepository;


        public AddLinkToBoxCommandHandler(IRepository<Box> boxRepository, IUserRepository userRepository, IQueueProvider queueProvider,
            IItemRepository itemRepository,
            IItemTabRepository itemTabRepository,
            IRepository<Reputation> reputationRepository,
            IBlobProvider blobProvider, IIdGenerator idGenerator, IRepository<Comment> commentRepository)
        {
            m_BoxRepository = boxRepository;
            m_UserRepository = userRepository;
            m_QueueProvider = queueProvider;
            m_ItemRepository = itemRepository;
            m_ItemTabRepository = itemTabRepository;
            m_ReputationRepository = reputationRepository;
            m_BlobProvider = blobProvider;
            m_IdGenerator = idGenerator;
            m_CommentRepository = commentRepository;
        }

        public async Task<AddLinkToBoxCommandResult> ExecuteAsync(AddLinkToBoxCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");
            Uri u = CheckUrl(command);
            //Get Box
            var box = m_BoxRepository.Get(command.BoxId);
            var user = m_UserRepository.Get(command.UserId);
            if (user == null)
            {
                throw new NullReferenceException("user");
            }
            if (user.Quota.FreeSpace < LinkStorageSize)
            {
                throw new Exception("File Size Exceeds Quota");
            }
            UserRelationshipType type = m_UserRepository.GetUserToBoxRelationShipType(user.Id, box.Id);
            if (type == UserRelationshipType.Invite || type == UserRelationshipType.None)
            {
                user.ChangeUserRelationShipToBoxType(box, UserRelationshipType.Subscribe);
                m_UserRepository.Save(user);
            }
            //Add link to Box 
            var link = box.AddLink(u.AbsoluteUri, user, LinkStorageSize, command.UrlTitle, ThumbnailProvider.LinkTypePicture,
               m_BlobProvider.GetThumbnailLinkUrl());

            m_ReputationRepository.Save(user.AddReputation(ReputationAction.AddItem));
            m_ItemRepository.Save(link, true);
            link.GenerateUrl();
            m_ItemRepository.Save(link);

            if (!command.IsQuestion)
            {
                var comment = m_ItemRepository.GetPreviousCommentId(box, user) ??
                             box.AddComment(user, null, m_IdGenerator.GetId(), null, true);
                comment.AddItem(link);
                m_CommentRepository.Save(comment);
            }

            box.UserTime.UpdateUserTime(user.Name);
            m_BoxRepository.Save(box, true);

            AddItemToTab(command.TabId, link);// DUPLICATE in FILE as well

            var t1 = m_QueueProvider.InsertMessageToTranactionAsync(new UpdateData(user.Id, box.Id, link.Id));
            var t2 = m_QueueProvider.InsertMessageToTranactionAsync(new ReputationData(user.Id));
            await Task.WhenAll(t1, t2);
            return new AddLinkToBoxCommandResult(link);


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

        private Uri CheckUrl(AddLinkToBoxCommand command)
        {
            string url = command.Url;
            if (Validation.IsUrlWithoutSceme(url))
            {
                url = string.Format("http://{0}", url);
            }
            Uri u;
            if (!Uri.TryCreate(url, UriKind.Absolute, out u))
            {
                throw new ArgumentException("Invalid url");
            }
            if (u.Scheme != Uri.UriSchemeHttp && u.Scheme != Uri.UriSchemeHttps)
            {
                throw new ArgumentException("Invalid url Scheme");

            }
            return u;
        }
    }
}
