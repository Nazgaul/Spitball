using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands.Quiz
{
    public class CreateAnswerCommand : ICommand
    {
        public CreateAnswerCommand(long userId, Guid id, string text, bool isCorrect, Guid questionId)
        {
            UserId = userId;
            Id = id;
            Text = text;
            IsCorrect = isCorrect;
            QuestionId = questionId;
        }
        public long UserId { get; private set; }
        public Guid Id { get; set; }
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
        public Guid QuestionId { get; set; }
    }
}
