using Cloudents.Core.DTOs.Admin;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query.Admin
{
    public class SubjectsTranslationQuery : IQueryAdmin2<IEnumerable<SubjectDto>>
    {
        public SubjectsTranslationQuery(Country? country)
        {
            Country = country;
        }

        public Country? Country { get; }

        internal sealed class SubjectsTranslationQueryHandler : IQueryHandler<SubjectsTranslationQuery, IEnumerable<SubjectDto>>
        {
            private readonly IStatelessSession _statelessSession;
            public SubjectsTranslationQueryHandler(QuerySession dapperRepository)
            {
                _statelessSession = dapperRepository.StatelessSession;
            }

            public async Task<IEnumerable<SubjectDto>> GetAsync(SubjectsTranslationQuery query,
                CancellationToken token)
            {

                var x = _statelessSession.Query<CourseSubject>();
                if (query.Country != null)
                {
                    x = x.Where(w => w.Country == query.Country);
                }
                return await x.Select(s => new SubjectDto
                {
                    Id = s.Id,
                    Name = s.Name
                }).ToListAsync(token);

                //const string sql = @"select cs.Id, cs.Name as HeName, st.NameTranslation as EnName
                //                    from sb.CourseSubject cs
                //                    left join sb.SubjectTranslation st
                //                    left join sb.SubjectTranslation st
                //                     on cs.Id = st.SubjectId 
                //                     and st.LanguageId = (select Id from sb.AdminLanguage where Name = 'en')";

                //using var conn = _dapperRepository.OpenConnection();
                //return await conn.QueryAsync<SubjectDto>(sql);
            }
        }

    }
}
