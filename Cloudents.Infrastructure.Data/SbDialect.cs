using NHibernate.Dialect;
using NHibernate.Dialect.Function;

namespace Cloudents.Infrastructure.Data
{
    public class SbDialect : MsSql2012Dialect
    {
        protected override void RegisterFunctions()
        {
            base.RegisterFunctions();
            //RegisterFunction("freeText", new StandardSQLFunction("freetext", null));
            RegisterFunction("FullTextContains", new StandardSQLFunction("contains", null));
        }
    }

}