﻿using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Enum;
using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Admin
{
    public class TutorSearchQuery : IQueryAdmin<IEnumerable<TutorDto>>
    {
        public TutorSearchQuery(string term, ItemState? state, int page, string country)
        {
            Term = term;
            State = state;
            Page = page;
            Country = country;
        }

        private string Term { get; }
        private ItemState? State { get;  }
        private int Page { get; }
        public string Country { get; }

        internal sealed class TutorSearchQueryHandler : IQueryHandler<TutorSearchQuery, IEnumerable<TutorDto>>
        {
            private readonly IDapperRepository _dapperRepository;

            public TutorSearchQueryHandler(IDapperRepository dapperRepository)
            {
                _dapperRepository = dapperRepository;
            }

            public async Task<IEnumerable<TutorDto>> GetAsync(TutorSearchQuery query, CancellationToken token)
            {
                const int pageSize = 200;

                const string sqlWithTerm = @"Select @Term = case when @Term is null then '""""' else '""' + @Term+ '*""' end 
                            select u.Id, u.Name, u.Email, u.PhoneNumberHash as PhoneNumber, u.Country, t.State 
                            from sb.Tutor t
                            join sb.[User] u
	                            on u.Id = t.Id
                            where CONTAINS(([Name], Email, PhoneNumberHash) , @Term)
                            where (u.Country = @Country or @Country is null)
                            and (t.State = @State or @State is null)
                            order by u.Id
                            OFFSET @PageSize * @Page ROWS
                            FETCH NEXT @PageSize ROWS ONLY;";

                const string sqlWithoutTerm = @"select u.Id, u.Name, u.Email, u.PhoneNumberHash as PhoneNumber, u.Country, t.State 
                            from sb.Tutor t
                            join sb.[User] u
	                            on u.Id = t.Id
                            where (u.Country = @Country or @Country is null)
                            and (t.State = @State or @State is null)
                            order by u.Id
                            OFFSET @PageSize * @Page ROWS
                            FETCH NEXT @PageSize ROWS ONLY;";

                var sql = string.IsNullOrEmpty(query.Term) ? sqlWithoutTerm : sqlWithTerm;
                using var conn = _dapperRepository.OpenConnection();
                return await conn.QueryAsync<TutorDto>(sql, new
                {
                    query.Term,
                    State = query.State?.ToString(),
                    query.Country,
                    PageSize = pageSize,
                    query.Page
                });
            }
        }
    }
}