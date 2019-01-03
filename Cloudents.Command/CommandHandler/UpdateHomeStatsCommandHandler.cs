using Cloudents.Command.Command;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class UpdateHomeStatsCommandHandler : ICommandHandler<UpdateHomeStatsCommand>
    {
        private readonly IStatsRepository _statsRepository;

        public UpdateHomeStatsCommandHandler(IStatsRepository statsRepository)
        {
            _statsRepository = statsRepository;
        }

        public async Task ExecuteAsync(UpdateHomeStatsCommand message, CancellationToken token)
        {
            await _statsRepository.UpdateTableAsync(token);
        }

    }
}
