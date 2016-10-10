using System.Threading;
using System.Threading.Tasks;
using Dapper;
using System;
using System.Linq;
using Zbang.Zbox.Infrastructure.Data.Dapper;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.ViewModel.Dto;
using Zbang.Zbox.ViewModel.Dto.UserDtos;
using Zbang.Zbox.ViewModel.Queries;
//using ExtensionTransformers = Zbang.Zbox.Infrastructure.Data.Transformers;

namespace Zbang.Zbox.ReadServices
{
    public class BaseReadService : IBaseReadService, IZboxReadSecurityReadService
    {

        #region login
        public async Task<LogInUserDto> GetUserDetailsByFacebookId(GetUserByFacebookQuery query)
        {
            using (var con = await DapperConnection.OpenConnectionAsync())
            {
                var retVal = await con.QueryAsync<LogInUserDto>(ViewModel.SqlQueries.Sql.GetUserByFacebookId,
                     new { FacebookUserId = query.FacebookId });
                return retVal.FirstOrDefault();
            }
        }

        public async Task<LogInUserDto> GetUserDetailsByGoogleIdAsync(GetUserByGoogleQuery query, CancellationToken cancellationToken)
        {
            using (var con = await DapperConnection.OpenConnectionAsync(cancellationToken))
            {
                var retVal = await con.QueryAsync<LogInUserDto>(new CommandDefinition(ViewModel.SqlQueries.Sql.GetUserByGoogleId,
                    cancellationToken: cancellationToken, parameters: new { GoogleUserId = query.GoogleId }));
                return retVal.FirstOrDefault();
            }
        }

        public async Task<LogInUserDto> GetUserDetailsById(GetUserByIdQuery query)
        {
            using (var con = await DapperConnection.OpenConnectionAsync())
            {
                var retVal = await con.QueryAsync<LogInUserDto>(ViewModel.SqlQueries.Sql.GetUserById,
                     new { UserId = query.Id });
                var t = retVal.FirstOrDefault();
                return t;
            }
        }

        public async Task<LogInUserDto> GetUserDetailsByMembershipId(GetUserByMembershipQuery query)
        {
            using (var con = await DapperConnection.OpenConnectionAsync())
            {
                var retVal = await con.QueryAsync<LogInUserDto>(ViewModel.SqlQueries.Sql.GetUserByMembershipId,
                     new { MembershipUserId = query.MembershipId });
                var t = retVal.FirstOrDefault();
                if (t == null)
                {
                    throw new UserNotFoundException();
                }
                return t;
            }
        }

        public async Task<LogInUserDto> GetUserDetailsByEmail(GetUserByEmailQuery query)
        {
            using (var con = await DapperConnection.OpenConnectionAsync())
            {
                var retVal = await con.QueryAsync<LogInUserDto>(ViewModel.SqlQueries.Sql.GetUserByEmail,
                     new { query.Email });
                return retVal.FirstOrDefault();
            }
        }

        public async Task<ForgotPasswordDto> GetForgotPasswordByEmailAsync(GetUserByEmailQuery query,
            CancellationToken token)
        {
            using (var con = await DapperConnection.OpenConnectionAsync(token))
            {
                var retVal = await con.QueryAsync<ForgotPasswordDto>(new CommandDefinition(ViewModel.SqlQueries.Sql.GetUserDetailsForgotPassword,
                   cancellationToken: token, parameters: new { query.Email }));
                return retVal.FirstOrDefault();
            }
        }
        #endregion


        private UserRelationshipType GetUserStatusToBox(BoxSecurityDto securityDto)
        {
            if (securityDto.BoxType == BoxType.AcademicClosed)
            {
                if (securityDto.LibraryUserType == UserLibraryRelationType.None ||
                    securityDto.LibraryUserType == UserLibraryRelationType.Pending)
                {
                    throw new BoxAccessDeniedException();
                }
            }
            if (securityDto.UserType == UserRelationshipType.Owner)
            {
                return securityDto.UserType;
            }
            if (securityDto.PrivacySetting == BoxPrivacySetting.AnyoneWithUrl)
            {
                return securityDto.UserType;
            }

            if (securityDto.PrivacySetting == BoxPrivacySetting.MembersOnly)
            {
                if (securityDto.UserType == UserRelationshipType.Subscribe || securityDto.UserType == UserRelationshipType.Invite)
                {
                    return securityDto.UserType;
                }
            }

            throw new BoxAccessDeniedException();
        }


        public UserRelationshipType GetUserStatusToBox(long boxId, long userId, Guid? inviteId)
        {
            using (var conn = DapperConnection.OpenConnection())
            {
                var sqlQueries = ViewModel.SqlQueries.Security.GetUserAccessParams;// string.Format("{0} {1} ", ViewModel.SqlQueries.Security.GetBoxPrivacySettings,
                //ViewModel.SqlQueries.Security.GetUserToBoxRelationship);
                if (inviteId.HasValue)
                {
                    sqlQueries += " " + ViewModel.SqlQueries.Security.GetInviteToBoxInvite;
                }
                using (
                    var grid =
                            conn.QueryMultiple(sqlQueries, new { boxId, userId, inviteId }))
                {
                    try
                    {
                        var userAccess = grid.Read<BoxSecurityDto>().First();
                        //var privacySettings = grid.Read<BoxPrivacySettings>().First();
                        //var userType = grid.Read<UserRelationshipType>().FirstOrDefault();
                        if (inviteId.HasValue && userAccess.BoxType != BoxType.AcademicClosed)
                        {
                            var x = grid.Read<int>();
                            if (x.Any())
                            {
                                return UserRelationshipType.Invite;
                            }
                        }
                        return GetUserStatusToBox(userAccess);
                    }
                    catch (InvalidOperationException)
                    {
                        throw new BoxDoesntExistException();
                    }
                }

            }
        }
        public async Task<UserRelationshipType> GetUserStatusToBoxAsync(long boxId, long userId)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                try
                {
                    var reVal = (await conn.QueryAsync<BoxSecurityDto>(ViewModel.SqlQueries.Security.GetUserAccessParams,
                        new { boxId, userId })).First();
                    return GetUserStatusToBox(reVal);
                }
                catch (InvalidOperationException)
                {
                    throw new BoxDoesntExistException();
                }


            }
        }






    }
}
