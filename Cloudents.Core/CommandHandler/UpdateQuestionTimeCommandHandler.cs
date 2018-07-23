using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.CommandHandler
{
    public class UpdateQuestionTimeCommandHandler : ICommandHandler<UpdateQuestionTimeCommand>
    {
        private readonly IQuestionRepository _questionRepository;
        readonly Random _randomGenerator = new Random();

        public UpdateQuestionTimeCommandHandler(IQuestionRepository questionRepository)
        {
            _questionRepository = questionRepository;
        }

        public async Task ExecuteAsync(UpdateQuestionTimeCommand message, CancellationToken token)
        {
            foreach (var question in await _questionRepository.GetOldQuestionsAsync(token))
            {
                question.Updated =  NextRandomDate();
                await _questionRepository.UpdateAsync(question, token);
            }

          ;
        }


        private DateTime NextRandomDate()
        {
            var range = 2;
            var start = DateTime.UtcNow.AddDays(-range);

            return start.AddDays(_randomGenerator.Next(range)).AddHours(_randomGenerator.Next(0, 24)).AddMinutes(_randomGenerator.Next(0, 60)).AddSeconds(_randomGenerator.Next(0, 60));
        }
    }
}