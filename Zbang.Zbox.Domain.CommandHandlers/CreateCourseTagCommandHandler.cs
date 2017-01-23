using System.Linq;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class CreateCourseTagCommandHandler : ICommandHandler<CreateCourseTagCommand>
    {
        private readonly IGuidIdGenerator m_GuidIdGenerator;
        private readonly IRepository<CourseTag> m_CourseTagRepository;

        

        public CreateCourseTagCommandHandler(IGuidIdGenerator guidIdGenerator, IRepository<CourseTag> courseTagRepository)
        {
            m_GuidIdGenerator = guidIdGenerator;
            m_CourseTagRepository = courseTagRepository;
        }

        public void Handle(CreateCourseTagCommand message)
        {
            var id = m_GuidIdGenerator.GetId();



            var course = m_CourseTagRepository.Query().FirstOrDefault(
                 w => w.Name == message.Name
                 && w.Code == message.Code.NullIfWhiteSpace()
                 && w.Professor == message.Professor.NullIfWhiteSpace());
            if (course != null)
            {
                return;
            }

            var tag = new CourseTag(id, message.Name, message.Code, message.Professor);
            m_CourseTagRepository.Save(tag);
        }
    }
}
