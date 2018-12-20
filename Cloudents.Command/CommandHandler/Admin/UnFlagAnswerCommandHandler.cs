using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
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
            answer.Item.State = ItemState.Ok;

            if (answer.Item.FlagReason.Equals("Too many down vote", StringComparison.CurrentCultureIgnoreCase))
            {
                var votes = answer.Item.Votes.ToList();
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
