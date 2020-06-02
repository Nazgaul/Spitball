using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler
{
    public class AddStripeCustomerCommandHandler : ICommandHandler<AddStripeCustomerCommand>
    {
        private readonly IStripeService _stripeService;
        private readonly IRegularUserRepository _userRepository;

        public AddStripeCustomerCommandHandler(IStripeService stripeService, IRegularUserRepository userRepository)
        {
            _stripeService = stripeService;
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(AddStripeCustomerCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);
            var customerId = await _stripeService.CreateCustomerAsync(user, token);

            var payment = new StripePayment(customerId);
            user.AddPayment(payment);
            var futureCardPayment = await _stripeService.FutureCardPaymentsAsync(customerId);

            message.ClientSecretId = futureCardPayment;

        }
    }
}