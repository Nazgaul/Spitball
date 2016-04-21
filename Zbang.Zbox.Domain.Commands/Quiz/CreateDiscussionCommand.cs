﻿using System;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands.Quiz
{
    public class CreateDiscussionCommand : ICommandAsync
    {
        public CreateDiscussionCommand(long userId, string text, Guid questionId, Guid discussionId)
        {
            UserId = userId;
            Text = text;
            QuestionId = questionId;
            DiscussionId = discussionId;
        }

        public long UserId { get; private set; }
        public string Text { get; private set; }
        public Guid QuestionId { get; private set; }

        public Guid DiscussionId { get; private set; }
    }
}
