﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class DeleteCommentCommand : ICommand
    {
        public DeleteCommentCommand(Guid questionId, long userId)
        {
            QuestionId = questionId;
            UserId = userId;
        }
        public Guid QuestionId { get; private set; }

        public long UserId { get; private set; }
    }
}
