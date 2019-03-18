using Cloudents.Command.Command.Admin;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class ApproveUniversityCommandHandler : ICommandHandler<ApproveUniversityCommand>
    {
        private readonly IUniversityRepository _universityRepository;
        public ApproveUniversityCommandHandler(IUniversityRepository universityRepository)
        {
            _universityRepository = universityRepository;
        }

        public async Task ExecuteAsync(ApproveUniversityCommand message, CancellationToken token)
        {
            var university = await _universityRepository.LoadAsync(message.Id, token);
            university.Approve();
        }
    }
}
