using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.Type;
using System;
using System.Data.Common;
using System.Globalization;

namespace Cloudents.Persistence
{
    [Serializable]
    public class MinuteTimeSpanType : TimeSpanType
    {

        public override string Name
        {
            get { return "MinuteTimeSpanType"; }
        }


        public override void Set(DbCommand st, object value, int index, ISessionImplementor session)
        {
            st.Parameters[index].Value = ((TimeSpan)value).TotalMinutes;
        }

        public override object Get(DbDataReader rs, int index, ISessionImplementor session)
        {
            try
            {

                return TimeSpan.FromMinutes(Convert.ToInt64(rs[index]));
            }
            catch (Exception ex)
            {
                throw new FormatException(string.Format("Input string '{0}' was not in the correct format.", rs[index]), ex);
            }
        }

        public override object Get(DbDataReader rs, string name, ISessionImplementor session)
        {
            try
            {
                return TimeSpan.FromMinutes(Convert.ToInt64(rs[name]));
            }
            catch (Exception ex)
            {
                throw new FormatException(string.Format("Input string '{0}' was not in the correct format.", rs[name]), ex);
            }
        }

        public override string ObjectToSQLString(object value, Dialect dialect)
        {
            return '\'' + ((TimeSpan)value).TotalMinutes.ToString(CultureInfo.InvariantCulture) + '\'';
        }
    }


}
