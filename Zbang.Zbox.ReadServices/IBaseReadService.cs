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

        LogInUserDto GetUserDetailsByMembershipId(GetUserByMembershipQuery query);
        LogInUserDto GetUserDetailsByFacebookId(GetUserByFacebookQuery query);
        LogInUserDto GetUserDetailsByEmail(GetUserByEmailQuery query);
       
        
        


        #endregion

        long GetItemIdByBlobId(string blobId);
    }
}
