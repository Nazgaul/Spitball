using Cloudents.Core.Command.Admin;
using Cloudents.Domain.Entities;
using Cloudents.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Domain.Enums;
using System.Linq;

namespace Cloudents.Core.CommandHandler.Admin
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
            answer.Item.State = ItemState.Ok;

            if (answer.Item.FlagReason.Equals("Too many down vote", StringComparison.CurrentCultureIgnoreCase))
            {
                var votes = answer.Item.Votes.ToList<Vote>();
                foreach (var vote in votes)
                {
                    answer.Item.Votes.Remove(vote);
                }
            }

            answer.Item.FlagReason = null;
            answer.Item.VoteCount = answer.Item.Votes.Count;

            await _answerRepository.UpdateAsync(answer, token);

        }
    }
}
