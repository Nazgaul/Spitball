using System;
using System.Data;
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
            return null == obj
                ? null
                : obj.GetType().AssemblyQualifiedName;
        }


        public static Type GetType(string simpleTypeName)
        {
            return Type.GetType(simpleTypeName);
        }


    }
    public class JsonType : IUserType
    {
        public bool Equals(object x, object y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }
            if (ReferenceEquals(null, x) || ReferenceEquals(null, y))
            {
                return false;
            }


            return x.Equals(y);

        }



        public int GetHashCode(object x)
        {
            return (x == null) ? 0 : x.GetHashCode();
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
            return null == value
                ? null
                : JsonConvert.SerializeObject(value);
        }
        private static string GetType(object value)
        {
            return null == value
                ? null
                : TypeNameHelper.GetSimpleTypeName(value);
        }

       

        

        public object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
        {
            int typeIndex = rs.GetOrdinal(names[0]);
            int dataIndex = rs.GetOrdinal(names[1]);
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
            var parts = cached as string[];
            return parts == null
                ? null
                : Deserialize(parts[1], parts[0]);
        }

        public object Disassemble(object value)
        {
            return (value == null)
                ? null
                : new string[]
                {
                    GetType(value),
                    Serialize(value)
                };
        }

        public SqlType[] SqlTypes => new SqlType[]
        {
            SqlTypeFactory.GetString(10000), // Type
            SqlTypeFactory.GetStringClob(10000) // Data
        };

        public Type ReturnedType => typeof(ICommand);
        public bool IsMutable => false;
    }
}