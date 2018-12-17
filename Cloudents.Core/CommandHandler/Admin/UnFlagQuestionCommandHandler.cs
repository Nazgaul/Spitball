using Cloudents.Core.Command.Admin;
using Cloudents.Domain.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Domain.Enums;

namespace Cloudents.Core.CommandHandler.Admin
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
                question.Item.FlagReason = null;

                await _questionRepository.UpdateAsync(question, token);
   
        }
    }
}
