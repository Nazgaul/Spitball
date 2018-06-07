using AutoMapper;
using Cloudents.Core.Entities.Db;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Cloudents.Web.Mapper
{
    [UsedImplicitly]
    public class UserIdResolver : IValueResolver<object,object,long>
    {
        private readonly IHttpContextAccessor _context;
        private readonly UserManager<User> _userManager;

        public UserIdResolver(IHttpContextAccessor context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public long Resolve(object source, object destination, long destMember, ResolutionContext context)
        {
            return long.Parse(_userManager.GetUserId(_context.HttpContext.User));
        }
    }
}
