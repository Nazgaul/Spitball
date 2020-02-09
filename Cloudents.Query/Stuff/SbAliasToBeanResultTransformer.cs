using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using NHibernate.Transform;

namespace Cloudents.Query.Stuff
{
    public class SbAliasToBeanResultTransformer<T> : IResultTransformer
    {
        private readonly Type _resultClass;

        public SbAliasToBeanResultTransformer()
        {
            _resultClass = typeof(T);

        }

        public object TransformTuple(object[] tuple, string[] aliases)
        {
            var retVal = Activator.CreateInstance(_resultClass);

            for (int i = 0; i < aliases.Length; i++)
            {
                SetValues(aliases[i], tuple[i], retVal);
            }

            return retVal;
        }

        public IList TransformList(IList collection)
        {
            return collection;
        }


        private void SetValues(string alias, object value, object x)
        {
            //if (!dic.TryGetValue(propertyInfo.Name, out var value)) return;

            if (value == null) return;
            var propertyInfo = _resultClass.GetProperties().FirstOrDefault(w => string.Equals(w.Name, alias, StringComparison.OrdinalIgnoreCase));
            if (propertyInfo == null)
            {
                return;
            }
            if (propertyInfo.PropertyType.IsEnum)
            {
                if (HandleEnum(propertyInfo, propertyInfo.PropertyType, x, value)) return;
            }
            if (value is Guid g && propertyInfo.PropertyType == typeof(string))
            {
                propertyInfo.SetValue(x, g.ToString());
                return;

            }

            if (HandleTicksTimeSpan(value, x, propertyInfo)) return;
            var nullableType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
            if (nullableType != null)
            {
                if (nullableType.IsEnum && HandleEnum(propertyInfo, nullableType, x, value))
                {
                    return;
                }
                if (HandleTicksTimeSpan(value, x, propertyInfo)) return;
                var z = Convert.ChangeType(value, nullableType);
                propertyInfo.SetValue(x, z);
                return;
            }
            var y = Convert.ChangeType(value, propertyInfo.PropertyType);
            propertyInfo.SetValue(x, y);
        }

        private static bool HandleTicksTimeSpan(object value, object x, PropertyInfo propertyInfo)
        {
            var type = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
            if (type == typeof(TimeSpan) && value is long l)
            {
                propertyInfo.SetValue(x, new TimeSpan(l));
                return true;
            }

            return false;
        }

        private static bool HandleEnum(PropertyInfo propertyInfo, Type property, object x, object value)
        {
            if (value is string str)
            {
                var e = Enum.Parse(property, str, true);
                propertyInfo.SetValue(x, e);
                return true;
            }

            if (value is int i)
            {
                var t = Enum.ToObject(property, i);
                propertyInfo.SetValue(x, t);
                return true;
            }

            return false;
        }
    }
}