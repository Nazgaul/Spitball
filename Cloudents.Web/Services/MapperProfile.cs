using System.Linq;
using AutoMapper;
using Cloudents.Core.Command;
using Cloudents.Core.Query;
using Cloudents.Web.Models;
using JetBrains.Annotations;

namespace Cloudents.Web.Services
{
    [UsedImplicitly]
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<CreateQuestionRequest, CreateQuestionCommand>()
                .ForMember(f => f.UserId, c => c.ResolveUsing<UserIdResolver>());

            //CreateMap<CreateAnswerRequest, CreateAnswerCommand>()
            //    .ForMember(f => f.UserId, c => c.ResolveUsing<UserIdResolver>());

            //CreateMap<MarkAsCorrectRequest, MarkAnswerAsCorrectCommand>()
            //    .ForMember(f => f.UserId, c => c.ResolveUsing<UserIdResolver>());

            CreateMap<AssignUniversityRequest, AssignUniversityToUserCommand>()
                .ForMember(f => f.UserId, c => c.ResolveUsing<UserIdResolver>());

            //CreateMap<UpVoteAnswerRequest, UpVoteAnswerCommand>()
            //    .ForMember(f => f.UserId, c => c.ResolveUsing<UserIdResolver>());

            CreateMap<GetQuestionsRequest, QuestionsQuery>()
                .ForMember(f => f.Term, c => c.MapFrom(f => string.Join(" ", f.Term ?? Enumerable.Empty<string>())))
                .ForMember(f => f.Page, c => c.MapFrom(f => f.Page.GetValueOrDefault()))
                .ForMember(f => f.Source, opt => opt.AllowNull())
                .AfterMap((_,d) =>
                {
                    if (d.Source.Length == 0)
                    {
                        d.Source = null;
                    }
                });


            CreateMap<DeleteAnswerRequest, DeleteAnswerCommand>()
                .ForMember(f => f.UserId, c => c.ResolveUsing<UserIdResolver>());

            CreateMap<DeleteQuestionRequest, DeleteQuestionCommand>()
                .ForMember(f => f.UserId, c => c.ResolveUsing<UserIdResolver>());

            CreateMap<CreateRedeemRequest, RedeemTokenCommand>()
                .ForMember(f => f.UserId, c => c.ResolveUsing<UserIdResolver>());
            //DeleteAnswerCommand

            //.ForMember(f => f.UserId, c => c.ResolveUsing<UserIdResolver>());

            //.ForMember(f => f.UserId, c => c.ResolveUsing<UserIdResolver>());
            //.ForMember(f => f.UserId, c => c.ResolveUsing<UserIdResolver>());
        }

        //IMappingAction
    }
}
