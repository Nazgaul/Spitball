using System;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRoleSearch.DomainProcess
{
    public class UpdateBadge : IDomainProcess
    {
        private readonly IZboxWorkerRoleService m_ZboxWriteService;

        public UpdateBadge(IZboxWorkerRoleService zboxWriteService)
        {
            m_ZboxWriteService = zboxWriteService;
        }

        public Task<bool> ExecuteAsync(Infrastructure.Transport.DomainProcess data, CancellationToken token)
        {
            var parameters = data as BadgeData;
            if (parameters == null)
            {
                throw new NullReferenceException("parameters");
            }
            //TODO change that
            var badge = BadgeType.None;
            if (parameters is RegisterBadgeData)
            {
                badge = BadgeType.Register;
            }
            if (parameters is FollowClassBadgeData)
            {
                badge = BadgeType.FollowClass;
            }
            if (parameters is CreateQuizzesBadgeData)
            {
                badge = BadgeType.CreateQuizzes;
            }
            if (parameters is UploadItemsBadgeData)
            {
                badge = BadgeType.UploadFiles;
            }
            if (parameters is LikesBadgeData)
            {
                badge = BadgeType.Likes;
            }
            var command = new UpdateBadgesCommand(parameters.UserId, badge);
            m_ZboxWriteService.UpdateBadges(command);
            return Infrastructure.Extensions.TaskExtensions.CompletedTaskTrue;
        }
    }
}
