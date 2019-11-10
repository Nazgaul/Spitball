using NHibernate;
using NHibernate.SqlCommand;
using System;
using System.Diagnostics;

namespace Cloudents.Persistence
{
    [Serializable]
    public class LoggingInterceptor : EmptyInterceptor
    {
        public override SqlString OnPrepareStatement(SqlString sql)
        {
            Debug.WriteLine(sql, "nhibernate");
            return base.OnPrepareStatement(sql);
        }
    }
}