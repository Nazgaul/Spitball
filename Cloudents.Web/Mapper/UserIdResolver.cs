using AutoMapper;
using Cloudents.Core.Command;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Http;

namespace Cloudents.Web.Mapper
{

    public class UserIdResolver : IValueResolver<object,object,long>
    {
        private readonly IHttpContextAccessor _context;

        public UserIdResolver(IHttpContextAccessor context)
        {
            _context = context;
        }

        public long Resolve(object source, object destination, long destMember, ResolutionContext context)
        {
            return _context.HttpContext.User.GetUserId();
        }
    }
}
