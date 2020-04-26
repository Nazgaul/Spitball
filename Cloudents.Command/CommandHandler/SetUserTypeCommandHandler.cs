//using Cloudents.Command.Command;
//using Cloudents.Core.Entities;
//using Cloudents.Core.Interfaces;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Cloudents.Command.CommandHandler
//{
//    public class SetUserTypeCommandHandler : ICommandHandler<SetUserTypeCommand>
//    {
//        private readonly IRepository<User> _userRepository;
//        public SetUserTypeCommandHandler(IRepository<User> userRepository)
//        {
//            _userRepository = userRepository;
//        }

//        public async Task ExecuteAsync(SetUserTypeCommand message, CancellationToken token)
//        {
//            var user = await _userRepository.LoadAsync(message.UserId, token);
//            user.SetUserType(message.UserType);
//            await _userRepository.UpdateAsync(user, token);
//        }
//    }
//}
