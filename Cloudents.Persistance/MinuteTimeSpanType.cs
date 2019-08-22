using NHibernate.Type;
using System;
using System.Collections;
using System.Data.Common;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using System.Collections.Generic;
using System.Data;
using NHibernate.Dialect;

namespace Cloudents.Persistence
{
    [Serializable]
    public class MinuteTimeSpanType : TimeSpanType
    {

        public MinuteTimeSpanType()
            : base()
        {
        }

        public override string Name
        {
            get { return "MinuteTimeSpanType"; }
        }


        public override void Set(DbCommand st, object value, int index, ISessionImplementor session)
        {
            st.Parameters[index].Value = ((TimeSpan)value).Minutes;
        }


        public override object Seed(ISessionImplementor session)
        {
            return new TimeSpan(DateTime.Now.Minute);
        }

        public override string ObjectToSQLString(object value, Dialect dialect)
        {
            return '\'' + ((TimeSpan)value).Minutes.ToString() + '\'';
        }

    }
}
