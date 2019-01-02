using System.Diagnostics;
using NHibernate;
using NHibernate.SqlCommand;

namespace Cloudents.Persistance
{
    public class LoggingInterceptor : EmptyInterceptor
    {
        public override SqlString OnPrepareStatement(SqlString sql)
        {
            Debug.WriteLine(sql, "nhibernate");
            return base.OnPrepareStatement(sql);
        }


    }
}