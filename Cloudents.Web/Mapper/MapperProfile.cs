using AutoMapper;
using Cloudents.Core.Command;
using Cloudents.Web.Models;
using JetBrains.Annotations;

namespace Cloudents.Web.Mapper
{
    [UsedImplicitly]
    public class MapperProfile :  Profile
    {
        public MapperProfile()
        {
            //_userManager = userManager;
           // _context = context;

            CreateMap<QuestionRequest, CreateQuestionCommand>()
                .ForMember(f => f.UserId,c=>c.ResolveUsing<UserIdResolver>());

            CreateMap<CreateAnswerRequest, CreateAnswerCommand>()
                .ForMember(f => f.UserId, c => c.ResolveUsing<UserIdResolver>());

            CreateMap<MarkAsCorrectRequest, MarkAnswerAsCorrectCommand>()
                .ForMember(f => f.UserId, c => c.ResolveUsing<UserIdResolver>());

            //MarkAsCorrectRequest
        }
    }
}
