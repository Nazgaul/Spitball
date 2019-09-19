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
            var tutorToRemove = _tutorRepository.LoadAsync(message.Id, token);
            var googleToken = _googleTokenRepository.GetAsync(message.Id.ToString(), token);
            await Task.WhenAll(tutorToRemove, googleToken);

            await _tutorRepository.DeleteAsync(tutorToRemove.Result, token);

            if (googleToken.Result != null)
            {
                await _googleTokenRepository.DeleteAsync(googleToken.Result, token);
            }
        }
    }
}
