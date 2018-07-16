using System;
using NHibernate.Dialect;
using NHibernate.Dialect.Function;
using NHibernate.Linq;

namespace Cloudents.Infrastructure.Data
{
    public class SbDialect : MsSql2012Dialect
    {
        protected override void RegisterFunctions()
        {
            base.RegisterFunctions();
            RegisterFunction("FullTextContains", new StandardSQLFunction("contains", null));
            //RegisterFunction("NEWID()", new StandardSQLFunction("NEWID()", NHibernateUtil.Guid));


        }
    }

    //public class SqlFunctions
    //{
    //    [LinqExtensionMethod("NEWID")] public static Guid NewID() { return Guid.NewGuid(); }
    //}
}