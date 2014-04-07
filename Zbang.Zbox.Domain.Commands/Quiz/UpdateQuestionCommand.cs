using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands.Quiz
{
    public class UpdateQuestionCommand : ICommand
    {
        public UpdateQuestionCommand(long userId, Guid questionId, string newText)
        {
            UserId = userId;
            QuestionId = questionId;
            NewText = newText;
        }
        public long UserId { get; private set; }
        public Guid QuestionId { get; private set; }
        public string NewText { get; private set; }
    }
}
