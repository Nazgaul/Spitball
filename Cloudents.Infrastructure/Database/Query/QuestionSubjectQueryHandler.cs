using Cloudents.Core;
using Cloudents.Core.Attributes;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Database.Query
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class QuestionSubjectQueryHandler : IQueryHandler<QuestionSubjectQuery, IEnumerable<QuestionSubjectDto>>
    {
        private readonly IStatelessSession _session;

        public QuestionSubjectQueryHandler(ReadonlyStatelessSession session)
        {
            _session = session.Session;
        }

        //TODO: we can put attribute on culture hebrew text field and by reflection do the select

        private static Expression<Func<QuestionSubject, object>> GetFieldBaseOnCulture(CultureInfo info)
        {
            if (Equals(info, new CultureInfo("he")))
            {
                return subject => subject.TextHebrew;
            }
            else
            {
                return subject => subject.Text;
            }
        }

        [Cache(TimeConst.Hour, "QuestionSubject", false)]

        public async Task<IEnumerable<QuestionSubjectDto>> GetAsync(QuestionSubjectQuery query, CancellationToken token)
        {
            QuestionSubjectDto dto = null;
            return await _session.QueryOver<QuestionSubject>()


                 .OrderBy(o => o.Order).Asc
                 .ThenBy(GetFieldBaseOnCulture(query.CultureInfo)).Asc


                 .SelectList(l =>
                {
                    l.Select(s => s.Id).WithAlias(() => dto.Id);

                    l.Select(
                       Projections.SqlFunction("coalesce", NHibernateUtil.String,
                           Projections.Property(GetFieldBaseOnCulture(query.CultureInfo)),
                           Projections.Property<QuestionSubject>(p => p.Text)).WithAlias(() => dto.Subject)
                   );

                    return l;
                })
                .TransformUsing(Transformers.AliasToBean<QuestionSubjectDto>())
                .ListAsync<QuestionSubjectDto>(token).ConfigureAwait(false);
        }
    }
}