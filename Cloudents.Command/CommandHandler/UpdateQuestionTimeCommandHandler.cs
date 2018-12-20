using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.Command;
using Cloudents.Application.Extension;
using Cloudents.Application.Interfaces;

namespace Cloudents.Application.CommandHandler
{
    public class UpdateQuestionTimeCommandHandler : ICommandHandler<UpdateQuestionTimeCommand>
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly Random _randomGenerator = new Random();

        public UpdateQuestionTimeCommandHandler(IQuestionRepository questionRepository)
        {
            _questionRepository = questionRepository;
        }

        public async Task ExecuteAsync(UpdateQuestionTimeCommand message, CancellationToken token)
        {
            foreach (var question in await _questionRepository.GetOldQuestionsAsync(token))
            {
                question.Updated = DateTimeHelpers.NextRandomDate(1, _randomGenerator);
                await _questionRepository.UpdateAsync(question, token);
            }
        }
    }
}