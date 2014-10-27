using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ReadServices
{
    public interface IZboxReadSecurityReadService
    {
        UserRelationshipType GetUserStatusToBox(long boxId, long userId);
    }
}
