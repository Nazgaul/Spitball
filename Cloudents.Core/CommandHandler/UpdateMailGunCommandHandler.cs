using System;
using System.Globalization;
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
            var student = await _repository.GetAsync(message.Id, token).ConfigureAwait(false);
            student.Sent += $" {ConvertDateTimeToString(DateTime.UtcNow)}";
            student.ShouldSend = false;

            await _repository.AddAsync(student, token).ConfigureAwait(false);
        }

        private static string ConvertDateTimeToString(DateTime dateTime)
        {
           return  string.Format(CultureInfo.CreateSpecificCulture("en-US"), "{0:MMM} {0,2:%d} {0:yyyy} {0,2:%h}:{0:mmtt}", dateTime);
        }
    }
}