using System;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ReadServices
{
    public interface IZboxReadSecurityReadService
    {
        UserRelationshipType GetUserStatusToBox(long boxId, long userId, Guid? inviteId);
        Task<UserRelationshipType> GetUserStatusToBoxAsync(long boxId, long userId);
    }
}
