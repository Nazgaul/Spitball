using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands.Quiz;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers.Quiz
{
    public class SaveQuizCommandHandler : ICommandHandlerAsync<SaveQuizCommand, SaveQuizCommandResult>
    {
        private readonly IRepository<Domain.Quiz> m_QuizRepository;
        private readonly IQueueProvider m_QueueProvider;
        private readonly IBoxRepository m_BoxRepository;
        private readonly IItemRepository m_ItemRepository;
        private readonly IGuidIdGenerator m_IdGenerator;
        private readonly IRepository<Comment> m_CommentRepository;
        // private readonly IUserRepository m_UserRepository;

        public SaveQuizCommandHandler(
            IRepository<Domain.Quiz> quizRepository,
            IQueueProvider queueProvider,
            IBoxRepository boxRepository,
            IItemRepository itemRepository,
            IGuidIdGenerator idGenerator,
            IRepository<Comment> commentRepository
            //IUserRepository userRepository
            )
        {
            m_QuizRepository = quizRepository;
            m_QueueProvider = queueProvider;
            m_BoxRepository = boxRepository;
            m_ItemRepository = itemRepository;
            m_IdGenerator = idGenerator;
            m_CommentRepository = commentRepository;
            //m_UserRepository = userRepository;
        }
        public async Task<SaveQuizCommandResult> ExecuteAsync(SaveQuizCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var quiz = m_QuizRepository.Load(message.QuizId);
            if (quiz.User.Id != message.UserId)
            {
                throw new UnauthorizedAccessException("user is not owner of quiz");
            }

            if (string.IsNullOrEmpty(quiz.Name))
            {
                throw new ArgumentException("quiz name is empty");
            }
            foreach (var question in quiz.Questions)
            {
                if (string.IsNullOrWhiteSpace(question.Text))
                {
                    throw new ArgumentException("question text is empty");
                }
                if (question.RightAnswer == null)
                {
                    throw new ArgumentException("question have no right answer");
                }
                if (question.Answers.Count < 2)
                {
                    throw new ArgumentException("question answers are below 2");
                }
                if (question.Answers.Any(w => w.Text == null))
                {
                    throw new ArgumentException("question answers don't have text");
                }
            }

            quiz.Publish = true;
            var sb = new StringBuilder();
            foreach (var question in quiz.Questions)
            {
                sb.Append($"{question.Text} ");
            }

            quiz.Content = sb.ToString().Substring(0, Math.Min(sb.Length, 254));

            quiz.Box.UserTime.UpdateUserTime(quiz.User.Id);
            quiz.Box.UpdateItemCount();
            quiz.GenerateUrl();
            m_BoxRepository.Save(quiz.Box);
            m_QuizRepository.Save(quiz);

            var comment = m_ItemRepository.GetPreviousCommentId(quiz.Box.Id, quiz.User.Id) ??
                         new Comment(quiz.User, null, quiz.Box, m_IdGenerator.GetId(), null, FeedType.AddedItems, false);
            comment.AddQuiz(quiz);
            m_CommentRepository.Save(comment);

            //m_UserRepository.Save(quiz.User);

            var t1 = m_QueueProvider.InsertMessageToTransactionAsync(new UpdateData(quiz.User.Id, quiz.Box.Id, quizId: quiz.Id));
            var t2 = m_QueueProvider.InsertMessageToTransactionAsync(new CreateQuizzesBadgeData(quiz.User.Id));
            var t5 = m_QueueProvider.InsertFileMessageAsync(new BoxProcessData(quiz.Box.Id));

            await Task.WhenAll(t1, t2, t5).ConfigureAwait(true);
            return new SaveQuizCommandResult(quiz.Url);
        }
    }
}
