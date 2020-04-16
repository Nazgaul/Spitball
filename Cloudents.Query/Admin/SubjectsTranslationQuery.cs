//using Cloudents.Core.DTOs.Admin;
//using Dapper;
//using System.Collections.Generic;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Cloudents.Query.Admin
//{
//    public class SubjectsTranslationQuery : IQuery<IEnumerable<SubjectDto>>
//    {
//        internal sealed class SubjectsTranslationQueryHandler : IQueryHandler<SubjectsTranslationQuery, IEnumerable<SubjectDto>>
//        {
//            private readonly IDapperRepository _dapperRepository;
//            public SubjectsTranslationQueryHandler(IDapperRepository dapperRepository)
//            {
//                _dapperRepository = dapperRepository;
//            }

//            public async Task<IEnumerable<SubjectDto>> GetAsync(SubjectsTranslationQuery query,
//                CancellationToken token)
//            {
//                const string sql = @"select cs.Id, cs.Name as HeName, st.NameTranslation as EnName
//                                    from sb.CourseSubject cs
//                                    left join sb.SubjectTranslation st
//	                                    on cs.Id = st.SubjectId 
//	                                    and st.LanguageId = (select Id from sb.AdminLanguage where Name = 'en')";

//                using var conn = _dapperRepository.OpenConnection();
//                return await conn.QueryAsync<SubjectDto>(sql);
//            }
//        }
//    }
//}
