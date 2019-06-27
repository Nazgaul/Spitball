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
    class ChangeLeadStatusCommandHandler : ICommandHandler<ChangeLeadStatusCommand>
    {
        private readonly IRepository<Lead> _leadRepository;

        public ChangeLeadStatusCommandHandler(IRepository<Lead> leadRepository)
        {
            _leadRepository = leadRepository;
        }

        public async Task ExecuteAsync(ChangeLeadStatusCommand message, CancellationToken token)
        {
            var lead = await _leadRepository.LoadAsync(message.LeadId, token);
            lead.ChangeState(message.State);
        }
    }
}
