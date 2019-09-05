using Cloudents.Core.DTOs.Admin;
using Cloudents.Query;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Query.Query.Admin;
using Microsoft.AspNetCore.Authorization;
using Cloudents.Core.Extension;

namespace Cloudents.Admin2.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AdminStudyRoomController: ControllerBase
    {
        private readonly IQueryBus _queryBus;

        public AdminStudyRoomController(IQueryBus queryBus)
        {
            _queryBus = queryBus;
        }

        [HttpGet]
        public async Task<IEnumerable<StudyRoomDto>> StudyRoomAsync(CancellationToken token)
        {
            var query = new AdminStudyRoomQuery(User.GetCountryClaim());
            var retVal = await _queryBus.QueryAsync(query, token);
            return retVal;
        }

        [HttpGet("tutors")]
        public async Task<IEnumerable<TutorInfoDto>> TutorsAsync(CancellationToken token)
        {
            var query = new AdminTutorsWithStudyRoomsQuery(User.GetCountryClaim());
            var retVal = await _queryBus.QueryAsync(query, token);
            return retVal;
        }

        [HttpGet("bills")]
        public async Task<IEnumerable<SessionBillDto>> BillsAsync(long Id, CancellationToken token)
        {
            var query = new AdminTutorSessionsQuery(Id, User.GetCountryClaim());
            var retVal = await _queryBus.QueryAsync(query, token);
            return retVal;
        }
    }
}
