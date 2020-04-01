using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class PaymentCommandHandler : ICommandHandler<PaymentCommand>
    {
        private readonly IPayment _payment;
        private readonly IRepository<StudyRoomSession> _studyRoomSessionRepository;
        private readonly ITutorRepository _tutorRepository;
        private readonly IRegularUserRepository _userRepository;

        public PaymentCommandHandler(IPayment payment,
            IRepository<StudyRoomSession> studyRoomSessionRepository, ITutorRepository tutorRepository,
            IRegularUserRepository userRepository)
        {
            _payment = payment;
            _studyRoomSessionRepository = studyRoomSessionRepository;
            _tutorRepository = tutorRepository;
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(PaymentCommand message, CancellationToken token)
        {
            var session = await _studyRoomSessionRepository.LoadAsync(message.StudyRoomSessionId, token);
            var tutor = await _tutorRepository.LoadAsync(message.TutorId, token);
            var user = await _userRepository.LoadAsync(message.UserId, token);

            var receipt = $"Payed in {DateTime.UtcNow}";
            if (message.StudentPay != 0)
            {
                var response = await _payment.TransferPaymentAsync(tutor.SellerKey!,
                    user.BuyerPayment!.PaymentKey!, message.StudentPay, token);
                receipt = response.PaymeSaleId;
            }

            if (message.SpitballPay != 0)
            {
                await _payment.TransferPaymentAsync(tutor.SellerKey!,
                    message.SpitballBuyerKey, message.SpitballPay, token);
            }

            if (session.StudyRoomVersion.GetValueOrDefault() == 0)
            {
                session.SetReceiptAndAdminDate(receipt, message.AdminDuration);
            }
            else
            {
                var sessionUser = session.RoomSessionUsers.AsQueryable().Single(s => s.User.Id == message.UserId);
                sessionUser.Pay(receipt,message.AdminDuration);
            }

            user.UseCoupon(tutor);

            await _studyRoomSessionRepository.UpdateAsync(session, token);
        }
    }
}
