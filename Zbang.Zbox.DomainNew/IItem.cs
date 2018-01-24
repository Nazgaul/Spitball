using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain
{
    public interface ITag //: IDirty
    {
        Task AddTagAsync(Tag tag, TagType type);
        void RemoveTag(string tag);
    }

    public interface ILanguage
    {
        Language Language { get; set; }
    }
}