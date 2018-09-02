using NHibernate;
using NHibernate.Engine;
using NHibernate.Event.Default;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using System.Collections.Generic;
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