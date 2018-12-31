﻿using Cloudents.Core.Entities;

namespace Cloudents.Command.Votes.Commands.AddVoteQuestion
{
    public class AddVoteQuestionCommand : ICommand
    {
        public AddVoteQuestionCommand(long userId, long questionId, VoteType voteType)
        {
            UserId = userId;
            QuestionId = questionId;
            VoteType = voteType;
        }

        public long UserId { get; }
        public long QuestionId { get; }

        public VoteType VoteType { get; }
    }
}