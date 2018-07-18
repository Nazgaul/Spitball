using System;
using System.Data.Common;
using Cloudents.Core.Interfaces;
using Newtonsoft.Json;
using NHibernate;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace Cloudents.Infrastructure.Data
{
    public static class TypeNameHelper
    {
        public static string GetSimpleTypeName(object obj)
        {
            return obj?.GetType().AssemblyQualifiedName;
        }

        public static Type GetType(string simpleTypeName)
        {
            return Type.GetType(simpleTypeName);
        }
    }

    public class JsonType : IUserType
    {
        public new bool Equals(object x, object y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }
            if (x is null || y is null)
            {
                return false;
            }

            return x.Equals(y);
        }

        public int GetHashCode(object x)
        {
            return x?.GetHashCode() ?? 0;
        }

        private static object Deserialize(string data, string type)
        {
            return Deserialize(data, TypeNameHelper.GetType(type));
        }

        private static object Deserialize(string data, Type type)
        {
            return JsonConvert.DeserializeObject(data, type);
        }

        private static string Serialize(object value)
        {
            return value == null
                ? null
                : JsonConvert.SerializeObject(value);
        }

        private static string GetType(object value)
        {
            return value == null
                ? null
                : TypeNameHelper.GetSimpleTypeName(value);
        }

        public object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
        {
            var typeIndex = rs.GetOrdinal(names[0]);
            var dataIndex = rs.GetOrdinal(names[1]);
            if (rs.IsDBNull(typeIndex) || rs.IsDBNull(dataIndex))
            {
                return null;
            }

            var type = (string)rs.GetValue(typeIndex);
            var data = (string)rs.GetValue(dataIndex);
            return Deserialize(data, type);
        }

        public void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
        {
            if (value == null)
            {
                NHibernateUtil.String.NullSafeSet(cmd, null, index,session);
                NHibernateUtil.String.NullSafeSet(cmd, null, index + 1, session);
                return;
            }

            var type = GetType(value);
            var data = Serialize(value);
            NHibernateUtil.String.NullSafeSet(cmd, type, index, session);
            NHibernateUtil.String.NullSafeSet(cmd, data, index + 1, session);
        }

        public object DeepCopy(object value)
        {
            return value == null
                ? null
                : Deserialize(Serialize(value), GetType(value));
        }

        public object Replace(object original, object target, object owner)
        {
            return original;
        }

        public object Assemble(object cached, object owner)
        {
            return !(cached is string[] parts)
                ? null
                : Deserialize(parts[1], parts[0]);
        }

        public object Disassemble(object value)
        {
            return (value == null)
                ? null
                : new[]
                {
                    GetType(value),
                    Serialize(value)
                };
        }

        public SqlType[] SqlTypes { get; } = new SqlType[]
        {
            SqlTypeFactory.GetString(10000), // Type
            SqlTypeFactory.GetStringClob(10000) // Data
        };

        public Type ReturnedType => typeof(ICommand);
        public bool IsMutable => false;
    }
}