using Cloudents.Core.DTOs.Email;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Email
{
    public class GetUpdatesEmailByUserQuery : IQuery<IEnumerable<UpdateEmailDto>>
    {
        public GetUpdatesEmailByUserQuery(long userId, DateTime since)
        {
            UserId = userId;
            Since = since;
        }
        private long UserId { get; }
        private DateTime Since { get; }

        internal sealed class GetUpdatesEmailQuestionsQueryHandler : IQueryHandler<GetUpdatesEmailByUserQuery, IEnumerable<UpdateEmailDto>>
        {
            private readonly IStatelessSession _session;

            public GetUpdatesEmailQuestionsQueryHandler(IStatelessSession querySession)
            {
                _session = querySession;
            }

            public async Task<IEnumerable<UpdateEmailDto>> GetAsync(GetUpdatesEmailByUserQuery query, CancellationToken token)
            {
                User userAlias = null!;
                Course courseAlias = null!;

                var followCourse = QueryOver.Of<Follow>().Where(w => w.Follower.Id == query.UserId)
                    .Select(s => s.User.Id);


                DocumentUpdateEmailDto documentEmailDtoAlias = null!;


                var documentFuture = _session.QueryOver<Document>()
                    .JoinAlias(x => x.User, () => userAlias)
                    .JoinAlias(x=>x.Course,()=>courseAlias)
                    .Where(x => x.TimeStamp.CreationTime > query.Since)
                    .And(x => x.Status.State == ItemState.Ok)
                    .WithSubquery.WhereProperty(x => x.User.Id).In(followCourse)
                    .And(x => x.User.Id != query.UserId)


                    .SelectList(sl =>
                    {
                        sl.Select(x => x.Id).WithAlias(() => documentEmailDtoAlias.Id);
                        sl.Select(x => x.Name).WithAlias(() => documentEmailDtoAlias.Name);
                        sl.Select(() => userAlias.Name).WithAlias(() => documentEmailDtoAlias.UserName);
                        sl.Select(() => userAlias.Id).WithAlias(() => documentEmailDtoAlias.UserId);
                        sl.Select(() => courseAlias.Name).WithAlias(() => documentEmailDtoAlias.Course);
                        sl.Select(() => userAlias.ImageName).WithAlias(() => documentEmailDtoAlias.UserImage);
                        sl.Select(x => x.DocumentType).WithAlias(() => documentEmailDtoAlias.DocumentType);
                        return sl;
                    }).TransformUsing(Transformers.AliasToBean<DocumentUpdateEmailDto>())
                    .OrderBy(x=>x.DocumentType).Desc
                    .UnderlyingCriteria.SetComment(nameof(GetUpdatesEmailByUserQuery))
                    .Future<DocumentUpdateEmailDto>();

                IEnumerable<UpdateEmailDto> documents = await documentFuture.GetEnumerableAsync(token);

                return documents;
            }
        }
    }
}
