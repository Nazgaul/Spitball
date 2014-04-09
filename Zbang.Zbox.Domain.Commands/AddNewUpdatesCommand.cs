using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class AddNewUpdatesCommand : ICommand
    {
        public AddNewUpdatesCommand(long boxId, long userId, Guid? questionId, Guid? answerId, long? itemId, long? quizId)
        {
            BoxId = boxId;
            UserId = userId;
            QuestionId = questionId;
            AnswerId = answerId;
            ItemId = itemId;
            QuizId = quizId;
        }
        public long BoxId { get; set; }

        public long UserId { get; set; }

        public Guid? QuestionId { get; set; }
        public Guid? AnswerId { get; set; }
        public long? ItemId { get; set; }
        public long? QuizId { get; set; }
    }
}
