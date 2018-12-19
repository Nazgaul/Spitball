using NHibernate;
using NHibernate.SqlCommand;
using System.Diagnostics;

namespace Cloudents.Infrastructure.Database
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