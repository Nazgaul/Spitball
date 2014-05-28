using System;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands.Quiz
{
    public class CreateAnswerCommand : ICommand
    {
        public CreateAnswerCommand(long userId, Guid id, string text,  Guid questionId)
        {
            UserId = userId;
            Id = id;
            Text = text;
            QuestionId = questionId;
        }
        public long UserId { get; private set; }
        public Guid Id { get; set; }
        public string Text { get; set; }
        public Guid QuestionId { get; set; }
    }
}
