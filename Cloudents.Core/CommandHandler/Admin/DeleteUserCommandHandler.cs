using Cloudents.Core.Command.Admin;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
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
        public async Task ExecuteAsync(DeleteUserCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.Id, false, token);
            string[] BreakEmail = user.Email.Split(new[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
            Random random = new Random();
            var email = $"{BreakEmail[0]}@demi{random.Next(1, 9999)}.com";
           
            user.Email = email;
            user.NormalizedEmail = email.ToUpper();
            user.PhoneNumber = null;
            user.Fictive = true;
            
            await _userRepository.UpdateAsync(user, token);
            
        }
    }
}
