﻿using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.ViewModel.Dto.UserDtos;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Zbox.ReadServices
{
    public interface IBaseReadService
    {
        #region UserBasedData

        Task<LogInUserDto> GetUserDetailsByMembershipId(GetUserByMembershipQuery query);
        Task<LogInUserDto> GetUserDetailsByFacebookId(GetUserByFacebookQuery query);
        Task<LogInUserDto> GetUserDetailsByGoogleIdAsync(GetUserByGoogleQuery query, CancellationToken cancellationToken);
        Task<LogInUserDto> GetUserDetailsByEmail(GetUserByEmailQuery query);

        Task<LogInUserDto> GetUserDetailsById(GetUserByIdQuery query);


        Task<ForgotPasswordDto> GetForgotPasswordByEmailAsync(GetUserByEmailQuery query, CancellationToken token);

        #endregion

        //long GetItemIdByBlobId(string blobId);
    }
}
