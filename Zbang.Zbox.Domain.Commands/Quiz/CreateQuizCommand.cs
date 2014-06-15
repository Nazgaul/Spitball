using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands.Quiz
{
    public class CreateQuizCommand : ICommand
    {
        public CreateQuizCommand(long userId, long quizId, string text, long boxId)
        {
            UserId = userId;
            QuizId = quizId;
            Text = text;
            BoxId = boxId;
        }
        public long UserId { get; private set; }
        public long QuizId { get; private set; }

        public string Text { get; private set; }

        public long BoxId { get; private set; }

    }
}
