using System;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands.Quiz;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers.Quiz
{
    public class CreateDiscussionCommandHandler : ICommandHandlerAsync<CreateDiscussionCommand>
    {
        // private readonly IIdGenerator m_IdGenerator;
        private readonly IRepository<Question> m_QuestionRepository;
        //private readonly IRepository<Domain.Quiz> m_QuizRepository;
        private readonly IRepository<QuizDiscussion> m_DiscussionRepository;
        private readonly IUserRepository m_UserRepository;
        private readonly IQueueProvider m_QueueProvider;

        public CreateDiscussionCommandHandler(
            IUserRepository userRepository,
            IRepository<Question> questionRepository,
            IRepository<QuizDiscussion> discussionRepository,
            //IRepository<Domain.Quiz> quizRepository, 
            IQueueProvider queueProvider)
        {
            m_QuestionRepository = questionRepository;
            m_DiscussionRepository = discussionRepository;
            m_UserRepository = userRepository;
           // m_QuizRepository = quizRepository;
            m_QueueProvider = queueProvider;
        }
        public Task HandleAsync(CreateDiscussionCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            if (string.IsNullOrEmpty(message.Text))
            {
                throw new NullReferenceException("message.Text");
            }

            var user = m_UserRepository.Load(message.UserId);
            var question = m_QuestionRepository.Load(message.QuestionId);

            var discussion = new QuizDiscussion(message.DiscussionId, user, message.Text, question);
            //to get to box we have 3 selects.
            discussion.Quiz.Box.UserTime.UpdateUserTime(user.Id);
            discussion.Quiz.Box.ShouldMakeDirty = () => false;
            m_DiscussionRepository.Save(discussion);
            return m_QueueProvider.InsertMessageToTransactionAsync(new UpdateData(user.Id, question.Quiz.Box.Id, quizDiscussionId: discussion.Id));
        }
    }
}
