using System;
using System.Collections;
using System.Collections.Generic;
using Cloudents.Core.DTOs.SearchSync;
using NHibernate.Transform;

namespace Cloudents.Infrastructure.Database
{
    public class AzureSyncBaseDtoTransformer<T,TU> : IResultTransformer where T : AzureSyncBaseDto<TU>,new() where TU : new()
    {

        public object TransformTuple(object[] tuple, string[] aliases)
        {
            var dic = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            for (var i = 0; i < aliases.Length; i++)
            {
                dic[aliases[i]] = tuple[i];
            }

            var x = new T {Data = new TU()};
            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                if (dic.TryGetValue(propertyInfo.Name, out var t))
                {
                    propertyInfo.SetValue(x, t);
                }
            }

            foreach (var propertyInfo in typeof(TU).GetProperties())
            {
                if (!dic.TryGetValue(propertyInfo.Name, out var value)) continue;
                if (value == null) continue;
                if (propertyInfo.PropertyType.IsEnum && value is string str)
                {
                    var e = Enum.Parse(propertyInfo.PropertyType, str, true);
                    propertyInfo.SetValue(x.Data, e);
                    continue;
                }

                var nullableType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
                if (nullableType != null)
                {
                    var y = Convert.ChangeType(value, nullableType);
                    propertyInfo.SetValue(x.Data, y);
                }
                else
                {
                    var y = Convert.ChangeType(value, propertyInfo.PropertyType);
                    propertyInfo.SetValue(x.Data, y);
                }
            }
            return x;
        }

        public IList TransformList(IList collection)
        {
            return collection;
        }
    }
}