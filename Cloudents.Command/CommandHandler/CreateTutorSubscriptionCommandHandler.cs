using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler
{
    public class CreateTutorSubscriptionCommandHandler : ICommandHandler<CreateTutorSubscriptionCommand>
    {
        private readonly ITutorRepository _tutorRepository;
        private readonly IStripeService _stripeService;

        public CreateTutorSubscriptionCommandHandler(ITutorRepository tutorRepository, IStripeService stripeService)
        {
            _tutorRepository = tutorRepository;
            _stripeService = stripeService;
        }

        public async Task ExecuteAsync(CreateTutorSubscriptionCommand message, CancellationToken token)
        {
            var tutor = await _tutorRepository.LoadAsync(message.TutorId, token);
            tutor.ChangeSubscriptionPrice(message.Price);

            await _stripeService.CreateProductAsync(tutor, token);
        }
    }
}