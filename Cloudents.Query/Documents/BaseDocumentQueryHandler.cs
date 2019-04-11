//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Cloudents.Core.DTOs;
//using Cloudents.Core.Entities;
//using Cloudents.Core.Enum;
//using Cloudents.Query.Query;
//using NHibernate;
//using NHibernate.Linq;

//namespace Cloudents.Query.Documents
//{
//    public abstract class BaseDocumentQueryHandler
//    {
//        private readonly IStatelessSession _session;

//        protected BaseDocumentQueryHandler(QuerySession session)
//        {
//            _session = session.StatelessSession;
//        }


//        public async Task<IList<DocumentFeedDto>> DoQuery(CancellationToken token)
//        {
//            IQueryable<Document> z = _session.Query<Document>()
//                .Fetch(f => f.User)
//                .Fetch(f => f.University)
//                .Where(w=>w.Status.State == ItemState.Ok);

            
//                //.Where(w => Filter(w))
//                z.Select(s => new DocumentFeedDto
//                {
//                    Id = s.Id,
//                    User = new UserDto
//                    {
//                        Id = s.User.Id,
//                        Name = s.User.Name,
//                        Score = s.User.Score,
//                        Image = s.User.Image
//                    },
//                    DateTime = s.TimeStamp.UpdateTime,
//                    Course = s.Course.Id,
//                    Type = s.Type,
//                    Professor = s.Professor,
//                    Title = s.Name,
//                    Snippet = s.MetaContent,
//                    Views = s.Views,
//                    Downloads = s.Downloads,
//                    University = s.University.Name,
//                    Price = s.Price,
//                    Vote = new VoteDto()
//                    {
//                        Votes = s.VoteCount
//                    }
//                });
                

//            return z.;

//        }

//        protected abstract bool Filter(Document w);

        

       
//    }
//}