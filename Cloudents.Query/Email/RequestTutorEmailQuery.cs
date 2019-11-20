using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.Email;
using Cloudents.Core.Entities;
using Cloudents.Core.Message.System;
using NHibernate.Linq;

namespace Cloudents.Query.Email
{
    public class RequestTutorEmailQuery : IQuery<IEnumerable<RequestTutorEmailDto>>
    {
        public RequestTutorEmailQuery(Guid leadId)
        {
            LeadId = leadId;
        }

        private Guid LeadId { get;  }


        internal sealed class RequestTutorEmailQueryHandler : IQueryHandler<RequestTutorEmailQuery, IEnumerable<RequestTutorEmailDto>>
        {
            private readonly QuerySession _querySession;

            public RequestTutorEmailQueryHandler(QuerySession querySession)
            {
                _querySession = querySession;
            }

            public async Task<IEnumerable<RequestTutorEmailDto>> GetAsync(RequestTutorEmailQuery query, CancellationToken token)
            {
               return await _querySession.StatelessSession.Query<Lead>()
                    .Fetch(f => f.Tutor).ThenFetch(f => f.User)
                    .Fetch(f => f.User)
                    .Where(w => w.Id == query.LeadId).Select(s => new RequestTutorEmailDto
                    {
                        TutorLanguage = s.Tutor.User.Language,
                        CourseName = s.Course.Id,
                        Request = s.Text,
                        TutorId = s.Tutor.Id,
                        TutorEmail = s.Tutor.User.Email,
                        StudentName = s.User.Name,
                        TutorFirstName = s.Tutor.User.FirstName,
                        StudentPhoneNumber = s.User.PhoneNumber
                    }).ToListAsync(token);
            }
        }
    }
}