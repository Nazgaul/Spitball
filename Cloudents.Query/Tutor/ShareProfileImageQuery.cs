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

        private long Id { get;  }


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
                    .Join(_statelessSession.Query<User>(),x=>x.Id,x=>x.Id,((tutor, user) => new  {tutor,user}))
                    .Where(w => w.tutor.Id == query.Id)
                    .Select(s => new ShareProfileImageDto()
                    {
                        Image = s.tutor.ImageName,
                        Name = s.tutor.Name,
                        Rate = s.tutor.Rate.GetValueOrDefault(),
                        Description = s.user.Description
                    }).FirstOrDefaultAsync(token);
            }
        }
    }
}