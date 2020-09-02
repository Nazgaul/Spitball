using System;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class DeleteTutorCommandHandler : ICommandHandler<DeleteTutorCommand>
    {
        private readonly ITutorRepository _tutorRepository;
        private readonly IRepository<GoogleTokens> _googleTokenRepository;
        private readonly IRepository<AdminUser> _adminUserRepository;

        public DeleteTutorCommandHandler(ITutorRepository tutorRepository,
            IRepository<GoogleTokens> googleTokenRepository, IRepository<AdminUser> adminUserRepository)
        {
            _tutorRepository = tutorRepository;
            _googleTokenRepository = googleTokenRepository;
            _adminUserRepository = adminUserRepository;
        }

        public async Task ExecuteAsync(DeleteTutorCommand message, CancellationToken token)
        {
            var tutorToRemove = await _tutorRepository.GetAsync(message.Id, token);
            if (tutorToRemove == null)
            {
                return;
            }
            var adminUser = await _adminUserRepository.LoadAsync(message.UserId, token);
            if (adminUser.SbCountry != null && tutorToRemove.User.SbCountry != adminUser.SbCountry)
            {
                throw new ArgumentException();
            }

            var googleToken = await _googleTokenRepository.GetAsync(message.Id.ToString(), token);

            if (googleToken != null)
            {
                await _googleTokenRepository.DeleteAsync(googleToken, token);
            }
            await _tutorRepository.DeleteAsync(tutorToRemove, token);

        }
    }
}
