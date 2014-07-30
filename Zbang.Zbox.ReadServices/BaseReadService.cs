using NHibernate;
using NHibernate.Transform;
using System;
using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.ViewModel.Dto.UserDtos;
using Zbang.Zbox.ViewModel.Queries;
using ExtensionTransformers = Zbang.Zbox.Infrastructure.Data.Transformers;

namespace Zbang.Zbox.ReadServices
{
    public abstract class BaseReadService : IBaseReadService
    {
        private readonly IHttpContextCacheWrapper m_ContextCacheWrapper;
        public BaseReadService(IHttpContextCacheWrapper contextCacheWrapper)
        {
            m_ContextCacheWrapper = contextCacheWrapper;
        }
        #region login
        public LogInUserDto GetUserDetailsByFacebookId(GetUserByFacebookQuery query)
        {
            using (UnitOfWork.Start())
            {
                IQuery dbQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetUserByFacebookId");
                dbQuery.SetInt64("FacebookId", query.FacebookId);
                dbQuery.SetResultTransformer(Transformers.AliasToBean<LogInUserDto>());
                var t = dbQuery.UniqueResult<LogInUserDto>();
                if (t == null)
                {
                    throw new UserNotFoundException();
                }
                return t;
            }
        }

        public LogInUserDto GetUserDetailsByMembershipId(GetUserByMembershipQuery query)
        {
            using (UnitOfWork.Start())
            {
                IQuery dbQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetUserByMembershipId");
                dbQuery.SetResultTransformer(Transformers.AliasToBean<LogInUserDto>());
                dbQuery.SetGuid("MembershipUserId", query.MembershipId);
                var t = dbQuery.UniqueResult<LogInUserDto>();
                if (t == null)
                {
                    throw new UserNotFoundException();
                }
                return t;
            }
        }

        public LogInUserDto GetUserDetailsByEmail(GetUserByEmailQuery query)
        {
            using (UnitOfWork.Start())
            {
                IQuery dbQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetUserByEmail");
                dbQuery.SetResultTransformer(Transformers.AliasToBean<LogInUserDto>());
                dbQuery.SetString("Email", query.Email);
                var t = dbQuery.UniqueResult<LogInUserDto>();
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
            const string key = "AllowedToSee";
            var cacheElem = m_ContextCacheWrapper.GetObject(key);
            if (cacheElem != null)
            {
                return (UserRelationshipType)cacheElem;
            }


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
        protected UserRelationshipType GetUserStatusToBox(BoxPrivacySettings privacySettings, UserRelationshipType userRelationShipType)
        {
            const string key = "AllowedToSee";
            if (userRelationShipType == UserRelationshipType.Owner)
            {
                m_ContextCacheWrapper.AddObject(key, userRelationShipType);
                return userRelationShipType;
            }
            if (privacySettings == BoxPrivacySettings.AnyoneWithUrl)
            {
                m_ContextCacheWrapper.AddObject(key, userRelationShipType);
                return userRelationShipType;
            }

            if (privacySettings == BoxPrivacySettings.MembersOnly)
            {
                if (userRelationShipType == UserRelationshipType.Subscribe || userRelationShipType == UserRelationshipType.Invite)
                {
                    m_ContextCacheWrapper.AddObject(key, userRelationShipType);
                    return userRelationShipType;
                }
            }

            throw new BoxAccessDeniedException();
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
