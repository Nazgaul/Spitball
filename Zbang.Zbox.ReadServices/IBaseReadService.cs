using System.Threading.Tasks;
using Zbang.Zbox.ViewModel.Dto.UserDtos;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Zbox.ReadServices
{
    public interface IBaseReadService
    {
        #region BoxBasedData
        //UserPermissionPerBoxDto GetUserPermission(GetBoxQuery query);

        #endregion

        #region UserBasedData

        Task<LogInUserDto> GetUserDetailsByMembershipId(GetUserByMembershipQuery query);
        Task<LogInUserDto> GetUserDetailsByFacebookId(GetUserByFacebookQuery query);
        Task<LogInUserDto> GetUserDetailsByEmail(GetUserByEmailQuery query);





        #endregion

        long GetItemIdByBlobId(string blobId);
    }
}
