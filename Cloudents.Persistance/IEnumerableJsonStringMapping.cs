using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NHibernate;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace Cloudents.Persistence
{
    public class IEnumerableJsonStringMapping : IUserType
    {
        bool IUserType.Equals(object x, object y)
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
            if (val == null)
            {
                return null;
            }
            var jObject = JArray.Parse(val);
            return jObject.Children().Select(s => (string)s.First).ToList();


            //return val?.Split(new [] { "+-+-+-" },StringSplitOptions.RemoveEmptyEntries);
        }

        public void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
        {
            if (value == null)
            {
                NHibernateUtil.String.NullSafeSet(cmd, null, index, session);
                return;
            }
            if (value is IEnumerable<string> p)
            {
            var data = JsonConvert.SerializeObject(p.Select(s=> new { name = s }));
            NHibernateUtil.String.NullSafeSet(cmd, data, index, session);
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

        public SqlType[] SqlTypes => new SqlType[] { new SqlType(DbType.String) };
        public Type ReturnedType => typeof(IEnumerable<string>);
        public bool IsMutable => false;
    }
}