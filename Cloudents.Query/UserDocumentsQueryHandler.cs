﻿using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Infrastructure.Data;
using Cloudents.Query.Query;
using Dapper;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Query.Stuff;

namespace Cloudents.Query
{
    public class UserDocumentsQueryHandler : IQueryHandler<UserDataPagingByIdQuery, IEnumerable<DocumentFeedDto>>
    {
        private readonly IStatelessSession _session;

        public UserDocumentsQueryHandler(QuerySession session)
        {
            _session = session.StatelessSession;
        }
        public async Task<IEnumerable<DocumentFeedDto>> GetAsync(UserDataPagingByIdQuery query, CancellationToken token)
        {
            return await _session.Query<Document>()
                .Fetch(f => f.University)
                .Fetch(f => f.User)
                .Where(w => w.User.Id == query.Id && w.State == ItemState.Ok)
                .OrderByDescending(o => o.Id)
                .Select(s => new DocumentFeedDto()
                {
                    Id = s.Id,
                    User = new UserDto(s.User.Id, s.User.Name, s.User.Score),
                    DateTime = s.TimeStamp.UpdateTime,
                    Course = s.Course.Name,
                    TypeStr = s.Type,
                    Professor = s.Professor,
                    Title = s.Name,
                    Source = "Cloudents",
                    Views = s.Views,
                    Downloads = s.Downloads,
                    University = s.University.Name,
                    Snippet = s.MetaContent,
                    Vote = new VoteDto()
                    {
                        Votes = s.VoteCount
                    }

                }
                )
                .Take(50).Skip(query.Page * 50)
                .ToListAsync(token);
        }
    }


    public class UserPurchasedDocumentsQueryHandler : IQueryHandler<UserPurchaseDocumentByIdQuery, IEnumerable<DocumentFeedDto>>
    {
        private readonly DapperRepository _dapper;
        private readonly IMapper _mapper;

        

        public UserPurchasedDocumentsQueryHandler(DapperRepository dapper, IMapper mapper)
        {
            _dapper = dapper;
            _mapper = mapper;
        }
        public async Task<IEnumerable<DocumentFeedDto>> GetAsync(UserPurchaseDocumentByIdQuery query, CancellationToken token)
        {
            /*Mapper.Initialize(cfg => {
                cfg.CreateMap<DocumentFeedDto, UserPurchasedDocumentsQueryResult>()
                .ForMember(m => m.University, x => x.MapFrom(z => z.University))
                .ForMember(m => m.Course, x => x.MapFrom(z => z.Course))
                .ForMember(m => m.Snippet, x => x.MapFrom(z => z.Snippet))
                .ForMember(m => m.Title, x => x.MapFrom(z => z.Title))
                .ForMember(m => m.TypeStr, x => x.MapFrom(z => z.TypeStr))
                .ForMember(m => m.UserId, x => x.MapFrom(z => z.User.Id))
                .ForMember(m => m.UserName, x => x.MapFrom(z => z.User.Name))
                .ForMember(m => m.UserScore, x => x.MapFrom(z => z.User.Score))
                .ForMember(m => m.Id, x => x.MapFrom(z => z.Id))
                .ForMember(m => m.DateTime, x => x.MapFrom(z => z.DateTime))
                .ForMember(m => m.Professor, x => x.MapFrom(z => z.Professor))
                .ForMember(m => m.Views, x => x.MapFrom(z => z.Views))
                .ForMember(m => m.Price, x => x.MapFrom(z => z.Price))
                .ForMember(m => m.Downloads, x => x.MapFrom(z => z.Downloads))
                .ForMember(m => m.VoteCount, x => x.MapFrom(z => z.Vote.Votes))
                .ReverseMap()
                .ForPath(s => s.User.Id, opt => opt.MapFrom(src => src.UserId))
                .ForPath(s => s.User.Name, opt => opt.MapFrom(src => src.UserName))
                .ForPath(s => s.User.Score, opt => opt.MapFrom(src => src.UserScore))
                .ForPath(s => s.Source, opt => opt.Ignore())
                .ForPath(s => s.Url, opt => opt.Ignore());
            });*/

            var result = await _dapper.WithConnectionAsync(async connection =>
            {
                return await connection.QueryAsync<UserPurchasedDocumentsQueryResult>(@"select d.Id as id,
u2.id as userId,
u2.Name as userName,
u2.Score as userScore,
d.UpdateTime as 'DateTime',
d.CourseName as 'Course',
d.Type as 'TypeStr',
d.Professor,
d.Name as Title,
d.Views,
d.Downloads,
u.Name as University,
d.MetaContent as Snippet,
d.VoteCount,
d.Price
 from sb.[Transaction] t
Join sb.Document d on t.DocumentId = d.Id and d.state = 'ok'
join sb.University u on u.Id = d.UniversityId
join sb.[User] u2 on u2.Id = d.UserId
where t.User_id = @userId", new { userId = query.Id });
            }, token);

            //return _mapper.Map<IEnumerable<DocumentFeedDto>>(result);
            /*return result.Select(s => new DocumentFeedDto()
            {
                Price = s.Price,
                User = new UserDto(s.UserId, s.UserName, s.UserScore),
                Id = s.Id,
                Course = s.CourseName,
                DateTime = s.UpdateTime,
                Downloads = s.Downloads,
                Professor = s.Professor,
                Snippet = s.MetaContent,
                Source = "Cloudents",
                Title = s.Name,
                TypeStr = s.Type,
                University = s.UniversityName,
                Views = s.Views,
                Vote = new VoteDto()
                {
                    Votes = s.VoteCount
                }
            });*/
            var test = new List<DocumentFeedDto>();
           
            var orderDto = _mapper.Map<IEnumerable<UserPurchasedDocumentsQueryResult>, IEnumerable<DocumentFeedDto>>(result);
            return _mapper.Map(result, test);
            //_mapper.Map<IEnumerable<DocumentFeedDto>>(result);
            // return Mapper.Map<DocumentFeedDto, UserPurchasedDocumentsQueryResult>(result);

        }




       

    }

    public class UserPurchasedDocumentsQueryResult
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public int UserScore { get; set; }
        public DateTime? DateTime { get; set; }
        public string Course { get; set; }
        public DocumentType? TypeStr { get; set; }
        public string Professor { get; set; }
        public string Title { get; set; }
        public int? Views { get; set; }
        public decimal? Price { get; set; }
        public int? Downloads { get; set; }
        public string University { get; set; }
        public string Snippet { get; set; }
        public int VoteCount { get; set; }
    }
}