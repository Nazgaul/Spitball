using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class BecomeTutorCommandHandler : ICommandHandler<BecomeTutorCommand>
    {

        private readonly IRegularUserRepository _userRepository;
        private readonly IRepository<Tutor> _tutorRepository;

        public BecomeTutorCommandHandler(IRegularUserRepository userRepository, IRepository<Tutor> tutorRepository)
        {
            _userRepository = userRepository;
            _tutorRepository = tutorRepository;
        }

        public async Task ExecuteAsync(BecomeTutorCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);
            if (user.Tutor != null)
            {
                throw new ArgumentException("user is already a tutor");
            }
            user.BecomeTutor(message.Bio, message.Price, message.Description, message.FirstName, message.LastName);
            await _tutorRepository.AddAsync(user.Tutor, token);
            await _userRepository.UpdateAsync(user, token);
        }
    }
}