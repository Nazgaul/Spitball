using Cloudents.Core.Command.Admin;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.CommandHandler.Admin
{
    public class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand>
    {

   
        private readonly IUserRepository _userRepository;


        public DeleteUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }


         public Task ExecuteAsync(DeleteUserCommand message, CancellationToken token)
         {
             return Task.CompletedTask;
             /*var user = await _userRepository.GetUserByEmail(message.Email, token);
              foreach (var q in user.Questions)
                              q.CorrectAnswer = null;
 
 
              await _userRepository.DeleteAsync(user, token);*/
         }
    }
}
