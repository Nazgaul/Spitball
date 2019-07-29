﻿using Cloudents.Core.DTOs;
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

                Document documentAlias = null;
                ViewTutor tutorAlias = null;
                University universityAlias = null;
                BaseUser userAlias = null;
                DocumentDetailDto dtoAlias = null;

                var futureValue = _session.QueryOver(() => documentAlias)
                    .JoinAlias(x => x.University, () => universityAlias)
                    .JoinAlias(x => x.User, () => universityAlias)

                    .JoinEntityAlias(() => tutorAlias, () => documentAlias.User.Id == tutorAlias.Id, JoinType.LeftOuterJoin)
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
                            .Select(() => documentAlias.Price).WithAlias(() => dtoAlias.Price)
                            .Select(() => documentAlias.Course.Id).WithAlias(() => dtoAlias.Course)
                            .Select(() => documentAlias.User.Id).WithAlias(() => dtoAlias.UploaderId)
                            .Select(() => userAlias.Name).WithAlias(() => dtoAlias.UploaderName)

                            .Select(Projections.Property(() => tutorAlias.Id).As($"{nameof(DocumentDetailDto.User)}.{nameof(TutorCardDto.UserId)}"))
                            .Select(Projections.Property(() => tutorAlias.Name).As($"{nameof(DocumentDetailDto.User)}.{nameof(TutorCardDto.Name)}"))
                            .Select(Projections.Property(() => tutorAlias.Image).As($"{nameof(DocumentDetailDto.User)}.{nameof(TutorCardDto.Image)}"))
                            .Select(Projections.Property(() => tutorAlias.Courses).As($"{nameof(DocumentDetailDto.User)}.{nameof(TutorCardDto.Courses)}"))
                            .Select(Projections.Property(() => tutorAlias.Subjects).As($"{nameof(DocumentDetailDto.User)}.{nameof(TutorCardDto.Subjects)}"))
                            .Select(Projections.Property(() => tutorAlias.CourseCount).As($"{nameof(DocumentDetailDto.User)}.{nameof(TutorCardDto.CourseCount)}"))
                            .Select(Projections.Property(() => tutorAlias.Price).As($"{nameof(DocumentDetailDto.User)}.{nameof(TutorCardDto.Price)}"))
                            .Select(Projections.Property(() => tutorAlias.Rate).As($"{nameof(DocumentDetailDto.User)}.{nameof(TutorCardDto.Rate)}"))
                            .Select(Projections.Property(() => tutorAlias.SumRate).As($"{nameof(DocumentDetailDto.User)}.{nameof(TutorCardDto.ReviewsCount)}"))
                            .Select(Projections.Property(() => tutorAlias.Bio).As($"{nameof(DocumentDetailDto.User)}.{nameof(TutorCardDto.Bio)}"))
                            .Select(Projections.Property(() => tutorAlias.University).As($"{nameof(DocumentDetailDto.User)}.{nameof(TutorCardDto.University)}"))
                            .Select(Projections.Property(() => tutorAlias.Lessons).As($"{nameof(DocumentDetailDto.User)}.{nameof(TutorCardDto.Lessons)}"))


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
                    if (result.UploaderId == query.UserId.Value)
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