using Dapper;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Admin
{
    public class SubjectsQuery : IQuery<IEnumerable<string>>
    {
        private Guid AdminUserId { get; }
        public SubjectsQuery(Guid adminUserId)
        {
            AdminUserId = adminUserId;
        }
        internal sealed class SubjectsQueryHandler : IQueryHandler<SubjectsQuery, IEnumerable<string>>
        {
            private readonly IDapperRepository _dapperRepository;
            public SubjectsQueryHandler(IDapperRepository dapperRepository)
            {
                _dapperRepository = dapperRepository;
            }

            public async Task<IEnumerable<string>> GetAsync(SubjectsQuery query,
                CancellationToken token)
            {
                const string sql = @"select COALESCE(st.NameTranslation, cs.Name)
                                        from sb.CourseSubject cs
                                        left join sb.SubjectTranslation st
	                                        on cs.Id = st.SubjectId 
	                                        and st.LanguageId = (select LanguageId from sb.AdminUser where Id = @Id)
                                            order by COALESCE(st.NameTranslation, cs.Name)";

                using var conn = _dapperRepository.OpenConnection();
                return await conn.QueryAsync<string>(sql, new { id = query.AdminUserId });
            }
        }
    }
}
