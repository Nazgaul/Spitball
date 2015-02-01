﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class AddQuestionCommandHandler : ICommandHandlerAsync<AddCommentCommand>
    {
        private readonly IUserRepository m_UserRepository;
        private readonly IBoxRepository m_BoxRepository;
        private readonly IRepository<Comment> m_CommentRepository;
        private readonly IRepository<Item> m_ItemRepository;
        private readonly IRepository<Reputation> m_ReputationRepository;
        private readonly IQueueProvider m_QueueProvider;



        public AddQuestionCommandHandler(IUserRepository userRepository,
            IBoxRepository boxRepository,
            IRepository<Comment> commentRepository
            , IRepository<Item> itemRepository,
            IRepository<Reputation> reputationRepository,
            IQueueProvider queueProvider)
        {
            m_UserRepository = userRepository;
            m_BoxRepository = boxRepository;
            m_CommentRepository = commentRepository;
            m_ItemRepository = itemRepository;
            m_ReputationRepository = reputationRepository;
            m_QueueProvider = queueProvider;
        }
        public Task HandleAsync(AddCommentCommand message)
        {
            if (message == null) throw new ArgumentNullException("message");


            var user = m_UserRepository.Load(message.UserId);
            var box = m_BoxRepository.Load(message.BoxId);
            //Decode the comment to html friendly
            var text = message.Text;
            if (message.ShouldEncode)
            {
                text = TextManipulation.EncodeComment(message.Text);
            }


            var files = new List<Item>();
            if (message.FilesIds != null)
            {
                files = message.FilesIds.Select(s => m_ItemRepository.Load(s)).ToList();
            }
            var comment = box.AddComment(user, text, message.Id, files, FeedType.None);
            m_CommentRepository.Save(comment);

            var reputation = user.AddReputation(ReputationAction.AddQuestion);
            m_ReputationRepository.Save(reputation);
            m_BoxRepository.Save(box);
            var t1 = m_QueueProvider.InsertMessageToTranactionAsync(new UpdateData(user.Id, box.Id, questionId: comment.Id));
            var t2 = m_QueueProvider.InsertMessageToTranactionAsync(new ReputationData(user.Id));

            return Task.WhenAll(t1, t2);


        }
    }
}
