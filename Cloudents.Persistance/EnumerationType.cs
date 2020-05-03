using Cloudents.Core;
using NHibernate;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using System;
using System.Data;
using System.Data.Common;

namespace Cloudents.Persistence
{
    [Serializable]
    public class EnumerationType<T> : IUserType where T : Enumeration
    {
       bool IUserType.Equals(object x, object y)
        {
            if (ReferenceEquals(x, y))
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

            var id = (int?)NHibernateUtil.Int32.NullSafeGet(rs, names[0], session);

            if (id.HasValue)
            {
                return Enumeration.FromValue<T>(id.Value);
            }

            return null;




            //return EnumerationExtensions.GetAll<T>().SingleOrDefault(s => Equals(s.Id, id));


            // here you can grab your data from external service

            //return val;
        }

        public void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
        {
            var parameter = cmd.Parameters[index];

            if (value == null)
            {
                parameter.Value = DBNull.Value;
            }
            else
            {
                parameter.Value = ((Enumeration)value).Id;
            }
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

        public SqlType[] SqlTypes => new[] { new SqlType(DbType.Int32) };
        public Type ReturnedType => typeof(int);
        public bool IsMutable => false;
    }
}