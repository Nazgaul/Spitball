using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class AddQuizLikeCommandHandler : ICommandHandlerAsync<AddQuizLikeCommand>
    {
        private readonly IRepository<QuizLike> m_QuizLikeRepository;
        private readonly IRepository<Domain.Quiz> m_QuizRepository;
        private readonly IGuidIdGenerator m_GuidGenerator;
        private readonly IUserRepository m_UserRepository;
        private readonly IQueueProvider m_QueueProvider;

        public AddQuizLikeCommandHandler(IRepository<QuizLike> quizLikeRepository, IRepository<Domain.Quiz> quizRepository, IGuidIdGenerator guidGenerator, IUserRepository userRepository, IQueueProvider queueProvider)
        {
            m_QuizLikeRepository = quizLikeRepository;
            m_QuizRepository = quizRepository;
            m_GuidGenerator = guidGenerator;
            m_UserRepository = userRepository;
            m_QueueProvider = queueProvider;
        }

        public Task HandleAsync(AddQuizLikeCommand message)
        {
            var user = m_UserRepository.Load(message.UserId);
            var quiz = m_QuizRepository.Load(message.QuizId);
            quiz.LikeCount = quiz.Likes.Count + 1;
            var like = new QuizLike(m_GuidGenerator.GetId(), user, quiz);
            m_QuizLikeRepository.Save(like);
            quiz.ShouldMakeDirty = () => true;
            m_QuizRepository.Save(quiz);
            message.Id = like.Id;
            var t1 = m_QueueProvider.InsertMessageToTransactionAsync(new ReputationData(quiz.User.Id));
            var t2 = m_QueueProvider.InsertMessageToTransactionAsync(new LikesBadgeData(message.UserId));
            return Task.WhenAll(t1, t2);
        }
    }
}