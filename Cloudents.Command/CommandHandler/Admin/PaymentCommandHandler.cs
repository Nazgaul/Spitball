using Cloudents.Command.Command.Admin;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class PaymentCommandHandler : ICommandHandler<PaymentCommand>
    {
        private readonly IPayment _payment;
        public PaymentCommandHandler(IPayment payment)
        {
            _payment = payment;
        }

        public async Task ExecuteAsync(PaymentCommand message, CancellationToken token)
        {
            await _payment.TransferPaymentAsync(message.TutorKey, message.UserKey, message.Anount, token);
        }
    }
}
