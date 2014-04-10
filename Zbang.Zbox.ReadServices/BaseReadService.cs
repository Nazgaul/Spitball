using NHibernate;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Linq;
using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.Data.NHibernameUnitOfWork;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.ViewModel.DTOs;
using Zbang.Zbox.ViewModel.DTOs.Notification;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.ViewModel.Queries.Notification;
using ExtensionTransformers = Zbang.Zbox.Infrastructure.Data.Transformers;
using User = Zbang.Zbox.ViewModel.DTOs.UserDtos;

namespace Zbang.Zbox.ReadServices
{
    public abstract class BaseReadService : IBaseReadService
    {
        protected readonly IHttpContextCacheWrapper m_ContextCacheWrapper;
        public BaseReadService(IHttpContextCacheWrapper contextCacheWrapper)
        {
            m_ContextCacheWrapper = contextCacheWrapper;
        }
        #region login
        public User.LogInUserDto GetUserDetailsByFacebookId(GetUserByFacebookQuery query)
        {
            using (UnitOfWork.Start())
            {
                IQuery dbQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetUserByFacebookId");
                dbQuery.SetInt64("FacebookId", query.FacebookId);
                dbQuery.SetResultTransformer(Transformers.AliasToBean<User.LogInUserDto>());
                var t = dbQuery.UniqueResult<User.LogInUserDto>();
                if (t == null)
                {
                    throw new UserNotFoundException();
                }
                return t;
            }
        }

        public User.LogInUserDto GetUserDetailsByMembershipId(GetUserByMembershipQuery query)
        {
            using (UnitOfWork.Start())
            {
                IQuery dbQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetUserByMembershipId");
                dbQuery.SetResultTransformer(Transformers.AliasToBean<User.LogInUserDto>());
                dbQuery.SetGuid("MembershipUserId", query.MembershipId);
                var t = dbQuery.UniqueResult<User.LogInUserDto>();
                if (t == null)
                {
                    throw new UserNotFoundException();
                }
                return t;
            }
        }

        public User.LogInUserDto GetUserDetailsByEmail(GetUserByEmailQuery query)
        {
            using (UnitOfWork.Start())
            {
                IQuery dbQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetUserByEmail");
                dbQuery.SetResultTransformer(Transformers.AliasToBean<User.LogInUserDto>());
                dbQuery.SetString("Email", query.Email);
                var t = dbQuery.UniqueResult<User.LogInUserDto>();
                if (t == null)
                {
                    throw new UserNotFoundException();
                }
                return t;
            }
        }
        #endregion





        //public UserPermissionPerBoxDto GetUserPermission(GetBoxQuery query)
        //{
        //    var result = new UserPermissionPerBoxDto();
        //    using (UnitOfWork.Start())
        //    {
        //        IQuery queryGetUserPermission = UnitOfWork.CurrentSession.GetNamedQuery("GetUserRelationToBox");
        //        queryGetUserPermission.SetInt64("UserId", query.UserId);
        //        queryGetUserPermission.SetInt64("BoxId", query.BoxId);
        //        queryGetUserPermission.SetResultTransformer(Transformers.AliasToBean<UserPermissionPerBoxDto>());

        //        var dbResult = queryGetUserPermission.FutureValue<UserPermissionPerBoxDto>();
        //        CheckIfUserAllowedToSee(query.BoxId, query.UserId);
        //        if (dbResult.Value != null)
        //        {
        //            result = dbResult.Value;
        //        }
        //        result.CanUserSeeData = true;
        //    }

        //    return result;

        //}


        /// <summary>
        /// Check if user have rights in dto model only
        /// </summary>
        /// <param name="boxId">The Box the user requested to view</param>
        /// <param name="userId">The user</param>
        /// <returns>If the user authorize to view return the type of the user otherwise throw exception that the box is denied</returns>
        public UserRelationshipType CheckIfUserAllowedToSee(long boxId, long userId)
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
