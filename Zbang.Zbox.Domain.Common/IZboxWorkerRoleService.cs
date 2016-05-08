using System;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;

namespace Zbang.Zbox.Domain.Common
{
    public interface IZboxWorkerRoleService
    {
        Task DeleteBoxAsync(DeleteBoxCommand command);
        void Statistics(UpdateStatisticsCommand command);
        void UpdateQuota(UpdateQuotaCommand updateQuotaCommand);
        void UpdateReputation(UpdateReputationCommand command);
        Task AddNewUpdateAsync(AddNewUpdatesCommand command);
       
        //void AddCategories(AddCategoriesCommand command);
        //void AddProducts(AddProductsToStoreCommand command);

        //void AddBanners(AddBannersCommand command);

        //bool Dbi();
        void DeleteOldUpdates();
        void OneTimeDbi();
        void UpdateUniversityStats(DateTime dateTime);

        void UpdateThumbnailPicture(UpdateThumbnailCommand command);

        Task UpdateSearchUniversityDirtyToRegularAsync(UpdateDirtyToRegularCommand command);
        Task UpdateSearchBoxDirtyToRegularAsync(UpdateDirtyToRegularCommand command);
        Task UpdateSearchItemDirtyToRegularAsync(UpdateDirtyToRegularCommand command);
        Task UpdateSearchQuizDirtyToRegularAsync(UpdateDirtyToRegularCommand command);

        void UpdateUserFromUnsubscribe(UnsubscribeUsersFromEmailCommand command);
    }
}
