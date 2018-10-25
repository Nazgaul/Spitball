using Cloudents.Core.DTOs.SearchSync;
using NHibernate.Transform;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Cloudents.Infrastructure.Database
{
    public class AzureSyncBaseDtoTransformer<T, TU> : IResultTransformer where T : AzureSyncBaseDto<TU>, new() where TU : new()
    {

        public object TransformTuple(object[] tuple, string[] aliases)
        {
            var dic = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            for (var i = 0; i < aliases.Length; i++)
            {
                dic[aliases[i]] = tuple[i];
            }

            var x = new T { Data = new TU() };
            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                SetValues(dic, propertyInfo, x);
            }

            foreach (var propertyInfo in typeof(TU).GetProperties())
            {
                SetValues(dic, propertyInfo, x.Data);
            }
            return x;
        }

        private static void SetValues(IReadOnlyDictionary<string, object> dic, PropertyInfo propertyInfo, object x)
        {
            if (!dic.TryGetValue(propertyInfo.Name, out var value)) return;
            if (value == null) return;
            if (propertyInfo.PropertyType.IsEnum)
            {
                if (value is string str)
                {
                    var e = Enum.Parse(propertyInfo.PropertyType, str, true);
                    propertyInfo.SetValue(x, e);
                    return;
                }

                if (value is int i)
                {

                    var t = Enum.ToObject(propertyInfo.PropertyType, i);
                    propertyInfo.SetValue(x, t);
                    return;
                }
            }
            if (value is Guid g && propertyInfo.PropertyType == typeof(string))
            {
                propertyInfo.SetValue(x, g.ToString());
                return;

            }
            var nullableType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
            if (nullableType != null)
            {
                var z = Convert.ChangeType(value, nullableType);
                propertyInfo.SetValue(x, z);
                return;
            }
            var y = Convert.ChangeType(value, propertyInfo.PropertyType);
            propertyInfo.SetValue(x, y);
        }

        public IList TransformList(IList collection)
        {
            return collection;
        }
    }
}