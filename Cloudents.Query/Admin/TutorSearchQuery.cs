using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Enum;
using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;

namespace Cloudents.Query.Admin
{
    public class TutorSearchQuery : IQueryAdmin2<IEnumerable<TutorDto>>
    {
        public TutorSearchQuery(string term, ItemState? state, Country? country)
        {
            Term = term;
            State = state;
            Country = country;
        }

        private string Term { get; }
        private ItemState? State { get; }
        public Country? Country { get; }

        internal sealed class TutorSearchQueryHandler : IQueryHandler<TutorSearchQuery, IEnumerable<TutorDto>>
        {
            private readonly IDapperRepository _dapperRepository;

            public TutorSearchQueryHandler(IDapperRepository dapperRepository)
            {
                _dapperRepository = dapperRepository;
            }

            public async Task<IEnumerable<TutorDto>> GetAsync(TutorSearchQuery query, CancellationToken token)
            {

                const string sqlWithTerm = @"Select @Term = case when @Term is null then '""""' else '""' + @Term+ '*""' end 
                            select u.Id, u.Name, u.Email, u.PhoneNumberHash as PhoneNumber, u.Country, t.State, t.Created
                            from sb.Tutor t
                            join sb.[User] u
	                            on u.Id = t.Id
                            where CONTAINS(([Name], Email, PhoneNumberHash) , @Term)
                            and (u.SbCountry = @Country or @Country is null)
                            and (t.State = @State or @State is null)
                            order by u.Id;";

                const string sqlWithoutTerm = @"select u.Id, u.Name, u.Email, u.PhoneNumberHash as PhoneNumber, u.Country, 
                                    t.State, t.Created
                            from sb.Tutor t
                            join sb.[User] u
	                            on u.Id = t.Id
                            where (u.SbCountry = @Country or @Country is null)
                            and (t.State = @State or @State is null)
                            order by u.Id;";

                var sql = string.IsNullOrEmpty(query.Term) ? sqlWithoutTerm : sqlWithTerm;
                using var conn = _dapperRepository.OpenConnection();
                return await conn.QueryAsync<TutorDto>(sql, new
                {
                    query.Term,
                    State = query.State?.ToString(),
                    Country = query.Country?.Id,
                });
            }
        }
    }
}
