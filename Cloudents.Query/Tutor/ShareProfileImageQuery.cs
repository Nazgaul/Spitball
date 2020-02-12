using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.Tutors;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query.Tutor
{
    public class ShareProfileImageQuery : IQuery<ShareProfileImageDto>
    {
        public ShareProfileImageQuery(long id)
        {
            Id = id;
        }

        public long Id { get;  }


        internal sealed class ShareProfileImageQueryHandler : IQueryHandler<ShareProfileImageQuery,ShareProfileImageDto>
        {
            private readonly IStatelessSession _statelessSession;

            public ShareProfileImageQueryHandler(QuerySession statelessSession)
            {
                _statelessSession = statelessSession.StatelessSession;
            }

            public async Task<ShareProfileImageDto> GetAsync(ShareProfileImageQuery query, CancellationToken token)
            {
                return  await _statelessSession.Query<ReadTutor>()
                    .Where(w => w.Id == query.Id)
                    .Select(s => new ShareProfileImageDto()
                    {
                        CountryStr = s.Country,
                        Image = s.ImageName,
                        Name = s.Name,
                        Rate = s.Rate.GetValueOrDefault()
                    }).FirstOrDefaultAsync(token);
            }
        }
    }
}