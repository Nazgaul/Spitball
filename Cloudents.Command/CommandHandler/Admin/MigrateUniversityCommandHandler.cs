using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class MigrateUniversityCommandHandler : ICommandHandler<MigrateUniversityCommand>
    {
        private readonly IUniversityRepository _universityRepository;

        public MigrateUniversityCommandHandler(IUniversityRepository universityRepository)
        {
            _universityRepository = universityRepository;
        }

        public async Task ExecuteAsync(MigrateUniversityCommand message, CancellationToken token)
        {

            await _universityRepository.MigrateUniversityAsync(message.UniversityToRemove, message.UniversityToKeep, token);
        }
    }
}
