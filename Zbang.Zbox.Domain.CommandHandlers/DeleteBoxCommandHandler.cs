using System.Linq;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class DeleteBoxCommandHandler : ICommandHandlerAsync<DeleteBoxCommand>
    {
        private readonly IRepository<Box> _boxRepository;
        private readonly IRepository<Library> _departmentRepository;
        private readonly IUniversityRepository _universityRepository;
        private readonly IQueueProvider _queueProvider;

        public DeleteBoxCommandHandler(IRepository<Box> boxRepository,
            IRepository<Library> departmentRepository,
            IUniversityRepository universityRepository, IQueueProvider queueProvider)
        {
            _boxRepository = boxRepository;
            _departmentRepository = departmentRepository;
            _universityRepository = universityRepository;
            _queueProvider = queueProvider;
        }

        public Task HandleAsync(DeleteBoxCommand message)
        {
            var box = _boxRepository.Load(message.BoxId);

            if (box.Actual is AcademicBox academicBox)
            {
                message.UniversityId = academicBox.University.Id;
                var university = academicBox.University;
                var department = academicBox.Department;
                var noOfBoxes = _universityRepository.GetNumberOfBoxes(university);
                _boxRepository.Delete(box);

                _departmentRepository.Save(department.UpdateNumberOfBoxes());
                university.UpdateNumberOfBoxes(--noOfBoxes);
                _universityRepository.Save(university);
            }

            _boxRepository.Delete(box);
            var reputationItemUsers = box.Items.Where(w => !w.IsDeleted).Select(s => s.UploaderId);
            var reputationQuizUsers = box.Quizzes.Where(w => !w.IsDeleted).Select(s => s.User.Id);
            var reputationFlashcardUsers = box.Flashcards.Where(w => !w.IsDeleted).Select(s => s.User.Id);
            var reputationCommentUsers = box.Comments.Where(w => w.LikeCount > 0).Select(s => s.User.Id);
            var reputationReplyUsers = box.Replies.Where(w => w.LikeCount > 0).Select(s => s.User.Id);
            var t1 = _queueProvider.InsertMessageToTransactionAsync(new QuotaData(box.Items.Select(s => s.UploaderId)));
            var t2 = _queueProvider.InsertMessageToTransactionAsync(new ReputationData(
                reputationItemUsers.Union(reputationQuizUsers)
                .Union(reputationFlashcardUsers)
                .Union(reputationCommentUsers)
                .Union(reputationReplyUsers)
                ));
           // var t3 = _queueProvider.InsertFileMessageAsync(new BoxDeleteData(box.Id));
            return Task.WhenAll(t1, t2);
        }
    }
}
