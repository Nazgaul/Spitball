using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public interface IGamificationRepository : IRepository<Badge>
    {
        int GetUserReputation(long userId);
        int CalculateReputation(long userId);
        Badge GetBadgeOfUser(long userId, BadgeType type);
    }
}