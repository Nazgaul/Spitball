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

            var academicBox = box.Actual as AcademicBox;
            var users = box.UserBoxRelationship.Select(s => s.User.Id).ToList();


            if (academicBox != null)
            {
                var university = academicBox.University;
                var department = academicBox.Department;
                var noOfBoxes = m_UniversityRepository.GetNumberOfBoxes(university);
                m_BoxRepository.Delete(box);
                
                m_DepartmentRepository.Save(department.UpdateNumberOfBoxes());
                university.UpdateNumberOfBoxes(--noOfBoxes);
                m_UniversityRepository.Save(university);
            }


            m_BoxRepository.Delete(box);
            var t1 = m_QueueProvider.InsertMessageToTranactionAsync(new QuotaData(users));
            var t2 = m_QueueProvider.InsertMessageToTranactionAsync(new ReputationData(users));
            return Task.WhenAll(t1, t2);
        }
    }
}
