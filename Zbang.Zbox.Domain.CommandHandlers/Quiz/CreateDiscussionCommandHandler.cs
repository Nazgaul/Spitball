using Zbang.Zbox.Domain.Commands.Quiz;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers.Quiz
{
    public class CreateDiscussionCommandHandler : ICommandHandler<CreateDiscussionCommand>
    {
       // private readonly IIdGenerator m_IdGenerator;
        private readonly IRepository<Question> m_QuestionRepository;
        private readonly IRepository<Discussion> m_DiscussionRepository;
        private readonly IUserRepository m_UserRepository;

        public CreateDiscussionCommandHandler(
            IUserRepository userRepository,
            IRepository<Question> questionRepository,
            IRepository<Discussion> discussionRepository

            )
        {
            m_QuestionRepository = questionRepository;
            m_DiscussionRepository = discussionRepository;
            m_UserRepository = userRepository;
        }
        public void Handle(CreateDiscussionCommand message)
        {
            Throw.OnNull(message, "message");
            Throw.OnNull(message.Text, "text", false);

            var user = m_UserRepository.Load(message.UserId);
            var question = m_QuestionRepository.Load(message.QuestionId);

            var discussion = new Discussion(message.DiscussionId, user, TextManipulation.EncodeText(message.Text), question);
            m_DiscussionRepository.Save(discussion);
        }
    }
}
