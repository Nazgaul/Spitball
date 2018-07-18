using System.Text;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Engine;
using NHibernate.SqlCommand;

namespace Cloudents.Infrastructure.Data
{
    public class FullTextCriterion : ICriterion
    {
        private readonly ProjectionAsCriterion _criterion;

        public FullTextCriterion(PropertyProjection property, string text)
        {
            var sb = new StringBuilder(text);
            if (text.Contains(" "))
            {
                sb.Insert(0, '"');
                sb.Append('"');
            }

            sb.Replace("'", "''");
            var projection = Projections.SqlFunction("FullTextContains",
                NHibernateUtil.Boolean,
                property, Projections.Constant(sb.ToString(), NHibernateUtil.String));

            _criterion = new ProjectionAsCriterion(projection);
        }

        public SqlString ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery)
        {
            return _criterion.ToSqlString(criteria, criteriaQuery);
        }

        public TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
        {
            return _criterion.GetTypedValues(criteria, criteriaQuery);
        }

        public IProjection[] GetProjections()
        {
            return _criterion.GetProjections();
        }
    }
}