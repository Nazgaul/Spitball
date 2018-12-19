using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.Command.Admin;
using Cloudents.Application.Interfaces;
using Cloudents.Domain.Entities;
using Cloudents.Domain.Enums;

namespace Cloudents.Application.CommandHandler.Admin
{
    class UnFlagQuestionCommandHandler : ICommandHandler<UnFlagQuestionCommand>
    {
        private readonly IRepository<Question> _questionRepository;

        public UnFlagQuestionCommandHandler(IRepository<Question> questionRepository)
        {
            _questionRepository = questionRepository;
        }
        public async Task ExecuteAsync(UnFlagQuestionCommand message, CancellationToken token)
        {
           
            var question = await _questionRepository.LoadAsync(message.QuestionId, token);
       
            question.Item.State = ItemState.Ok;
            if (question.Item.FlagReason.Equals("Too many down vote", StringComparison.CurrentCultureIgnoreCase))
            {
                var votes = question.Item.Votes.Where(w => w.Answer == null).ToList<Vote>();
                foreach (var vote in votes)
                {
                    question.Item.Votes.Remove(vote);
                }
            }
            question.Item.FlagReason = null;
            question.Item.VoteCount = question.Item.Votes.Count;
            
            await _questionRepository.UpdateAsync(question, token);
        }
    }
}
