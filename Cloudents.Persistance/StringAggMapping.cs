using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using NHibernate;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace Cloudents.Persistence
{
    public class StringAggMapping : IUserType
    {

        public bool Equals(object x, object y)
        {
            if (object.ReferenceEquals(x, y))
                return true;

            if (x == null || y == null)
                return false;

            return x.Equals(y);
        }

        public int GetHashCode(object x)
        {
            if (x == null)
                return 0;

            return x.GetHashCode();
        }

        public object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
        {
            if (names.Length == 0)
                throw new ArgumentException("Expecting at least one column");
            var val = (string)NHibernateUtil.String.NullSafeGet(rs, names, session, owner);
            
            return val?.Split(new [] {','},StringSplitOptions.RemoveEmptyEntries);
        }

        public void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
        {
            throw new NotImplementedException();
        }

        public object DeepCopy(object value)
        {
            return value;
        }

        public object Replace(object original, object target, object owner)
        {
            return original;
        }

        public object Assemble(object cached, object owner)
        {
            return cached;
        }

        public object Disassemble(object value)
        {
            return value;
        }

        public SqlType[] SqlTypes => new SqlType[] { new SqlType(DbType.String) };
        public Type ReturnedType => typeof(IEnumerable<string>);
        public bool IsMutable => false;
    }
}