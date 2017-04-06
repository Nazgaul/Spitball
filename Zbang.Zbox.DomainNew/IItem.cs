using Zbang.Zbox.Infrastructure.Culture;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain
{
    public interface ITag //: IDirty
    {
        void AddTag(Tag tag, TagType type);
        void RemoveTag(string tag);
    }

    public interface ILanguage
    {
        Language Language { get; set; }
    }
}