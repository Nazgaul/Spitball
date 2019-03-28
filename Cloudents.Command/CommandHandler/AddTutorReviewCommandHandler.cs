using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class AddTutorReviewCommandHandler : ICommandHandler<AddTutorReviewCommand>
    {
        private readonly IRegularUserRepository _regularUserRepository;
        private readonly IRepository<TutorReview> _repository;

        public AddTutorReviewCommandHandler(IRepository<TutorReview> repository, IRegularUserRepository regularUserRepository)
        {
            _repository = repository;
            _regularUserRepository = regularUserRepository;
        }

        public async Task ExecuteAsync(AddTutorReviewCommand message, CancellationToken token)
        {
            var user = await _regularUserRepository.GetAsync(message.Tutor, token);
            //Tutor tutor = (Tutor)user.UserRoles.FirstOrDefault(f => f is Tutor);
            if (user.Tutor != null)
            {
                var review = new TutorReview(message.Review, message.Rate, message.User, user);
                await _repository.AddAsync(review, token);
            }
        }
    }
}
