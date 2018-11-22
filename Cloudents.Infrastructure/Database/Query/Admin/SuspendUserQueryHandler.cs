//using System.Diagnostics.CodeAnalysis;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Cloudents.Core.DTOs;
//using Cloudents.Core.Entities.Db;
//using Cloudents.Core.Interfaces;
//using Cloudents.Core.Query;
//using NHibernate;
//using NHibernate.Linq;

//namespace Cloudents.Infrastructure.Database.Query.Admin
//{
//    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
//    public class SuspendUserQueryHandler : IQueryHandler<UserDataByIdQuery, SuspendUserDto>
//    {
//        private readonly ISession _session;

//        public SuspendUserQueryHandler(QuerySession session)
//        {
//            _session = session.Session;
//        }

//        public async Task<SuspendUserDto> GetAsync(UserDataByIdQuery query, CancellationToken token)
//        {
//            var answerQuery = _session.Query<Answer>();
//            var futureAnswersData = answerQuery.Where(w => w.User.Id == query.Id)
//                    .Select(s => s.Id)
//                    .ToFuture();
//            var futureQuestionsData = _session.Query<Question>()
//               .Where(w => w.User.Id == query.Id)
//               .Select(s => s.Id)
//               .ToFuture();

//            var dto = new SuspendUserDto
//            {
//                Answers = await futureAnswersData.GetEnumerableAsync(token),
//                Questions = await futureQuestionsData.GetEnumerableAsync(token)
//            };

//            return dto;
//        }
//    }
//}
