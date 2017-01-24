using System.Linq;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class UpdateItemCourseTagCommandHandler:ICommandHandler<UpdateItemCourseTagCommand>
    {
        private readonly IRepository<CourseTag> m_CourseTagRepository;
        private readonly IRepository<Item> m_ItemRepository;
        private readonly IGuidIdGenerator m_GuidIdGenerator;

        public UpdateItemCourseTagCommandHandler(IRepository<CourseTag> courseTagRepository, IRepository<Item> itemRepository, IGuidIdGenerator guidIdGenerator)
        {
            m_CourseTagRepository = courseTagRepository;
            m_ItemRepository = itemRepository;
            m_GuidIdGenerator = guidIdGenerator;
        }

        public void Handle(UpdateItemCourseTagCommand message)
        {
            var item = m_ItemRepository.Load(message.ItemId);
            if (item.CourseTag != null)
            {
                return;
            }
            var courseTag = m_CourseTagRepository.Query().FirstOrDefault(
                w => w.Name == message.BoxName
                     && w.Code == message.BoxCode.NullIfWhiteSpace()
                     && w.Professor == message.BoxProfessor.NullIfWhiteSpace());
            if (courseTag == null)
            {
                var id = m_GuidIdGenerator.GetId();
                courseTag = new CourseTag(id, message.BoxName, message.BoxCode.NullIfWhiteSpace(), message.BoxProfessor.NullIfWhiteSpace());
                m_CourseTagRepository.Save(courseTag);
            }
            item.CourseTag = courseTag;
            item.ShouldMakeDirty = () => false;
            m_ItemRepository.Save(item);
        }
    }
}