using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRoleSearch.DomainProcess
{
    public class UpdateBadge : IDomainProcess
    {
        private readonly IZboxWorkerRoleService m_ZboxWriteService;
        private readonly IQueueProvider m_QueueProvider;

        public UpdateBadge(IZboxWorkerRoleService zboxWriteService, IQueueProvider queueProvider)
        {
            m_ZboxWriteService = zboxWriteService;
            m_QueueProvider = queueProvider;
        }

        public async Task<bool> ExecuteAsync(Infrastructure.Transport.DomainProcess data, CancellationToken token)
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
                return true;
                //badge = BadgeType.Register;
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
            var tasks = new List<Task>();
            if (parameters.UserIds != null)
            {
                foreach (var userId in parameters.UserIds)
                {

                    tasks.Add(DoUpdateAsync(userId, badge));
                }
            }
            if (parameters.UserId > 0)
            {
                tasks.Add(DoUpdateAsync(parameters.UserId, badge));
            }
            await Task.WhenAll(tasks);
            return true;
        }

        private Task DoUpdateAsync(long userId, BadgeType badge)
        {
            TraceLog.WriteInfo($"processing badge {badge.GetEnumDescription()} for user {userId}");
            var command = new UpdateBadgesCommand(userId, badge);
            m_ZboxWriteService.UpdateBadges(command);
            if (command.Progress == 100)
            {
                return m_QueueProvider.InsertMessageToTranactionAsync(new ReputationData(userId));
                //try
                //{
                //    //TODO: culture
                //    var proxy = await SignalrClient.GetProxyAsync();
                //    await proxy.Invoke("Badge", badge.GetEnumDescription(), parameters.UserId);
                //    //var blobName = parameters.BlobUri.Segments[parameters.BlobUri.Segments.Length - 1];
                //    //if (parameters.Users != null)
                //    //{
                //    //    await proxy.Invoke("UpdateImage", blobName, parameters.Users);
                //    //}
                //    //else
                //    //{
                //    //    TraceLog.WriteError($"users is null on {blobName}");
                //    //}
                //}
                //catch (Exception ex)
                //{
                //    TraceLog.WriteError("on signalr update image", ex);
                //}
                //}

            }
            return Infrastructure.Extensions.TaskExtensions.CompletedTask;
        }
    }
}
