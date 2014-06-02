using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class AddAnswerToQuestionCommandHandler : ICommandHandlerAsync<AddAnswerToQuestionCommand>
    {
        private readonly IUserRepository m_UserRepository;
        private readonly IBoxRepository m_BoxRepository;
        private readonly IRepository<CommentReplies> m_AnswerRepository;
        private readonly IRepository<Comment> m_QuestionRepository;
        private readonly IRepository<Item> m_ItemRepository;
        private readonly IRepository<Reputation> m_ReputationRepository;
        private readonly IQueueProvider m_QueueProvider;



        public AddAnswerToQuestionCommandHandler(IUserRepository userRepository, IBoxRepository boxRepository,
            IRepository<CommentReplies> answerRepository,
            IRepository<Comment> questionRepository,
            IRepository<Item> itemRepository,
            IRepository<Reputation> reputationRepository,
            IQueueProvider queueProvider)
        {
            m_UserRepository = userRepository;
            m_BoxRepository = boxRepository;
            m_AnswerRepository = answerRepository;
            m_QuestionRepository = questionRepository;
            m_ItemRepository = itemRepository;
            m_ReputationRepository = reputationRepository;
            m_QueueProvider = queueProvider;
        }
        public async Task HandleAsync(AddAnswerToQuestionCommand message)
        {
            Throw.OnNull(message, "message");

            var user = m_UserRepository.Load(message.UserId);
            var box = m_BoxRepository.Load(message.BoxId);
            var question = m_QuestionRepository.Load(message.QuestionId);
            //Decode the comment to html friendly
            var text = TextManipulation.EncodeComment(message.Text);

            var userType = m_UserRepository.GetUserToBoxRelationShipType(message.UserId, message.BoxId); //user.GetUserType(box.Id);
            if (userType == UserRelationshipType.None || userType == UserRelationshipType.Invite)
            {
                throw new UnauthorizedAccessException("User is not connected to box");
            }
            var files = message.FilesIds.Select(s => m_ItemRepository.Load(s)).ToList();
            var answer = new CommentReplies(user, text, box, message.Id, question, files);
            box.UpdateQnACount(m_BoxRepository.QnACount(box.Id) + 1);
            var reputation = user.AddReputation(ReputationAction.AddAnswer);

            await m_QueueProvider.InsertMessageToTranactionAsync(new UpdateData(user.Id, box.Id, null, null, answer.Id));
            m_ReputationRepository.Save(reputation);
            m_BoxRepository.Save(box);
            m_AnswerRepository.Save(answer);
            m_UserRepository.Save(user);
        }
    }
}
