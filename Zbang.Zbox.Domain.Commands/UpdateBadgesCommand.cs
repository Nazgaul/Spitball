using Zbang.Zbox.Infrastructure.Commands;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain.Commands
{
   public  class UpdateBadgesCommand : ICommand
    {
       public UpdateBadgesCommand(long userId, BadgeType badgeType)
       {
           UserId = userId;
           BadgeType = badgeType;
       }

       public long UserId { get; private set; }
       public BadgeType BadgeType { get; private set; }

       public int Progress { get; set; }
    }
}
