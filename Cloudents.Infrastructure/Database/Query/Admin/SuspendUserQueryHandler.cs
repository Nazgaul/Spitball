using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Query;
using Cloudents.Infrastructure.Database.Query;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


using Cloudents.Core.Interfaces;

using NHibernate.Criterion;



namespace Cloudents.Infrastructure.Database
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class SuspendUserQueryHandler : IQueryHandler<UserDataByIdQuery, SuspendUserDto>
    {
        private readonly ISession _session;

        public SuspendUserQueryHandler(ReadonlySession session)
        {
            _session = session.Session;
        }

        public async Task<SuspendUserDto> GetAsync(UserDataByIdQuery query, CancellationToken token)
        {


            var answerQuery = _session.Query<Answer>();
            
            var futureAnswersData = answerQuery.Where(w => w.User.Id == query.Id)
                    .Select(s => s.Id)
                    .ToFuture();

            var futureQuestionsData = _session.Query<Question>()
               .Where(w => w.User.Id == query.Id)
               .Select(s => s.Id)
               .ToFuture();

            var futureQuestionsAnsweredData = _session.Query<Question>()
                .Where(w => w.CorrectAnswer.User.Id == query.Id)
                .Select(s => s.Id)
                .ToFuture();

            var dto = new SuspendUserDto();
         
            dto.AnswersData = futureAnswersData.GetEnumerable();
            dto.Questions = futureQuestionsData.GetEnumerable();
            dto.QuestionsAnsweredData = futureQuestionsAnsweredData.GetEnumerable();

            return dto;
        }
    }
}
