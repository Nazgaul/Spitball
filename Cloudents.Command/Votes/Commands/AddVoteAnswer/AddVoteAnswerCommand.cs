using System;
using Cloudents.Application.Interfaces;
using Cloudents.Domain.Entities;

namespace Cloudents.Application.Votes.Commands.AddVoteAnswer
{
    public class AddVoteAnswerCommand : ICommand
    {
        public AddVoteAnswerCommand(long userId, Guid answerId, VoteType voteType)
        {
            UserId = userId;
            AnswerId = answerId;
            VoteType = voteType;
        }

        public long UserId { get; }
        public Guid AnswerId { get; }

        public VoteType VoteType { get; }
    }
}