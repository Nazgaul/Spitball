using System;
using System.Data.Common;
using Cloudents.Application.Interfaces;
using Newtonsoft.Json;
using NHibernate;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.UserTypes;

namespace Cloudents.Infrastructure.Database
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



    [Serializable]
    public class JsonType<TSerializable> : MutableType
    {
        private readonly Type _serializableClass;
        private readonly StringClobType _dbType;

        public JsonType() : base(new StringClobSqlType())
        {
            _serializableClass = typeof(TSerializable);
            _dbType = NHibernateUtil.StringClob;
        }


        public override void Set(DbCommand cmd, object value, int index, ISessionImplementor session)
        {
            _dbType.Set(cmd, Serialize(value), index, session);
        }

        public override object Get(DbDataReader rs, int index, ISessionImplementor session)
        {
            var dbValue = (string)_dbType.Get(rs, index, session);
            return string.IsNullOrEmpty(dbValue) ? null : Deserialize(dbValue);
        }

        public override object Get(DbDataReader rs, string name, ISessionImplementor session)
        {
            return Get(rs, rs.GetOrdinal(name), session);
        }

        public override Type ReturnedClass => _serializableClass;

        public override bool IsEqual(object x, object y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x == null | y == null)
            {
                return false;
            }

            return x.Equals(y) | _dbType.IsEqual(Serialize(x), Serialize(y));
        }

        public override int GetHashCode(object x, ISessionFactoryImplementor factory)
        {
            return _dbType.GetHashCode(x, factory);
        }
       

        public override string ToString(object value)
        {
            return Serialize(value);
        }

        public override object FromStringValue(string xml)
        {
            return Deserialize((string)_dbType.FromStringValue(xml));
        }

        private static string Alias => string.Concat("json_", typeof(TSerializable).Name);

        public override string Name => Alias;

        public override object DeepCopyNotNull(object value)
        {
            return Deserialize(Serialize(value));
        }

        private string Serialize(object obj)
        {
            try
            {
                return JsonConvert.SerializeObject(obj);
            }
            catch (Exception e)
            {
                throw new SerializationException("Could not serialize a serializable property: ", e);
            }
        }

        public object Deserialize(string dbValue)
        {
            try
            {
                return JsonConvert.DeserializeObject(dbValue, _serializableClass);
            }
            catch (Exception e)
            {
                throw new SerializationException("Could not deserialize a serializable property: ", e);
            }
        }

        public override object Assemble(object cached, ISessionImplementor session, object owner)
        {
            return (cached == null) ? null : Deserialize((string)cached);
        }

        public override object Disassemble(object value, ISessionImplementor session, object owner)
        {
            return (value == null) ? null : Serialize(value);
        }
    }
}