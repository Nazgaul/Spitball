using System;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class DeleteQuizLikeCommandHandler : ICommandHandlerAsync<DeleteQuizLikeCommand>
    {
        private readonly IRepository<QuizLike> m_QuizLikeRepository;
        private readonly IRepository<Domain.Quiz> m_QuizRepository;
        private readonly IQueueProvider _queueProvider;

        public DeleteQuizLikeCommandHandler(IRepository<QuizLike> quizLikeRepository, IRepository<Domain.Quiz> quizRepository, IQueueProvider queueProvider)
        {
            m_QuizLikeRepository = quizLikeRepository;
            m_QuizRepository = quizRepository;
            _queueProvider = queueProvider;
        }

        public Task HandleAsync(DeleteQuizLikeCommand message)
        {
            var like = m_QuizLikeRepository.Load(message.Id);
            if (like.User.Id != message.UserId)
            {
                throw new UnauthorizedAccessException();
            }
            like.Quiz.LikeCount = like.Quiz.Likes.Count - 1;
            m_QuizLikeRepository.Delete(like);
            like.Quiz.ShouldMakeDirty = () => true;
            m_QuizRepository.Save(like.Quiz);
            var t1 = _queueProvider.InsertMessageToTransactionAsync(new ReputationData(like.Quiz.User.Id));
            var t2 = _queueProvider.InsertMessageToTransactionAsync(new LikesBadgeData(message.UserId));
            return Task.WhenAll(t1, t2);
        }
    }
}