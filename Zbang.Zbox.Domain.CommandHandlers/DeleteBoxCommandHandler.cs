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
        private readonly IRepository<Box> m_BoxRepository;
        private readonly IRepository<Library> m_DepartmentRepository;
        private readonly IUniversityRepository m_UniversityRepository;
        private readonly IQueueProvider m_QueueProvider;

        public DeleteBoxCommandHandler(IRepository<Box> boxRepository,
            IRepository<Library> departmentRepository,
            IUniversityRepository universityRepository, IQueueProvider queueProvider)
        {
            m_BoxRepository = boxRepository;
            m_DepartmentRepository = departmentRepository;
            m_UniversityRepository = universityRepository;
            m_QueueProvider = queueProvider;
        }

        public Task HandleAsync(DeleteBoxCommand message)
        {

            var box = m_BoxRepository.Load(message.BoxId);


            if (box.Actual is AcademicBox academicBox)
            {
                message.UniversityId = academicBox.University.Id;
                var university = academicBox.University;
                var department = academicBox.Department;
                var noOfBoxes = m_UniversityRepository.GetNumberOfBoxes(university);
                m_BoxRepository.Delete(box);

                m_DepartmentRepository.Save(department.UpdateNumberOfBoxes());
                university.UpdateNumberOfBoxes(--noOfBoxes);
                m_UniversityRepository.Save(university);
            }


            m_BoxRepository.Delete(box);
            var reputationItemUsers = box.Items.Where(w => !w.IsDeleted).Select(s => s.UploaderId);
            var reputationQuizUsers = box.Quizzes.Where(w => !w.IsDeleted).Select(s => s.User.Id);
            var reputationFlashcardUsers = box.Flashcards.Where(w => !w.IsDeleted).Select(s => s.User.Id);
            var reputationCommentUsers = box.Comments.Where(w => w.LikeCount > 0).Select(s => s.User.Id);
            var reputationReplyUsers = box.Replies.Where(w => w.LikeCount > 0).Select(s => s.User.Id);
            var t1 = m_QueueProvider.InsertMessageToTransactionAsync(new QuotaData(box.Items.Select(s => s.UploaderId)));
            var t2 = m_QueueProvider.InsertMessageToTransactionAsync(new ReputationData(
                reputationItemUsers.Union(reputationQuizUsers)
                .Union(reputationFlashcardUsers)
                .Union(reputationCommentUsers)
                .Union(reputationReplyUsers)
                ));
           // var t3 = m_QueueProvider.InsertFileMessageAsync(new BoxDeleteData(box.Id));
            return Task.WhenAll(t1, t2);
        }
    }
}
