using Cloudents.Core.DTOs;
using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Tutor
{
    public class AboutTutorQuery : IQuery<IEnumerable<AboutTutorDto>>
    {
        internal sealed class TutorListByCourseQueryHandler : IQueryHandler<AboutTutorQuery, IEnumerable<AboutTutorDto>>
        {

            private readonly IDapperRepository _dapperRepository;

            public TutorListByCourseQueryHandler(IDapperRepository dapperRepository)
            {
                _dapperRepository = dapperRepository;
            }

            public async Task<IEnumerable<AboutTutorDto>> GetAsync(AboutTutorQuery query, CancellationToken token)
            {
                const string sql = @"select top 10 u.Name, u.Image, tr.Rate, tr.Review
                                        from sb.TutorReview tr
                                        join sb.Tutor t
	                                        on tr.TutorId = t.Id
                                        join sb.[User] u
	                                        on t.Id = u.Id
                                        where len(tr.Review) > 0 and tr.Rate > 3
                                        order by NEWID()";
                using (var conn = _dapperRepository.OpenConnection())
                {
                    var retVal = await conn.QueryAsync<AboutTutorDto>(sql);

                    return retVal;
                }
            }
        }
    }
}
