using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class AddCommentCommandHandler : ICommandHandlerAsync<AddCommentCommand, AddCommentCommandResult>
    {
        private readonly IUserRepository m_UserRepository;
        private readonly IBoxRepository m_BoxRepository;
        private readonly IRepository<Comment> m_CommentRepository;
        private readonly IRepository<Item> m_ItemRepository;
        private readonly IQueueProvider m_QueueProvider;

        private const long AnonymousUserId = 22886;

        public AddCommentCommandHandler(IUserRepository userRepository,
            IBoxRepository boxRepository,
            IRepository<Comment> commentRepository
            , IRepository<Item> itemRepository,
            IQueueProvider queueProvider)
        {
            m_UserRepository = userRepository;
            m_BoxRepository = boxRepository;
            m_CommentRepository = commentRepository;
            m_ItemRepository = itemRepository;
            m_QueueProvider = queueProvider;
        }

        public async Task<AddCommentCommandResult> ExecuteAsync(AddCommentCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            var userId = command.UserId;
            //if (command.PostAnonymously)
            //{
            //    userId = AnonymousUserId;
            //}

            var user = m_UserRepository.Load(userId);
            var box = m_BoxRepository.Load(command.BoxId);

            var files = new List<Item>();
            if (command.FilesIds != null)
            {
                files = command.FilesIds.Select(s => m_ItemRepository.Load(s)).ToList();
            }
            foreach (var file in files)
            {
                file.User = user;
                m_ItemRepository.Save(file);
            }
            var comment = box.AddComment(user, command.Text, command.Id, files, FeedType.None, command.PostAnonymously);
            m_CommentRepository.Save(comment);
            m_BoxRepository.Save(box);
            var t1 = m_QueueProvider.InsertMessageToTransactionAsync(new UpdateData(user.Id, box.Id, questionId: comment.Id));
            var t2 = m_QueueProvider.InsertFileMessageAsync(new BoxProcessData(box.Id));

            await Task.WhenAll(t1, t2).ConfigureAwait(true);

            if (command.PostAnonymously)
            {
                var anonymousUser = m_UserRepository.Load(AnonymousUserId);
                return new AddCommentCommandResult(command.Id, anonymousUser.Name, anonymousUser.ImageLarge, anonymousUser.Id);
            }

            return new AddCommentCommandResult(command.Id, user.Name, user.ImageLarge, user.Id);
        }


    }
}
