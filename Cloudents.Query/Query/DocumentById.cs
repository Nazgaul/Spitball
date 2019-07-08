using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Query.Chat;
using Cloudents.Query.Stuff;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;

namespace Cloudents.Query.Query
{
    public class DocumentById : IQuery<DocumentDetailDto>
    {
        public DocumentById(long id, long? userId)
        {
            Id = id;
            UserId = userId;
        }



        public long Id { get; }
        private long? UserId { get; }


        internal sealed class DocumentByIdQueryHandler : IQueryHandler<DocumentById, DocumentDetailDto>
        {
            private readonly IStatelessSession _session;

            public DocumentByIdQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }
            public async Task<DocumentDetailDto> GetAsync(DocumentById query, CancellationToken token)
            {

               // Core.Entities.Tutor tutorAlias = null;
                //BaseUser userAlias = null;
                Document documentAlias = null;
                ViewUserDetail userAlias = null;
                University universityAlias = null;
                DocumentDetailDto dtoAlias = null;

                var futureValue = _session.QueryOver<Document>(() => documentAlias)
                    //.JoinAlias(x => x.User, () => userAlias)
                    .JoinAlias(x => x.University, () => universityAlias)
                    .JoinEntityAlias(() => userAlias, () => documentAlias.User.Id == userAlias.Id, JoinType.InnerJoin)
                    .Where(w => w.Id == query.Id && w.Status.State == ItemState.Ok)
                    .SelectList(l =>
                        l.Select(() => documentAlias.Id).WithAlias(() => dtoAlias.Id)
                            .Select(() => documentAlias.Name).WithAlias(() => dtoAlias.Name)
                            .Select(() => documentAlias.TimeStamp.UpdateTime).WithAlias(() => dtoAlias.Date)
                            .Select(() => universityAlias.Name).WithAlias(() => dtoAlias.University)
                            .Select(() => documentAlias.Type).WithAlias(() => dtoAlias.Type)
                            .Select(() => documentAlias.PageCount).WithAlias(() => dtoAlias.Pages)
                            .Select(() => documentAlias.Professor).WithAlias(() => dtoAlias.Professor)
                            .Select(() => documentAlias.Views).WithAlias(() => dtoAlias.Views)
                            //.Select(() => documentAlias.Downloads).WithAlias(() => dtoAlias.Downloads)
                            .Select(() => documentAlias.Price).WithAlias(() => dtoAlias.Price)
                            //.Select(() => documentAlias.PageCount).WithAlias(() => dtoAlias.PageCount)
                            .Select(() => documentAlias.Course.Id).WithAlias(() => dtoAlias.Course)

                            .Select(Projections.Property(() => userAlias.Id).As("User.Id"))
                            .Select(Projections.Property(() => userAlias.Name).As("User.Name"))
                            .Select(Projections.Property(() => userAlias.Image).As("User.Image"))
                            .Select(Projections.Property(() => userAlias.Score).As("User.Score"))
                            .Select(Projections.Property(() => userAlias.Courses).As("User.Courses"))
                            .Select(Projections.Property(() => userAlias.Price).As("User.Price"))
                            .Select(Projections.Property(() => userAlias.IsTutor).As("User.IsTutor"))
                            .Select(Projections.Property(() => userAlias.Rate).As("User.Rate"))
                            .Select(Projections.Property(() => userAlias.Bio).As("User.Bio"))
                            .Select(Projections.Property(() => userAlias.ReviewsCount).As("User.ReviewsCount"))


                    )
                    .TransformUsing(new DeepTransformer<DocumentDetailDto>())
                    .FutureValue<DocumentDetailDto>();

                //var futureValue = _session.Query<Document>()

                //   .Fetch(f => f.University)
                //   .Fetch(f => f.User)
                //   .Where(w => w.Id == query.Id && w.Status.State == ItemState.Ok)
                //   .Select(s => new DocumentDetailDto
                //   {
                //       Id = s.Id,
                //       Name = s.Name,
                //       Date = s.TimeStamp.UpdateTime,
                //       University = s.University.Name,
                //       Type = s.Type,
                //       Pages = s.PageCount.GetValueOrDefault(),
                //       Professor = s.Professor,
                //       Views = s.Views,
                //       Downloads = s.Downloads,
                //       Price = s.Price,
                //       PageCount = s.PageCount.GetValueOrDefault(),
                //       User = new UserDto
                //       {
                //           Id = s.User.Id,
                //           Name = s.User.Name,
                //           Image = s.User.Image,
                //           Score = s.User.Score
                //       },
                //       Course = s.Course.Id
                //   }).ToFutureValue();

                    
                if (!query.UserId.HasValue)
                {
                    return await futureValue.GetValueAsync(token);
                }
                var purchaseFuture = _session.Query<DocumentTransaction>()
                       .Where(w => w.User.Id == query.UserId.Value && w.Document.Id == query.Id && w.Type == TransactionType.Spent)
                       .ToFutureValue();

                var result = await futureValue.GetValueAsync(token);
                if (result == null)
                {
                    return null;
                }
                result.IsPurchased = true;
                if (result.Price.GetValueOrDefault() <= 0) return result;
                if (purchaseFuture == null)
                {
                    result.IsPurchased = false;

                }
                else
                {
                    if (result.User.Id == query.UserId.Value)
                    {

                        result.IsPurchased = true;
                    }
                    else
                    {
                        var transactionResult = await purchaseFuture.GetValueAsync(token);
                        result.IsPurchased = transactionResult != null;
                    }
                }
                return result;
            }
        }
    }
}