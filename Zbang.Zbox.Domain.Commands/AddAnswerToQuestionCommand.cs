﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class AddAnswerToQuestionCommand:ICommand
    {
        public AddAnswerToQuestionCommand(long userId, long boxId, string text, Guid answerId, Guid questionId, IEnumerable<long> filesIds)
        {
            UserId = userId;
            BoxId = boxId;
            Text = text.Trim();
            Id = answerId;
            QuestionId = questionId;
            FilesIds = filesIds;
        }
        public long UserId { get; private set; }

        public long BoxId { get; private set; }

        public string Text { get; private set; }

        public Guid Id { get; private set; }

        public Guid QuestionId { get; private set; }
        public IEnumerable<long> FilesIds { get; private set; }
    }
}
