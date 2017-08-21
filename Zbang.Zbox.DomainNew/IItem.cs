using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Domain
{
    public interface ITag //: IDirty
    {
        Task AddTagAsync(Tag tag, TagType type, IJaredPushNotification jaredPush);
        void RemoveTag(string tag);
    }

    public interface ILanguage
    {
        Language Language { get; set; }
    }
}