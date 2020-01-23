using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class DeleteGoogleTokenCommandHandler : ICommandHandler<DeleteGoogleTokenCommand>
    {
        private readonly IRepository<GoogleTokens> _repository;
        private readonly ITutorHoursRepository _tutorHoursRepository;
        private readonly ITutorCalendarRepository _tutorCalendarRepository;

        public DeleteGoogleTokenCommandHandler(IRepository<GoogleTokens> repository, ITutorHoursRepository tutorHoursRepository,
            ITutorCalendarRepository tutorCalendarRepository)
        {
            _repository = repository;
            _tutorHoursRepository = tutorHoursRepository;
            _tutorCalendarRepository = tutorCalendarRepository;
        }

        public async Task ExecuteAsync(DeleteGoogleTokenCommand message, CancellationToken token)
        {
            var googleToken = await _repository.LoadAsync(message.UserId.ToString(), token);
            var tutorHours = await _tutorHoursRepository.GetTutorHoursAsync(message.UserId, token);
            var tutorCalendars = await _tutorCalendarRepository.GetTutorCalendarsAsync(message.UserId, token);

            await _repository.DeleteAsync(googleToken, token);
            foreach (var th in tutorHours)
            {
                await _tutorHoursRepository.DeleteAsync(th, token);

            }
            foreach (var tc in tutorCalendars)
            {
                await _tutorCalendarRepository.DeleteAsync(tc, token);
            }
        }
    }
}
