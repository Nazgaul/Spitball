using AutoMapper;
using Cloudents.Core.Command;
using Cloudents.Web.Models;

namespace Cloudents.Web.Mapper
{
    public class MapperProfile :  Profile
    {
        public MapperProfile()
        {
            //_userManager = userManager;
           // _context = context;

            CreateMap<QuestionRequest, CreateQuestionCommand>()
                .ForMember(f => f.UserId,c=>c.ResolveUsing<UserIdResolver>());
        }
    }
}
