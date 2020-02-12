using System;
using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class SetChildNameCommandHandler : ICommandHandler<SetChildNameCommand>
    {
        private readonly IRepository<User> _userRepository;
        public SetChildNameCommandHandler(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(SetChildNameCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);

            if (user.Extend is Parent p)
            {
                p.SetChildData(message.FirstName, message.Grade);
                await _userRepository.UpdateAsync(user, token);
            }
            else
            {
                throw new ArgumentException();
            }
            //if (user.Extend.Type == UserType.Parent)
            //{
            //    user.SetChildName(message.FirstName, message.LasttName);
            //    await _userRepository.UpdateAsync(user, token);
            //}
        }
    }
}
