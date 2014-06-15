using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands.Quiz
{
    public class SaveQuizCommand : ICommand
    {
        public SaveQuizCommand(long userId, long quizId)
        {
            UserId = userId;
            QuizId = quizId;
        }
        public long UserId { get; private set; }
        public long QuizId { get; private set; }
    }
}
