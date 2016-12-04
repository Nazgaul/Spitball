using System;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;

namespace Zbang.Zbox.Domain.Common
{
    public interface IZboxWorkerRoleService
    {
        Task DeleteBoxAsync(DeleteBoxCommand command);
        Task StatisticsAsync(UpdateStatisticsCommand command);
        void UpdateBadges(UpdateBadgesCommand command);
        void UpdateQuota(UpdateQuotaCommand updateQuotaCommand);
        void UpdateReputation(UpdateReputationCommand command);
        Task AddNewUpdateAsync(AddNewUpdatesCommand command);

        Task<int> DeleteOldUpdatesAsync(CancellationToken token);
        Task<int> DeleteOldItemAsync(CancellationToken token);
        Task<int> DeleteOldBoxAsync(CancellationToken token);
        Task<int> DeleteOldQuizAsync(CancellationToken token);
        Task<int> DeleteOldUniversityAsync(CancellationToken token);
        Task<int> DoDirtyUpdateAsync(CancellationToken token);


        void OneTimeDbi();
        void UpdateUniversityStats(DateTime dateTime);

        void UpdateThumbnailPicture(UpdateThumbnailCommand command);

        Task UpdateSearchUniversityDirtyToRegularAsync(UpdateDirtyToRegularCommand command);
        Task UpdateSearchBoxDirtyToRegularAsync(UpdateDirtyToRegularCommand command);
        Task UpdateSearchItemDirtyToRegularAsync(UpdateDirtyToRegularCommand command);
        Task UpdateSearchQuizDirtyToRegularAsync(UpdateDirtyToRegularCommand command);

        void UpdateUserFromUnsubscribe(UnsubscribeUsersFromEmailCommand command);


        void ChangeBoxDepartment(ChangeBoxLibraryCommand command);


        Task UpdateSpamGunSendAsync(int id, CancellationToken token);

        void UpdateItemUrl();
    }
}
