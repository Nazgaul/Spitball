using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Attributes;
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

            [Cache(TimeConst.Minute * 10, "share-production", false)]
            public async Task<ShareProfileImageDto> GetAsync(ShareProfileImageQuery query, CancellationToken token)
            {
                return  await _statelessSession.Query<ReadTutor>()
                    .WithOptions(w => w.SetComment(nameof(ShareProfileImageQuery)))
                    //.Join(_statelessSession.Query<User>(),x=>x.Id,x=>x.Id,((tutor, user) => new  {tutor,user}))
                    .Where(w => w.Id == query.Id)
                    .Select(s => new ShareProfileImageDto()
                    {
                        Image = s.ImageName,
                        Name = s.Name,
                        Rate = s.Rate.GetValueOrDefault(),
                        Description = s.Description
                    }).SingleOrDefaultAsync(token);
            }
        }
    }
}