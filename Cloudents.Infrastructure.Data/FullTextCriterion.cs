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


       private static string ChangeTextForFullText(string str)
        {
            var spaceExists = false;
            char[] ca = str.ToCharArray();
            StringBuilder sb = new StringBuilder(str.Length * 5 / 4);
            for (var i = 0; i <= ca.Length - 1; i++)
            {
                if (ca[i] == '(' || ca[i] == ')')
                {
                    continue;
                }
                if (ca[i] == '\'')
                {
                    sb.Append('\'');
                }
                sb.Append(ca[i]);
                if (ca[i] == ' ')
                {
                    spaceExists = true;
                }
            }

            if (spaceExists)
            {
                sb.Insert(0, '"');
                sb.Append('"');
            }
            return sb.ToString();
        }

        public FullTextCriterion(PropertyProjection property, string text)
        {
            var projection = Projections.SqlFunction("FullTextContains",
                NHibernateUtil.Boolean,
                property, Projections.Constant(ChangeTextForFullText(text), NHibernateUtil.String));

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