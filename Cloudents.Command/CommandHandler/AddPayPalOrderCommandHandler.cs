using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class AddPayPalOrderCommandHandler : ICommandHandler<AddPayPalOrderCommand>
    {
        private readonly IRepository<PayPal> _payPalRepository;
        public AddPayPalOrderCommandHandler(IRepository<PayPal> payPalRepository)
        {
            _payPalRepository = payPalRepository;
        }

        public async Task ExecuteAsync(AddPayPalOrderCommand message, CancellationToken token)
        {
            var payPal = new PayPal(message.Token);
            await _payPalRepository.AddAsync(payPal, token);
        }
    }
}
