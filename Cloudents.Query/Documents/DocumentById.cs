using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Query.Stuff;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.SqlCommand;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Attributes;

namespace Cloudents.Query.Documents
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
            [Cache(TimeConst.Minute*2,"document-by-id",false)]
            public async Task<DocumentDetailDto> GetAsync(DocumentById query, CancellationToken token)
            {

                Document documentAlias = null;
                ReadTutor tutorAlias = null;
                University universityAlias = null;
                BaseUser userAlias = null;
                DocumentDetailDto dtoAlias = null;


                var futureValue = _session.QueryOver(() => documentAlias)
                    .JoinAlias(x => x.University, () => universityAlias)
                    .JoinAlias(x => x.User, () => userAlias)
                    .JoinEntityAlias(() => tutorAlias, () => documentAlias.User.Id == tutorAlias.Id, JoinType.LeftOuterJoin)
                    .Where(w => w.Id == query.Id && w.Status.State == ItemState.Ok)
                    .SelectList(l =>
                        l.Select(() => documentAlias.PageCount).WithAlias(() => dtoAlias.Pages)
                            .Select(Projections.Property(() => documentAlias.Id).As($"{nameof(DocumentDetailDto.Document)}.{nameof(DocumentFeedDto.Id)}"))
                            .Select(Projections.Property(() => documentAlias.Name).As($"{nameof(DocumentDetailDto.Document)}.{nameof(DocumentFeedDto.Title)}"))
                            .Select(Projections.Property(() => documentAlias.TimeStamp.UpdateTime).As($"{nameof(DocumentDetailDto.Document)}.{nameof(DocumentFeedDto.DateTime)}"))
                            .Select(Projections.Property(() => universityAlias.Name).As($"{nameof(DocumentDetailDto.Document)}.{nameof(DocumentFeedDto.University)}"))
                            .Select(Projections.Property(() => documentAlias.Views).As($"{nameof(DocumentDetailDto.Document)}.{nameof(DocumentFeedDto.Views)}"))
                            .Select(Projections.Property(() => documentAlias.Price).As($"{nameof(DocumentDetailDto.Document)}.{nameof(DocumentFeedDto.Price)}"))
                            .Select(Projections.Property(() => documentAlias.Course.Id).As($"{nameof(DocumentDetailDto.Document)}.{nameof(DocumentFeedDto.Course)}"))
                            .Select(Projections.Property(() => documentAlias.Downloads).As($"{nameof(DocumentDetailDto.Document)}.{nameof(DocumentFeedDto.Downloads)}"))
                            .Select(Projections.Property(() => documentAlias.VoteCount).As($"{nameof(DocumentDetailDto.Document)}.{nameof(DocumentFeedDto.Vote)}.{nameof(VoteDto.Votes)}"))
                            .Select(Projections.SqlFunction("COALESCE", NHibernateUtil.String
                               , Projections.Property<Document>(documentAlias2 => documentAlias2.Description)
                               , Projections.Property<Document>(documentAlias2 => documentAlias2.MetaContent))
                            .WithAlias($"{nameof(DocumentDetailDto.Document)}.{nameof(DocumentFeedDto.Snippet)}"))
                            .Select(Projections.Property(() => userAlias.Id).As($"{nameof(DocumentDetailDto.Document)}.{nameof(DocumentFeedDto.User)}.{nameof(DocumentUserDto.Id)}"))
                            .Select(Projections.Property(() => userAlias.Name).As($"{nameof(DocumentDetailDto.Document)}.{nameof(DocumentFeedDto.User)}.{nameof(DocumentUserDto.Name)}"))
                            .Select(Projections.Property(() => userAlias.Image).As($"{nameof(DocumentDetailDto.Document)}.{nameof(DocumentFeedDto.User)}.{nameof(DocumentUserDto.Image)}"))
                            .Select(Projections.Property(() => documentAlias.DocumentType).As($"{nameof(DocumentDetailDto.Document)}.{nameof(DocumentFeedDto.DocumentType)}"))
                            .Select(Projections.Property(() => tutorAlias.Id).As($"{nameof(DocumentDetailDto.Tutor)}.{nameof(TutorCardDto.UserId)}"))
                            .Select(Projections.Property(() => tutorAlias.SubsidizedPrice).As($"{nameof(DocumentDetailDto.Tutor)}.{nameof(TutorCardDto.DiscountPrice)}"))
                            .Select(Projections.Property(() => tutorAlias.Name).As($"{nameof(DocumentDetailDto.Tutor)}.{nameof(TutorCardDto.Name)}"))
                            .Select(Projections.Property(() => tutorAlias.Image).As($"{nameof(DocumentDetailDto.Tutor)}.{nameof(TutorCardDto.Image)}"))
                            .Select(Projections.Property(() => tutorAlias.Courses).As($"{nameof(DocumentDetailDto.Tutor)}.{nameof(TutorCardDto.Courses)}"))
                            .Select(Projections.Property(() => tutorAlias.Subjects).As($"{nameof(DocumentDetailDto.Tutor)}.{nameof(TutorCardDto.Subjects)}"))
                            .Select(Projections.Property(() => tutorAlias.Price).As($"{nameof(DocumentDetailDto.Tutor)}.{nameof(TutorCardDto.Price)}"))
                            .Select(Projections.Property(() => tutorAlias.Rate).As($"{nameof(DocumentDetailDto.Tutor)}.{nameof(TutorCardDto.Rate)}"))
                            .Select(Projections.Property(() => tutorAlias.RateCount).As($"{nameof(DocumentDetailDto.Tutor)}.{nameof(TutorCardDto.ReviewsCount)}"))
                            .Select(Projections.Property(() => tutorAlias.Bio).As($"{nameof(DocumentDetailDto.Tutor)}.{nameof(TutorCardDto.Bio)}"))
                            .Select(Projections.Property(() => tutorAlias.University).As($"{nameof(DocumentDetailDto.Tutor)}.{nameof(TutorCardDto.University)}"))
                            .Select(Projections.Property(() => tutorAlias.Lessons).As($"{nameof(DocumentDetailDto.Tutor)}.{nameof(TutorCardDto.Lessons)}"))
                            .Select(Projections.Property(() => tutorAlias.Country).As($"{nameof(DocumentDetailDto.Tutor)}.{nameof(TutorCardDto.Country)}"))


                    )
                    .TransformUsing(new DeepTransformer<DocumentDetailDto>())
                    .FutureValue<DocumentDetailDto>();


                if (!query.UserId.HasValue)
                {
                    return await futureValue.GetValueAsync(token);
                }
                var purchaseFuture = _session.Query<DocumentTransaction>()
                       .Where(w => w.User.Id == query.UserId.Value && w.Document.Id == query.Id && w.Type == TransactionType.Spent)
                       .ToFutureValue();

                var purchaseCountFuture = _session.QueryOver<DocumentTransaction>()
                    .Where(w => w.Document.Id == query.Id && w.Type == TransactionType.Spent)
                    .SelectList(s => s.SelectCount(c => c.Id)).FutureValue<int>();

                var voteQuery = _session.Query<Vote>().Where(w => w.User.Id == query.UserId && w.Document.Id == query.Id).Select(s => s.VoteType).Take(1).ToFutureValue();

                var result = await futureValue.GetValueAsync(token);

               
                if (result == null)
                {
                    return null;
                }
                result.IsPurchased = true;
                var voteResult = await voteQuery.GetValueAsync(token);
                if (voteResult == VoteType.None)
                {
                    result.Document.Vote.Vote = null;
                }
                else
                {
                    result.Document.Vote.Vote = voteResult;
                }
                if (result.Document.Price.GetValueOrDefault() <= 0) return result;
                if (purchaseFuture == null)
                {
                    result.IsPurchased = false;

                }
                else
                {
                    if (result.Document.User.Id == query.UserId.Value)
                    {
                        result.IsPurchased = true;
                    }
                    else
                    {
                        var transactionResult = await purchaseFuture.GetValueAsync(token);
                        result.IsPurchased = transactionResult != null;
                    }
                }
                result.Document.Purchased = await purchaseCountFuture.GetValueAsync(token);
                return result;
            }
        }
    }
}