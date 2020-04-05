using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Tutor
{
    public class GetPhoneNumberQuery : IQuery<string>
    {
        private long TutorId { get; }
        public GetPhoneNumberQuery(long tutorId)
        {
            TutorId = tutorId;
        }

        internal sealed class GetPhoneNumberQueryHandler : IQueryHandler<GetPhoneNumberQuery, string>
        {
            private readonly IStatelessSession _statelessSession;
            public GetPhoneNumberQueryHandler(QuerySession session)
            {
                _statelessSession = session.StatelessSession;
            }

            public async Task<string> GetAsync(GetPhoneNumberQuery query, CancellationToken token)
            {
                return await _statelessSession.Query<User>()
                    .WithOptions(w => w.SetComment(nameof(GetPhoneNumberQuery)))
                    .Where(w => w.Id == query.TutorId).Select(s => s.PhoneNumber).SingleOrDefaultAsync(token);
            }
        }
    }
}
