//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Reflection;
//using Cloudents.Core.DTOs.SearchSync;
//using NHibernate.Transform;

//namespace Cloudents.Persistence
//{
//    public class AzureSyncBaseDtoTransformer<T, TU> : IResultTransformer where T : AzureSyncBaseDto<TU>, new() where TU : new()
//    {

//        public object TransformTuple(object[] tuple, string[] aliases)
//        {
//            var dic = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
//            for (var i = 0; i < aliases.Length; i++)
//            {
//                dic.Add(aliases[i], tuple[i]);

//            }

//            var x = new T { Data = new TU() };
//            foreach (var propertyInfo in typeof(T).GetProperties())
//            {
//                SetValues(dic, propertyInfo, x);
//            }

//            foreach (var propertyInfo in typeof(TU).GetProperties())
//            {
//                SetValues(dic, propertyInfo, x.Data);
//            }
//            return x;
//        }

//        private static void SetValues(IReadOnlyDictionary<string, object> dic, PropertyInfo propertyInfo, object x)
//        {
//            if (!dic.TryGetValue(propertyInfo.Name, out var value)) return;
//            if (value == null) return;
//            if (propertyInfo.PropertyType.IsEnum)
//            {
//                if (HandleEnum(propertyInfo, propertyInfo.PropertyType, x, value)) return;
//            }
//            if (value is Guid g && propertyInfo.PropertyType == typeof(string))
//            {
//                propertyInfo.SetValue(x, g.ToString());
//                return;

//            }
//            var nullableType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
//            if (nullableType != null)
//            {
//                if (nullableType.IsEnum && HandleEnum(propertyInfo, nullableType, x, value))
//                {
//                    return;
//                }
//                var z = Convert.ChangeType(value, nullableType);
//                propertyInfo.SetValue(x, z);
//                return;
//            }
//            var y = Convert.ChangeType(value, propertyInfo.PropertyType);
//            propertyInfo.SetValue(x, y);
//        }

//        private static bool HandleEnum(PropertyInfo propertyInfo,Type property, object x, object value)
//        {
//            if (value is string str)
//            {
//                var e = Enum.Parse(property, str, true);
//                propertyInfo.SetValue(x, e);
//                return true;
//            }

//            if (value is int i)
//            {
//                var t = Enum.ToObject(property, i);
//                propertyInfo.SetValue(x, t);
//                return true;
//            }

//            return false;
//        }

//        public IList TransformList(IList collection)
//        {
//            return collection;
//        }



//        public bool Equals(AzureSyncBaseDtoTransformer<T, TU> other)
//        {
//            if (ReferenceEquals(null, other))
//            {
//                return false;
//            }
//            if (ReferenceEquals(this, other))
//            {
//                return true;
//            }
//            return base.Equals(other);
//        }

//        public override bool Equals(object obj)
//        {
//            return Equals(obj as AzureSyncBaseDtoTransformer<T, TU>);
//        }

//        public override int GetHashCode()
//        {
//            return base.GetHashCode();
//        }
//    }
//}