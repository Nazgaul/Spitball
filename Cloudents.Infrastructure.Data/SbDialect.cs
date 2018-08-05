using NHibernate;
using NHibernate.Dialect;
using NHibernate.Dialect.Function;

namespace Cloudents.Infrastructure.Data
{
    public class SbDialect : MsSql2012Dialect
    {
        internal const string RandomOrder = "random_Order";

        protected override void RegisterFunctions()
        {
            base.RegisterFunctions();
            RegisterFunction("FullTextContains", new StandardSQLFunction("contains", null));

            RegisterFunction(RandomOrder, new StandardSQLFunction("NEWID", NHibernateUtil.Guid));
            //RegisterFunction("NEWID()", new StandardSQLFunction("NEWID()", NHibernateUtil.Guid));

        }
    }

    public class MySqliteDialect : SQLiteDialect
    {
        protected override void RegisterFunctions()
        {
            base.RegisterFunctions();
            RegisterFunction(SbDialect.RandomOrder, new StandardSQLFunction("random", NHibernateUtil.Guid));
        }
    }

    //public class SqlFunctions
    //{
    //    [LinqExtensionMethod("NEWID")] public static Guid NewID() { return Guid.NewGuid(); }
    //}
}