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
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class AddReplyToCommentCommandHandler : ICommandHandlerAsync<AddReplyToCommentCommand>
    {
        private readonly IUserRepository m_UserRepository;
        private readonly IBoxRepository m_BoxRepository;
        private readonly IRepository<CommentReply> m_AnswerRepository;
        private readonly IRepository<Comment> m_QuestionRepository;
        private readonly IRepository<Item> m_ItemRepository;
        //private readonly IRepository<Reputation> m_ReputationRepository;
        private readonly IQueueProvider m_QueueProvider;



        public AddReplyToCommentCommandHandler(IUserRepository userRepository, IBoxRepository boxRepository,
            IRepository<CommentReply> answerRepository,
            IRepository<Comment> questionRepository,
            IRepository<Item> itemRepository,
           // IRepository<Reputation> reputationRepository,
            IQueueProvider queueProvider)
        {
            m_UserRepository = userRepository;
            m_BoxRepository = boxRepository;
            m_AnswerRepository = answerRepository;
            m_QuestionRepository = questionRepository;
            m_ItemRepository = itemRepository;
           // m_ReputationRepository = reputationRepository;
            m_QueueProvider = queueProvider;
        }
        public Task HandleAsync(AddReplyToCommentCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            var user = m_UserRepository.Load(message.UserId);
            var box = m_BoxRepository.Load(message.BoxId);

            var question = m_QuestionRepository.Load(message.QuestionId);


            var files = new List<Item>();
            if (message.FilesIds != null)
            {
                files = message.FilesIds.Select(s => m_ItemRepository.Load(s)).ToList();
            }
            var answer = new CommentReply(user, message.Text, box, message.Id, question, files);
            //var reputation = user.AddReputation(ReputationAction.AddReply);
            question.ReplyCount++;
            question.DateTimeUser.UpdateUserTime(user.Id);
            box.UserTime.UpdateUserTime(user.Id);
            var t1 = m_QueueProvider.InsertMessageToTranactionAsync(new UpdateData(user.Id, box.Id, answerId: answer.Id));
            //var t2 = m_QueueProvider.InsertMessageToTranactionAsync(new ReputationData(user.Id));

            m_QuestionRepository.Save(question);
            m_BoxRepository.Save(box);
            //m_ReputationRepository.Save(reputation);
            m_AnswerRepository.Save(answer);
            m_UserRepository.Save(user);
            return Task.WhenAll(t1);
        }
    }
}
