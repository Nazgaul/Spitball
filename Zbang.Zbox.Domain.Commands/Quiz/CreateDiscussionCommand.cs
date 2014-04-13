using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands.Quiz
{
    public class CreateDiscussionCommand : ICommand
    {
        public CreateDiscussionCommand(long userId, string text, Guid questionId, Guid disucssionId)
        {
            UserId = userId;
            Text = text;
            QuestionId = questionId;
            DiscussionId = disucssionId;
        }

        public long UserId { get; private set; }
        public string Text { get; private set; }
        public Guid QuestionId { get; private set; }

        public Guid DiscussionId { get; private set; }
    }
}
