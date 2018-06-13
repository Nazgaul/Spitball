using NHibernate;
using NHibernate.Criterion;
using NHibernate.Dialect;
using NHibernate.Dialect.Function;
using NHibernate.Engine;
using NHibernate.SqlCommand;

namespace Cloudents.Infrastructure.Data
{
    public class SbDialect : MsSql2012Dialect
    {
        protected override void RegisterFunctions()
        {
            base.RegisterFunctions();
            RegisterFunction("freeText", new StandardSQLFunction("freetext", null));
            RegisterFunction("FullTextContains", new StandardSQLFunction("contains", null));
        }
    }


    //public class FullTextExpression : AbstractCriterion
    //{
    //    public FullTextExpression()
    //    {
            
    //    }

    //    public override string ToString()
    //    {
    //        throw new System.NotImplementedException();
    //    }

    //    public override SqlString ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery)
    //    {
    //        throw new System.NotImplementedException();
    //    }

    //    public override TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
    //    {
    //        throw new System.NotImplementedException();
    //    }

    //    public override IProjection[] GetProjections()
    //    {
    //        throw new System.NotImplementedException();
    //    }
    //}
}