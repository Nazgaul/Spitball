using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class RenameUniversityCommandHandler : ICommandHandler<RenameUniversityCommand>
    {
        private readonly IRepository<University> _universityRepository;
        public RenameUniversityCommandHandler(IRepository<University> universityRepository)
        {
            _universityRepository = universityRepository;
        }

        public async Task ExecuteAsync(RenameUniversityCommand message, CancellationToken token)
        {
            var university = await _universityRepository.LoadAsync(message.UniversityId, token);
            university.Rename(message.NewName);
            university.Approve();
        }
    }
}
