using Zbang.Zbox.ViewModel.Queries;
using User = Zbang.Zbox.ViewModel.DTOs.UserDtos;

namespace Zbang.Zbox.ReadServices
{
    public interface IBaseReadService
    {
        #region BoxBasedData
        //UserPermissionPerBoxDto GetUserPermission(GetBoxQuery query);

        #endregion

        #region UserBasedData

        User.LogInUserDto GetUserDetailsByMembershipId(GetUserByMembershipQuery query);
        User.LogInUserDto GetUserDetailsByFacebookId(GetUserByFacebookQuery query);
        User.LogInUserDto GetUserDetailsByEmail(GetUserByEmailQuery query);
       
        
        


        #endregion

        long GetItemIdByBlobId(string blobId);
    }
}
