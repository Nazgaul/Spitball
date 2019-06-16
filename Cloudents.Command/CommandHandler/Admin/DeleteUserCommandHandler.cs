//using System.Threading;
//using System.Threading.Tasks;
//using Cloudents.Command.Command.Admin;

//namespace Cloudents.Command.CommandHandler.Admin
//{
//    public class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand>
//    {

//        //public DeleteUserCommandHandler(IUserRepository userRepository)
//        //{
//        //    _userRepository = userRepository;
//        //}
//        public  Task ExecuteAsync(DeleteUserCommand message, CancellationToken token)
//        {
//            return Task.CompletedTask;
            
//            //var user = await _userRepository.LoadAsync(message.Id, false, token);
//            //string[] BreakEmail = user.Email.Split(new[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
//            //Random random = new Random();
//            //var email = $"{BreakEmail[0]}@demi{random.Next(1, 9999)}.com";

//            //user.Email = email;
//            //user.NormalizedEmail = email.ToUpper();
//            //user.PhoneNumber = null;
//            //user.Fictive = true;
//            //user.UserLogins.Clear();

//            //await _userRepository.UpdateAsync(user, token);

//        }
//    }
//}
