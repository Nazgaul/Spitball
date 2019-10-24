using Cloudents.Command.Command.Admin;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class DeleteUniversityCommandHandler : ICommandHandler<DeleteUniversityCommand>
    {
        private readonly IUniversityRepository _universityRepository;
        public DeleteUniversityCommandHandler(IUniversityRepository universityRepository)
        {
            _universityRepository = universityRepository;
        }

        public async Task ExecuteAsync(DeleteUniversityCommand message, CancellationToken token)
        {
            await _universityRepository.DeleteAsync(message.Id, token);
        }
    }
}
