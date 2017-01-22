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

    public class AssignTagsToItemCommandHandler : ICommandHandler<AssignTagsToItemCommand>
    {
        private readonly IRepository<Item> m_ItemRepository;
        private readonly IRepository<Tag> m_TagRepository;
        private readonly IRepository<ItemTag> m_ItemTagRepository;

        public AssignTagsToItemCommandHandler(IRepository<Item> itemRepository, IRepository<Tag> tagRepository, IRepository<ItemTag> itemTagRepository)
        {
            m_ItemRepository = itemRepository;
            m_TagRepository = tagRepository;
            m_ItemTagRepository = itemTagRepository;
        }

        public void Handle(AssignTagsToItemCommand message)
        {
            var item = m_ItemRepository.Load(message.ItemId);

            foreach (var tagName in message.Tags)
            {
                var tag = m_TagRepository.Query().FirstOrDefault(f => f.Name == tagName);
                if (tag == null)
                {
                    tag = new Tag(tagName);
                    m_TagRepository.Save(tag);
                }
                
                var itemTag = new ItemTag(tag, item);
                if (item.ItemTags.Contains(itemTag))
                {
                    continue;
                }
                m_ItemTagRepository.Save(itemTag);
                //tag.ItemTags.Add(itemTag);
                //item.ItemTags.Add(itemTag);
                //m_TagRepository.Save(tag);
            }
            //m_ItemRepository.Save(item);
            
        }
    }
}
