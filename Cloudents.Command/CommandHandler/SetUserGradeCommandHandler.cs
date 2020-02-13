using System;
using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class SetUserGradeCommandHandler : ICommandHandler<SetUserGradeCommand>
    {
        private readonly IRepository<User> _userRepository;
        public SetUserGradeCommandHandler(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(SetUserGradeCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);
            if (user.Extend is HighSchoolStudent p)
            {
                p.SetGrade(message.Grade);
                await _userRepository.UpdateAsync(user, token);
            }
            else
            {
                throw new ArgumentException();
            }
            //if (user.UserType != UserType.University)
            //{
            //    user.SetUserGrade(message.Grade);
            //    await _userRepository.UpdateAsync(user, token);
            //}
        }
    }
}