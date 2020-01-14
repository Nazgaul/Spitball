using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class UpdateTutorHoursCommandHandler : ICommandHandler<UpdateTutorHoursCommand>
    {
        private readonly IRepository<Tutor> _tutorRepository;
        public UpdateTutorHoursCommandHandler(IRepository<Tutor> tutorRepository)
        {
            _tutorRepository = tutorRepository;
        }

        public async Task ExecuteAsync(UpdateTutorHoursCommand message, CancellationToken token)
        {
            var tutor = await _tutorRepository.GetAsync(message.UserId, token);
            if (tutor == null) throw new ArgumentNullException(nameof(tutor));

            tutor.UpdateTutorHours(message.TutorDailyHours.Select(s => new TutorHours(tutor, s.Day, s.From, s.To)));
        
            await _tutorRepository.UpdateAsync(tutor, token);
        }
    }
}
