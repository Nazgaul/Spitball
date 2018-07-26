using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Management.Command;

namespace Cloudents.Management.CommandHandler
{
    public class DeleteQuestionCommandHandler : ICommandHandler<DeleteQuestionCommand>
    {
        private readonly IRepository<Question> _questionRepository;
        private readonly IUserRepository _userRepository;

        private readonly IUnitOfWork _unitOfWork;

        public DeleteQuestionCommandHandler(IRepository<Question> questionRepository, IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _questionRepository = questionRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task ExecuteAsync(DeleteQuestionCommand message, CancellationToken token)
        {
            var question = await _questionRepository.LoadAsync(message.QuestionId, token);
            var userId = question.User.Id;
            await _questionRepository.DeleteAsync(question, token);
            await _questionRepository.FlushAsync(token); //TODO: this is not right


            //await _unitOfWork.CommitAsync(token);
            var user = await _userRepository.LoadAsync(userId, token);

            user.Balance = await _userRepository.UserBalanceAsync(userId, token);

            await _unitOfWork.CommitAsync(token);


        }
    }

    //public class UpdateUserBalanceCommandHandler : ICommandHandler<UpdateUserBalanceCommand>
    //{
    //    public Task ExecuteAsync(UpdateUserBalanceCommand message, CancellationToken token)
    //    {
    //        throw new System.NotImplementedException();
    //    }
    //}
}