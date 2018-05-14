using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Cloudents.Web.Models
{
    public class MapperProfile :  Profile
    {
        public MapperProfile()
        {
            //_userManager = userManager;
           // _context = context;

            CreateMap<QuestionRequest, CreateQuestionCommand>().ForMember(f => f.UserId,c=>c.ResolveUsing<UserIdResolver>());

        }

    }

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
