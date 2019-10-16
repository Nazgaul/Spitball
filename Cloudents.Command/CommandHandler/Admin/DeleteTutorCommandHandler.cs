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

        public DeleteTutorCommandHandler(ITutorRepository tutorRepository,
            IRepository<GoogleTokens> googleTokenRepository)
        {
            _tutorRepository = tutorRepository;
            _googleTokenRepository = googleTokenRepository;
        }

        public async Task ExecuteAsync(DeleteTutorCommand message, CancellationToken token)
        {
            var tutorToRemove = await _tutorRepository.LoadAsync(message.Id, token);
            var googleToken = await _googleTokenRepository.GetAsync(message.Id.ToString(), token);

            if (googleToken != null)
            {
                await _googleTokenRepository.DeleteAsync(googleToken, token);
            }
            await _tutorRepository.DeleteAsync(tutorToRemove, token);

        }
    }
}
