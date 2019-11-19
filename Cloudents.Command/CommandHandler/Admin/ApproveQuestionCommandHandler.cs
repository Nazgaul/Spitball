using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class ApproveQuestionCommandHandler : ICommandHandler<ApproveQuestionCommand>
    {
        private readonly IRepository<Question> _questionRepository;


        public ApproveQuestionCommandHandler(IRepository<Question> questionRepository)
        {
            _questionRepository = questionRepository;
        }

        public async Task ExecuteAsync(ApproveQuestionCommand message, CancellationToken token)
        {
            foreach (var questionId in message.QuestionIds)
            {
                var question = await _questionRepository.LoadAsync(questionId, token);
                question.MakePublic();
                await _questionRepository.UpdateAsync(question, token);
            }

        }
    }
}