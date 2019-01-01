﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class UnFlagAnswerCommandHandler : ICommandHandler<UnFlagAnswerCommand>
    {
        private readonly IRepository<Answer> _answerRepository;

        public UnFlagAnswerCommandHandler(IRepository<Answer> answerRepository)
        {
            _answerRepository = answerRepository;
        }
        public async Task ExecuteAsync(UnFlagAnswerCommand message, CancellationToken token)
        {
        
            var answer = await _answerRepository.LoadAsync(message.AnswerId, token);
           
            if (answer.FlagReason.Equals("Too many down vote", StringComparison.CurrentCultureIgnoreCase))
            {
                answer.Votes.Clear();
            }
            answer.MakePublic();
            answer.VoteCount = answer.Votes.Count;

            await _answerRepository.UpdateAsync(answer, token);

        }
    }
}
