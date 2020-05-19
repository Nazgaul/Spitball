﻿using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class PaymentCommandHandler : ICommandHandler<PaymentCommand>
    {
        // private readonly IPaymeProvider _payment;
        private readonly IIndex<Type, IPaymentProvider> _payments;
        private readonly IRepository<StudyRoomSession> _studyRoomSessionRepository;
        private readonly ITutorRepository _tutorRepository;
        private readonly IRegularUserRepository _userRepository;

        public PaymentCommandHandler(
            IRepository<StudyRoomSession> studyRoomSessionRepository, ITutorRepository tutorRepository,
            IRegularUserRepository userRepository, IIndex<Type, IPaymentProvider> payments)
        {
            _studyRoomSessionRepository = studyRoomSessionRepository;
            _tutorRepository = tutorRepository;
            _userRepository = userRepository;
            _payments = payments;
        }

        public async Task ExecuteAsync(PaymentCommand message, CancellationToken token)
        {
            var session = await _studyRoomSessionRepository.LoadAsync(message.StudyRoomSessionId, token);
            var tutor = await _tutorRepository.LoadAsync(message.TutorId, token);
            var user = await _userRepository.LoadAsync(message.UserId, token);

            if (tutor.User.SbCountry != user.SbCountry)
            {
                throw new ApplicationException("We cannot charge users from different country");
            }

            var receipt = $"Payed in {DateTime.UtcNow}";
            var payment = user.Payment?.AvoidProxy;
            if (payment == null)
            {
                throw new NullReferenceException("no payment on the user");
            }
            var paymentProvider = _payments[payment.GetType()];
            if (message.StudentPay != 0)
            {
                receipt = await paymentProvider.ChargeSessionAsync(tutor, user, message.StudentPay, token);
            }

            if (message.SpitballPay != 0)
            {
                await paymentProvider.ChargeSessionBySpitballAsync(tutor, message.SpitballPay, token);
            }

            if (session.StudyRoomVersion.GetValueOrDefault() == 0)
            {
                session.SetReceiptAndAdminDate(receipt, message.AdminDuration);
            }
            else
            {
                var sessionUser = session.RoomSessionUsers.AsQueryable().Single(s => s.User.Id == message.UserId);
                sessionUser.StudyRoomPayment.Pay(receipt, message.AdminDuration, message.StudentPay + message.SpitballPay);
            }

            //user.UseCoupon(tutor);

            await _studyRoomSessionRepository.UpdateAsync(session, token);
        }
    }
}
