using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler
{
    public class CreateOfflineSessionPaymentCommandHandler : ICommandHandler<CreateOfflineSessionPaymentCommand>
    {
        private readonly ITutorRepository _tutorRepository;
        private readonly IRegularUserRepository _userRepository;
        private readonly IRepository<StudyRoomPayment> _studyRoomPaymentRepository;

        public CreateOfflineSessionPaymentCommandHandler(ITutorRepository tutorRepository, IRegularUserRepository userRepository, IRepository<StudyRoomPayment> studyRoomPaymentRepository)
        {
            _tutorRepository = tutorRepository;
            _userRepository = userRepository;
            _studyRoomPaymentRepository = studyRoomPaymentRepository;
        }

        public async Task ExecuteAsync(CreateOfflineSessionPaymentCommand message, CancellationToken token)
        {

            var tutor = await _tutorRepository.LoadAsync(message.TutorId, token);
            var user = await _userRepository.LoadAsync(message.UserId, token);
            if (user.PaymentExists != PaymentStatus.Done)
            {
                throw new ArgumentException("user need credit card");
            }

            if (tutor.User.SbCountry != user.SbCountry)
            {
                throw new ArgumentException("user need to be in the same country");
            }
            var sessionPayment = new StudyRoomPayment(tutor, user, message.Duration, message.Price);
            await _studyRoomPaymentRepository.AddAsync(sessionPayment,token);
        }
    }
}