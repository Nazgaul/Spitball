using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class PaymentCommandHandler : ICommandHandler<PaymentCommand>
    {
        private readonly IPayment _payment;
        private readonly IRepository<StudyRoomSession> _studyRoomSessionRepository;
        public PaymentCommandHandler(IPayment payment,
            IRepository<StudyRoomSession> studyRoomSessionRepository)
        {
            _payment = payment;
            _studyRoomSessionRepository = studyRoomSessionRepository;
        }

        public async Task ExecuteAsync(PaymentCommand message, CancellationToken token)
        {
            var session = await _studyRoomSessionRepository.LoadAsync(message.StudyRoomSessionId, token);

            var response = await _payment.TransferPaymentAsync(message.TutorKey, 
                message.UserKey, message.Amount, token);

            session.SetReceipt(response.PaymeSaleId);
        }
    }
}
