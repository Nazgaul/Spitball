using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler
{
    public class BecomeTutorCommandHandler : ICommandHandler<BecomeTutorCommand>
    {

        private readonly IRegularUserRepository _userRepository;

        public BecomeTutorCommandHandler(IRegularUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(BecomeTutorCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);
            if (user.Tutor != null)
            {
                throw new ArgumentException("user is already a tutor");
            }
            user.BecomeTutor(message.Bio,message.Price,message.Description,message.FirstName,message.LastName);
            //user.Tutor = new Tutor(message.Bio, user, message.Price);
            //user.Description = message.Description;
            //user.CanTeachAllCourses();
            //user.ChangeName(message.FirstName,message.LastName);
            await _userRepository.UpdateAsync(user, token);
        }
    }
}