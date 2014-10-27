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
                if (t == null)
                {
                    throw new UserNotFoundException();
                }
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
                if (t == null)
                {
                    throw new UserNotFoundException();
                }
                return t;
            }
        }
        #endregion

        /// <summary>
        /// Check if user have rights in dto model only
        /// </summary>
        /// <param name="boxId">The Box the user requested to view</param>
        /// <param name="userId">The user</param>
        /// <returns>If the user authorize to view return the type of the user otherwise throw exception that the box is denied</returns>
        protected UserRelationshipType CheckIfUserAllowedToSee(long boxId, long userId)
        {


            IQuery boxQuery = UnitOfWork.CurrentSession.CreateQuery("select b.PrivacySettings.PrivacySetting from Box b where b.Id = :boxId ");
            boxQuery.SetInt64("boxId", boxId);

            IQuery userQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetUserRelationToBox");
            userQuery.SetInt64("BoxId", boxId);
            userQuery.SetParameter("UserId", userId);

            var box = boxQuery.FutureValue<BoxPrivacySettings?>();
            var userType = userQuery.FutureValue<UserRelationshipType>();
            if (!box.Value.HasValue)
            {
                throw new BoxDoesntExistException();
            }
            return GetUserStatusToBox(box.Value.Value, userType.Value);
        }
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


        public UserRelationshipType GetUserStatusToBox(long boxId, long userId)
        {
            using (var conn = DapperConnection.OpenConnection())
            {
                using (
                    var grid =

                            conn.QueryMultiple(
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

        public long GetItemIdByBlobId(string blobId)
        {
            using (UnitOfWork.Start())
            {
                IQuery query = UnitOfWork.CurrentSession.GetNamedQuery("GetItemIdByBlobId");
                query.SetString("BlobName", blobId);
                var x = query.UniqueResult();
                return Convert.ToInt64(x);
            }
        }

    }
}
