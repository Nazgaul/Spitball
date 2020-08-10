using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class PaymentDeclineCommandHandler : ICommandHandler<PaymentDeclineCommand>
    {
        private readonly IRepository<StudyRoomSession> _studyRoomSessionRepository;
        private readonly IRepository<StudyRoomPayment> _studyRoomPaymentRepository;

        public PaymentDeclineCommandHandler(IRepository<StudyRoomSession> studyRoomSessionRepository, IRepository<StudyRoomPayment> studyRoomPaymentRepository)
        {
            _studyRoomSessionRepository = studyRoomSessionRepository;
            _studyRoomPaymentRepository = studyRoomPaymentRepository;
        }

        public async Task ExecuteAsync(PaymentDeclineCommand message, CancellationToken token)
        {
            var payment = await _studyRoomPaymentRepository.GetAsync(message.StudyRoomSessionId, token);
            if (payment != null)
            {
                payment.NoPay();
                return;
            }
            var session = await _studyRoomSessionRepository.LoadAsync(message.StudyRoomSessionId, token);

            var sessionUser = session.RoomSessionUsers.AsQueryable().Single(s => s.User.Id == message.UserId);
            sessionUser.StudyRoomPayment.NoPay();

        }
    }
}