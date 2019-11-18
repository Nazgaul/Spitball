using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class DeleteQuestionCommandHandler : ICommandHandler<DeleteQuestionCommand>
    {
        private readonly IRepository<Question> _questionRepository;



        public DeleteQuestionCommandHandler(IRepository<Question> questionRepository)
        {
            _questionRepository = questionRepository;
        }

        public async Task ExecuteAsync(DeleteQuestionCommand message, CancellationToken token)
        {
            var question = await _questionRepository.LoadAsync(message.QuestionId, token);
            if (!(question.User.Actual is User))
            {
                return;
            }
            question.DeleteQuestionAdmin();
            //if (question.CorrectAnswer == null)
            //{ 
            //    t.MakeTransaction(QuestionTransaction.Deleted(question));
            //    await _userRepository.UpdateAsync(t, token);
            //}

            await _questionRepository.DeleteAsync(question, token);
        }
    }
}