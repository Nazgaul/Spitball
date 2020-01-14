using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class SetTutorHoursCommandHandler : ICommandHandler<SetTutorHoursCommand>
    {
        private readonly IRepository<Tutor> _tutorRepository;
        public SetTutorHoursCommandHandler(IRepository<Tutor> tutorRepository)
        {
            _tutorRepository = tutorRepository;
        }

        public async Task ExecuteAsync(SetTutorHoursCommand message, CancellationToken token)
        {
            var tutor = await _tutorRepository.LoadAsync(message.UserId, token);

            tutor.UpdateTutorHours(message.TutorDailyHoursObj);
            //foreach (var tutorDailyHours in message.TutorDailyHoursObj)
            //{
            //    tutor.AddTutorHours(tutorDailyHours.Day, tutorDailyHours.From, tutorDailyHours.To);
            //}

            await _tutorRepository.UpdateAsync(tutor, token);
        }
    }
}
