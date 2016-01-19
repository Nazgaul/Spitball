﻿using System.Threading;
using System.Threading.Tasks;
using Dapper;
using NHibernate;
using System;
using System.Linq;
using Zbang.Zbox.Infrastructure.Data.Dapper;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.ViewModel.Dto.UserDtos;
using Zbang.Zbox.ViewModel.Queries;
using ExtensionTransformers = Zbang.Zbox.Infrastructure.Data.Transformers;

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
                var t = retVal.FirstOrDefault();
                return t;
            }
        }

        public async Task<LogInUserDto> GetUserDetailsByGoogleIdAsync(GetUserByGoogleQuery query, CancellationToken cancellationToken)
        {
            using (var con = await DapperConnection.OpenConnectionAsync(cancellationToken))
            {
                var retVal = await con.QueryAsync<LogInUserDto>(new CommandDefinition(ViewModel.SqlQueries.Sql.GetUserByGoogleId,
                    cancellationToken: cancellationToken, parameters: new { GoogleUserId = query.GoogleId }));
                var t = retVal.FirstOrDefault();
                return t;
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
                var t = retVal.FirstOrDefault();
                //if (t == null)
                //{
                //    throw new UserNotFoundException();
                //}
                return t;
            }
        }
        #endregion


        private UserRelationshipType GetUserStatusToBox(BoxPrivacySettings privacySettings, UserRelationshipType userRelationShipType)
        {
            if (userRelationShipType == UserRelationshipType.Owner)
            {
                return userRelationShipType;
            }
            if (privacySettings == BoxPrivacySettings.AnyoneWithUrl)
            {
                return userRelationShipType;
            }

            if (privacySettings == BoxPrivacySettings.MembersOnly)
            {
                if (userRelationShipType == UserRelationshipType.Subscribe || userRelationShipType == UserRelationshipType.Invite)
                {
                    return userRelationShipType;
                }
            }

            throw new BoxAccessDeniedException();
        }


        public UserRelationshipType GetUserStatusToBox(long boxId, long userId, Guid? inviteId)
        {
            using (var conn = DapperConnection.OpenConnection())
            {
                var sqlQueries = string.Format("{0} {1} ", ViewModel.SqlQueries.Security.GetBoxPrivacySettings,
                    ViewModel.SqlQueries.Security.GetUserToBoxRelationship);
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
                        var privacySettings = grid.Read<BoxPrivacySettings>().First();
                        var userType = grid.Read<UserRelationshipType>().FirstOrDefault();
                        if (inviteId.HasValue)
                        {
                            var x = grid.Read<int>();
                            if (x.Any())
                            {
                                return UserRelationshipType.Invite;
                            }
                        }
                        return GetUserStatusToBox(privacySettings, userType);
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
                using (
                    var grid =
                          await conn.QueryMultipleAsync(
                                string.Format("{0} {1} ", ViewModel.SqlQueries.Security.GetBoxPrivacySettings,
                                    ViewModel.SqlQueries.Security.GetUserToBoxRelationship), new { boxId, userId }))
                {
                    try
                    {
                        var privacySettings = grid.Read<BoxPrivacySettings>().First();
                        var userType = grid.Read<UserRelationshipType>().FirstOrDefault();
                        return GetUserStatusToBox(privacySettings, userType);
                    }
                    catch (InvalidOperationException)
                    {
                        throw new BoxDoesntExistException();
                    }
                }

            }
        }






    }
}
