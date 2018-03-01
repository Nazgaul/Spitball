using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;

namespace Cloudents.Core.CommandHandler
{
    [UsedImplicitly]
    public class UpdateMailGunCommandHandler : ICommandHandlerAsync<UpdateMailGunCommand>
    {
        private readonly IRepository<MailGunStudent> _repository;

        public UpdateMailGunCommandHandler(IRepository<MailGunStudent> repository)
        {
            _repository = repository;
        }

        public async Task HandleAsync(UpdateMailGunCommand message, CancellationToken token)
        {
            var student = await _repository.LoadAsync(message.Id, token).ConfigureAwait(false);
            student.Sent += $" {DateTime.UtcNow}";
            student.ShouldSend = false;

            await _repository.SaveAsync(student, token).ConfigureAwait(false);
        }
    }
}