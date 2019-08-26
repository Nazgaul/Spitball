using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.EventHandler
{
    public class UpdateTutorReadRatingEventHandler : IEventHandler<TutorAddReviewEvent>,
        IEventHandler<EndSessionEvent>
    {
        private readonly IReadTutorRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateTutorReadRatingEventHandler(IReadTutorRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task HandleAsync(TutorAddReviewEvent eventMessage, CancellationToken token)
        {
            _repository.UpdateReadTutorRating(token);
            await _unitOfWork.CommitAsync(CancellationToken.None);
        }

        public async Task HandleAsync(EndSessionEvent eventMessage, CancellationToken token)
        {
            _repository.UpdateReadTutorRating(token);
            await _unitOfWork.CommitAsync(CancellationToken.None);
        }
    }
}
