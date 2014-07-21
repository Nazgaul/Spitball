using System;
using System.Web.Mvc;
using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.DTOs.UserDtos;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Cloudents.Mvc4WebRole.Helpers
{
    public class UserProfile : IUserProfile
    {
        public const string UserDetail = "userDetail";
        private readonly IZboxReadService m_ZboxReadService;
        private readonly IHttpContextCacheWrapper m_ContextCacheService;

        public UserProfile(IZboxReadService zboxReadService,
            IHttpContextCacheWrapper contextCacheService)
        {

            m_ZboxReadService = zboxReadService;
            m_ContextCacheService = contextCacheService;
        }
        public UserDetailDto GetUserData(ControllerContext controllerContext)
        {
           
            var result = m_ContextCacheService.GetObject(UserDetail) as UserDetailDto;

            if (result != null)
            {
                return result;
            }

            result = controllerContext.Controller.TempData[UserDetail] as UserDetailDto;
            if (result != null)
            {
                m_ContextCacheService.AddObject(UserDetail, result);
                return result;
            }

            if (!controllerContext.HttpContext.User.Identity.IsAuthenticated)
            {
                return null;
            }
// ReSharper disable once RedundantAssignment - need user id to be -1
            long userid = -1;
            if (!long.TryParse(controllerContext.HttpContext.User.Identity.Name, out userid))
            {
                throw new ArgumentException("user is not a number");
            }

            var query = new GetUserDetailsQuery(userid);
            var userData = m_ZboxReadService.GetUserData(query);
            m_ContextCacheService.AddObject(UserDetail, userData);
            return userData;

        }

    }

    public interface IUserProfile
    {
        UserDetailDto GetUserData(ControllerContext controllerContext);
    }
}