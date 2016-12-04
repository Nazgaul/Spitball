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
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class AddLinkToBoxCommandHandler : ICommandHandlerAsync<AddItemToBoxCommand, AddItemToBoxCommandResult>
    {
        private const int LinkStorageSize = 1;

        private readonly IRepository<Box> m_BoxRepository;
        private readonly IUserRepository m_UserRepository;
        private readonly IQueueProvider m_QueueProvider;
        private readonly IItemRepository m_ItemRepository;
        private readonly IItemTabRepository m_ItemTabRepository;
        private readonly IGuidIdGenerator m_IdGenerator;
        private readonly IRepository<Comment> m_CommentRepository;


        public AddLinkToBoxCommandHandler(IRepository<Box> boxRepository, IUserRepository userRepository, IQueueProvider queueProvider,
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

            var command = itemCommand as AddLinkToBoxCommand;
            if (command == null) throw new NullReferenceException("command");

            Uri u = CheckUrl(command);
            //Get Box
            var box = m_BoxRepository.Load(command.BoxId);
            var user = m_UserRepository.Load(command.UserId);

            //Add link to Box 
            var link = box.AddLink(u.AbsoluteUri, user, LinkStorageSize, command.UrlTitle);

            m_ItemRepository.Save(link, true);
            link.GenerateUrl();
            m_ItemRepository.Save(link);

            if (!command.IsQuestion)
            {
                var comment = m_ItemRepository.GetPreviousCommentId(box.Id, command.UserId) ??
                             box.AddComment(user, null, m_IdGenerator.GetId(), null, FeedType.AddedItems);
                comment.AddItem(link);
                m_CommentRepository.Save(comment);
            }

            box.UserTime.UpdateUserTime(user.Id);
            m_BoxRepository.Save(box, true);
            AddItemToTab(command.TabId, link);// DUPLICATE in FILE as well

            var t1 = m_QueueProvider.InsertMessageToTranactionAsync(new UpdateData(user.Id, box.Id, itemId: link.Id));
            var t4 = m_QueueProvider.InsertMessageToTranactionAsync(new UploadItemsBadgeData(user.Id));

            await Task.WhenAll(t1, t4);
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
            if (Validation.IsUrlWithoutScheme(url))
            {
                url = $"http://{url}";
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
