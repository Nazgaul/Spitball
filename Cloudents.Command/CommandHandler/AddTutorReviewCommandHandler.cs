using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Exceptions;
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
            var userTutor = await _regularUserRepository.LoadAsync(message.TutorId, token);
            if (userTutor.Tutor == null)
            {
                throw new EmptyResultException();
            }
            if (userTutor.Tutor.Reviews.Any(w => w.User.Id == message.UserId))
            {
                throw new DuplicateRowException();
            }
            
            if (userTutor.Tutor != null)
            {
                var user =  await _regularUserRepository.LoadAsync(message.UserId, token);
                userTutor.Tutor.AddReview(message.Review, message.Rate, user);
                //await _repository.AddAsync(userTutor, token);
            }
        }
    }
}
