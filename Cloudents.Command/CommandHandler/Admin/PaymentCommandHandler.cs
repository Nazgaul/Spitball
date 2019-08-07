﻿using Cloudents.Command.Command.Admin;
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
        private readonly ITutorRepository _tutorRepository;
        private readonly IRegularUserRepository _userRepository;
        public PaymentCommandHandler(IPayment payment,
            IRepository<StudyRoomSession> studyRoomSessionRepository, ITutorRepository tutorRepository, IRegularUserRepository userRepository)
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

            var response = await _payment.TransferPaymentAsync(tutor.SellerKey, 
                user.BuyerPayment.PaymentKey, message.StudentPay, token);
            await _payment.TransferPaymentAsync(tutor.SellerKey,
                "MPL15627-63639KKV-JHFYQUQ0-EUEGLVFI", message.SpitballPay, token);

            session.SetReceipt(response.PaymeSaleId);
            await _studyRoomSessionRepository.UpdateAsync(session, token);
        }
    }

    public class DeclinePaymentCommandHandler: ICommandHandler<DeclinePaymentCommand>
    {
        private readonly IRepository<StudyRoomSession> _studyRoomSessionRepository;

        public DeclinePaymentCommandHandler(IRepository<StudyRoomSession> studyRoomSessionRepository)
        {
            _studyRoomSessionRepository = studyRoomSessionRepository;
        }

        public async Task ExecuteAsync(DeclinePaymentCommand message, CancellationToken token)
        {
            var session = await _studyRoomSessionRepository.LoadAsync(message.StudyRoomSessionId, token);
            session.SetReceipt("No Pay");
            await _studyRoomSessionRepository.UpdateAsync(session, token);
        }
    }
}
